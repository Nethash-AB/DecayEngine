using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Math;

// ReSharper disable InconsistentNaming

namespace DecayEngine.OpenGL.OpenGLInterop
{
    public static class GL
    {
	    private static Type delegatesClass;
	    private static FieldInfo[] delegates;

	    private static int version = 0;
        private static uint currentProgram = 0;

        // pre-allocate the float[] for matrix and array data
        private static float[] float1 = new float[1];
        private static float[] matrix4Float = new float[16];
        private static float[] matrix3Float = new float[9];
        private static double[] double1 = new double[1];
        private static uint[] uint1 = new uint[1];
        private static int[] int1 = new int[1];
        private static bool[] bool1 = new bool[1];

        static GL()
        {
            delegatesClass = typeof(Delegates);
        }

        /// <summary>
        /// Loads all OpenGL functions (core and extensions).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This function will be automatically called the first time you use any opengl function.
        /// </para>
        /// <para>
        /// Call this function manually whenever you need to update OpenGL entry points.
        /// This need may arise if you change the pixelformat/visual, or in case you cannot
        /// (or do not want) to use the automatic initialization of the GL class.
        /// </para>
        /// </remarks>
        public static void ReloadFunctions()
        {
            // Using reflection is more than 3 times faster than directly loading delegates on the first
            // run, probably due to code generation overhead. Subsequent runs are faster with direct loading
            // than with reflection, but the first time is more significant.
            if (delegates == null)
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                foreach (var field in delegatesClass.GetTypeInfo().DeclaredFields)
                    if (field.IsStatic) fields.Add(field);
                delegates = fields.ToArray();
            }

            foreach (FieldInfo f in delegates)
            {
	            Delegate del = GetDelegate(f.Name, f.FieldType);
	            f.SetValue(null, del);
            }
        }

        /// <summary>
        /// Tries to reload the given OpenGL function (core or extension).
        /// </summary>
        /// <param name="function">The name of the OpenGL function (i.e. glShaderSource)</param>
        /// <returns>True if the function was found and reloaded, false otherwise.</returns>
        /// <remarks>
        /// <para>
        /// Use this function if you require greater granularity when loading OpenGL entry points.
        /// </para>
        /// <para>
        /// While the automatic initialisation will load all OpenGL entry points, in some cases
        /// the initialisation can take place before an OpenGL Context has been established.
        /// In this case, use this function to load the entry points for the OpenGL functions
        /// you will need, or use ReloadFunctions() to load all available entry points.
        /// </para>
        /// <para>
        /// This function returns true if the given OpenGL function is supported, false otherwise.
        /// </para>
        /// <para>
        /// To query for supported extensions use the IsExtensionSupported() function instead.
        /// </para>
        /// </remarks>
        public static bool Load(string function)
        {
            //FieldInfo f = delegatesClass.GetField(function, BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo f = null;
            foreach (var field in delegatesClass.GetTypeInfo().DeclaredFields)
            {
                if (field.Name == function)
                {
                    f = field;
                    break;
                }
            }

            if (f == null)
                return false;

            Delegate old = f.GetValue(null) as Delegate;
            Delegate @new = GetDelegate(f.Name, f.FieldType);
            if (old.Target != @new.Target)
            {
                f.SetValue(null, @new);
            }
            return @new != null;
        }

        /// <summary>
        /// Creates a System.Delegate that can be used to call an OpenGL function, core or extension.
        /// </summary>
        /// <param name="name">The name of the OpenGL function (eg. "glNewList")</param>
        /// <param name="signature">The signature of the OpenGL function.</param>
        /// <returns>
        /// A System.Delegate that can be used to call this OpenGL function, or null if the specified
        /// function name did not correspond to an OpenGL function.
        /// </returns>
        public static Delegate GetDelegate(string name, Type signature)
        {
            try
            {
	            Delegate del = GetExtensionDelegate(name, signature);
	            if (del == null)
	            {
		            GameEngine.LogAppendLine(LogSeverity.Debug, "OpenGL",
			            $"Unsupported OpenGL Function: {name}. This is probably safe to ignore.");
	            }
	            return del;
            }
            catch
            {
	            GameEngine.LogAppendLine(LogSeverity.CriticalError, "OpenGL", $"Error getting delegate for: {name}, {signature}");
	            throw;
            }
        }

        /// <summary>
        /// Creates a System.Delegate that can be used to call a dynamically exported OpenGL function.
        /// </summary>
        /// <param name="name">The name of the OpenGL function (eg. "glNewList")</param>
        /// <param name="signature">The signature of the OpenGL function.</param>
        /// <returns>
        /// A System.Delegate that can be used to call this OpenGL function or null
        /// if the function is not available in the current OpenGL context.
        /// </returns>
        internal static Delegate GetExtensionDelegate(string name, Type signature)
        {
            IntPtr address = GetAddress(name);

            if (address == IntPtr.Zero ||
                address == new IntPtr(1) ||     // Workaround for buggy nvidia drivers which return
                address == new IntPtr(2))       // 1 or 2 instead of IntPtr.Zero for some extensions.
            {
                return null;
            }

            return Marshal.GetDelegateForFunctionPointer(address, signature);
        }

        /// <summary>
        /// Retrieves the entry point for a dynamically exported OpenGL function.
        /// </summary>
        /// <param name="function">The function string for the OpenGL function (eg. "glNewList")</param>
        /// <returns>
        /// An IntPtr contaning the address for the entry point, or IntPtr.Zero if the specified
        /// OpenGL function is not dynamically exported.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The Marshal.GetDelegateForFunctionPointer method can be used to turn the return value
        /// into a call-able delegate.
        /// </para>
        /// <para>
        /// This function is cross-platform. It determines the underlying platform and uses the
        /// correct wgl, glx or agl GetAddress function to retrieve the function pointer.
        /// </para>
        /// <see cref="Marshal.GetDelegateForFunctionPointer"/>
        /// </remarks>
        public static GetRenderFunctionPtrDelegate GetAddress;

        /// <summary>
        /// Select active texture unit.
        /// <para>
        /// glActiveTexture selects which texture unit subsequent texture state calls will affect. The number of
        /// texture units an implementation supports is implementation dependent, but must be at least 80.
        /// </para>
        /// </summary>
        /// <param name="texture">
        /// Specifies which texture unit to make active. The number of texture units is implementation
        /// dependent, but must be at least 80. texture must be a value between 0 and
        /// GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS minus one. The initial value is 0.
        /// </param>
        public static void ActiveTexture(int texture)
        {
#pragma warning disable CS0618
            Delegates.glActiveTexture((int)TextureUnit.Texture0 + texture);
#pragma warning restore
        }

        /// <summary>
        /// Returns the boolean value of a selected parameter.
        /// </summary>
        /// <param name="pname">A parameter that returns a single boolean.</param>
        public static bool GetBoolean(GetPName pname)
        {
	        GetBooleanv(pname, bool1);
	        return bool1[0];
        }

        /// <summary>
        /// Returns the float value of a selected parameter.
        /// </summary>
        /// <param name="pname">A parameter that returns a single float.</param>
        public static float GetFloat(GetPName pname)
        {
            GetFloatv(pname, float1);
            return float1[0];
        }

        /// <summary>
        /// Returns the double value of a selected parameter.
        /// </summary>
        /// <param name="pname">A parameter that returns a single double.</param>
        public static double GetDouble(GetPName pname)
        {
            GetDoublev(pname, double1);
            return double1[0];
        }

        /// <summary>
        /// Returns the integer value of a selected parameter.
        /// </summary>
        /// <param name="name">A parameter that returns a single integer.</param>
        public static int GetInteger(GetPName name)
        {
            GetIntegerv(name, int1);
            return int1[0];
        }

        /// <summary>
        /// Set a scalar texture parameter.
        /// </summary>
        /// <param name="target">Specificies the target for which the texture is bound.</param>
        /// <param name="pname">Specifies the name of a single-values texture parameter.</param>
        /// <param name="param">Specifies the value of pname.</param>
        public static void TexParameteri(TextureTarget target, TextureParameterName pname, TextureParameter param)
        {
            Delegates.glTexParameteri(target, pname, (int)param);
        }

        /// <summary>
        /// Set a vector texture parameter.
        /// </summary>
        /// <param name="target">Specificies the target for which the texture is bound.</param>
        /// <param name="pname">Specifies the name of a single-values texture parameter.</param>
        /// <param name="params"></param>
        public static void TexParameteriv(TextureTarget target, TextureParameterName pname, TextureParameter[] @params)
        {
            int[] iparams = new int[@params.Length];
            for (int i = 0; i < iparams.Length; i++) iparams[i] = (int)@params[i];
            Delegates.glTexParameteriv(target, pname, iparams);
        }

        /// <summary>
        /// Shortcut for quickly generating a single buffer id without creating an array to
        /// pass to the gl function.  Calls Gl.GenBuffers(1, id).
        /// </summary>
        /// <returns>The ID of the generated buffer.  0 on failure.</returns>
        public static uint GenBuffer()
        {
            uint1[0] = 0;
            GenBuffers(1, uint1);
            return uint1[0];
        }

        /// <summary>
        /// Shortcut for quickly generating a single texture id without creating an array to
        /// pass to the gl function.  Calls Gl.GenTexture(1, id).
        /// </summary>
        /// <returns>The ID of the generated texture.  0 on failure.</returns>
        public static uint GenTexture()
        {
            uint1[0] = 0;
            GenTextures(1, uint1);
            return uint1[0];
        }

        /// <summary>
        /// Shortcut for deleting a single texture without created an array to pass to the gl function.
        /// Calls Gl.DeleteTextures(1, id).
        /// </summary>
        /// <param name="texture">The ID of the texture to delete.</param>
        public static void DeleteTexture(uint texture)
        {
            uint1[0] = texture;
            DeleteTextures(1, uint1);
        }

        /// <summary>
        /// Shortcut for quickly generating a single vertex array id without creating an array to
        /// pass to the gl function.  Calls Gl.GenVertexArrays(1, id).
        /// </summary>
        /// <returns>The ID of the generated vertex array.  0 on failure.</returns>
        public static uint GenVertexArray()
        {
            uint1[0] = 0;
            GenVertexArrays(1, uint1);
            return uint1[0];
        }

        /// <summary>
        /// Shortcut for deleting a single texture without created an array to pass to the gl function.
        /// Calls Gl.DeleteVertexArrays(1, id).
        /// </summary>
        /// <param name="vao">The ID of the vertex array to delete.</param>
        public static void DeleteVertexArray(uint vao)
        {
            uint1[0] = vao;
            DeleteVertexArrays(1, uint1);
        }

        /// <summary>
        /// Shortcut for quickly generating a single framebuffer object without creating an array
        /// to pass to the gl function.  Calls Gl.GenFramebuffers(1, id).
        /// </summary>
        /// <returns>The ID of the generated framebuffer.  0 on failure.</returns>
        public static uint GenFramebuffer()
        {
            uint1[0] = 0;
            GenFramebuffers(1, uint1);
            return uint1[0];
        }

        /// <summary>
        /// Shortcut for deleting a framebuffer without created an array to pass to the gl function.
        /// Calls Gl.DeleteFramebuffers(1, id).
        /// </summary>
        /// <param name="vao">The ID of the vertex array to delete.</param>
        public static void DeleteFramebuffer(uint framebuffer)
        {
            uint1[0] = framebuffer;
            DeleteFramebuffers(1, uint1);
        }

        /// <summary>
        /// Shortcut for quickly generating a single renderbuffer object without creating an array
        /// to pass to the gl function.  Calls Gl.GenRenderbuffers(1, id).
        /// </summary>
        /// <returns>The ID of the generated framebuffer.  0 on failure.</returns>
        public static uint GenRenderbuffer()
        {
            uint1[0] = 0;
            GenRenderbuffers(1, uint1);
            return uint1[0];
        }

        /// <summary>
        /// Gets whether the shader compiled successfully.
        /// </summary>
        /// <param name="shader">The ID of the shader program.</param>
        /// <returns></returns>
        public static bool GetShaderCompileStatus(uint shader)
        {
            const int SUCCESS = 1;
            GetShaderiv(shader, ShaderParameter.CompileStatus, int1);
            return int1[0] == SUCCESS;
        }

        /// <summary>
        /// Get whether program linking was performed successfully.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool GetProgramLinkStatus(uint program)
        {
            const int SUCCESS = 1;
            GetProgramiv(program, ProgramParameter.LinkStatus, int1);
            return int1[0] == SUCCESS;
        }

        /// <summary>
        /// Gets the program info from a shader program.
        /// </summary>
        /// <param name="program">The ID of the shader program.</param>
        public static string GetProgramInfoLog(uint program)
        {
            GetProgramiv(program, ProgramParameter.InfoLogLength, int1);
            if (int1[0] == 0) return string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(int1[0]);
            GetProgramInfoLog(program, sb.Capacity, int1, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Gets the program info from a shader program.
        /// </summary>
        /// <param name="shader">The ID of the shader program.</param>
        public static string GetShaderInfoLog(uint shader)
        {
            GetShaderiv(shader, ShaderParameter.InfoLogLength, int1);
            if (int1[0] == 0) return string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(int1[0]);
            GetShaderInfoLog(shader, sb.Capacity, int1, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Replaces the source code in a shader object.
        /// </summary>
        /// <param name="shader">Specifies the handle of the shader object whose source code is to be replaced.</param>
        /// <param name="source">Specifies a string containing the source code to be loaded into the shader.</param>
        public static void ShaderSource(uint shader, string source)
        {
            int1[0] = source.Length;
            ShaderSource(shader, 1, new string[] { source }, int1);
        }

        /// <summary>
        /// Creates and initializes a buffer object's data store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Specifies the target buffer object.</param>
        /// <param name="size">Specifies the size in bytes of the buffer object's new data store.</param>
        /// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization, or NULL if no data is to be copied.</param>
        /// <param name="usage">Specifies expected usage pattern of the data store.</param>
        public static void BufferData<T>(BufferTarget target, int size, ref T[] data, BufferUsageHint usage)
            where T : struct
        {
            GCHandle data_ptr = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Delegates.glBufferData(target, new IntPtr(size), data_ptr.AddrOfPinnedObject(), usage);
            }
            finally
            {
                data_ptr.Free();
            }
        }

        /// <summary>
        /// Creates and initializes a buffer object's data store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Specifies the target buffer object.</param>
        /// <param name="position">Offset into the data from which to start copying to the buffer.</param>
        /// <param name="size">Specifies the size in bytes of the buffer object's new data store.</param>
        /// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization, or NULL if no data is to be copied.</param>
        /// <param name="usage">Specifies expected usage pattern of the data store.</param>
        public static void BufferData<T>(BufferTarget target, int position, int size, [In, Out] T[] data, BufferUsageHint usage)
            where T : struct
        {
            GCHandle data_ptr = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Delegates.glBufferData(target, new IntPtr(size), (IntPtr)((int)data_ptr.AddrOfPinnedObject() + position), usage);
            }
            finally
            {
                data_ptr.Free();
            }
        }

        /// <summary>
        /// Creates a standard VBO of type T.
        /// </summary>
        /// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
        /// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
        /// <param name="data">The data to store in the VBO.</param>
        /// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateVBO<T>(BufferTarget target, ref T[] data, BufferUsageHint hint)
            where T : struct
        {
            uint vboHandle = GenBuffer();
            if (vboHandle == 0) return 0;

            int size = data.Length * Marshal.SizeOf<T>();

            BindBuffer(target, vboHandle);
            BufferData<T>(target, size, ref data, hint);
            BindBuffer(target, 0);
            return vboHandle;
        }

        /// <summary>
        /// Creates a standard VBO of type T where the length of the VBO is less than or equal to the length of the data.
        /// </summary>
        /// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
        /// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
        /// <param name="data">The data to store in the VBO.</param>
        /// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
        /// <param name="length">The length of the VBO (will take the first 'length' elements from data).</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateVBO<T>(BufferTarget target, ref T[] data, BufferUsageHint hint, int length)
            where T : struct
        {
            uint vboHandle = GenBuffer();
            if (vboHandle == 0) return 0;

            int size = length * Marshal.SizeOf<T>();

            BindBuffer(target, vboHandle);
            BufferData<T>(target, size, ref data, hint);
            BindBuffer(target, 0);
            return vboHandle;
        }

        /// <summary>
        /// Creates a standard VBO of type T where the length of the VBO is less than or equal to the length of the data.
        /// </summary>
        /// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
        /// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
        /// <param name="data">The data to store in the VBO.</param>
        /// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
        /// <param name="position">Starting element of the data that will be copied into the VBO.</param>
        /// <param name="length">The length of the VBO (will take the first 'length' elements from data).</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateVBO<T>(BufferTarget target, ref T[] data, BufferUsageHint hint, int position, int length)
            where T : struct
        {
            uint vboHandle = GenBuffer();
            if (vboHandle == 0) return 0;

            int offset = position * Marshal.SizeOf<T>();
            int size = length * Marshal.SizeOf<T>();

            BindBuffer(target, vboHandle);
            BufferData<T>(target, offset, size, data, hint);
            BindBuffer(target, 0);
            return vboHandle;
        }

        /// <summary>
        /// Creates an interleaved VBO that contains both Vector3 and Vector3 data (typically position and normal data).
        /// </summary>
        /// <param name="target">The VBO buffer target.</param>
        /// <param name="data1">The first array of Vector3 data (usually position).</param>
        /// <param name="data2">The second array of Vector3 data (usually normal).</param>
        /// <param name="hint">Specifies expected usage pattern of the data store.</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateInterleavedVBO(BufferTarget target, Vector3[] data1, Vector3[] data2, BufferUsageHint hint)
        {
            if (data2.Length != data1.Length) throw new Exception("Data lengths must be identical to construct an interleaved VBO.");

            float[] interleaved = new float[data1.Length * 6];

            for (int i = 0, j = 0; i < data1.Length; i++)
            {
                interleaved[j++] = data1[i].X;
                interleaved[j++] = data1[i].Y;
                interleaved[j++] = data1[i].Z;

                interleaved[j++] = data2[i].X;
                interleaved[j++] = data2[i].Y;
                interleaved[j++] = data2[i].Z;
            }

            return CreateVBO<float>(target, ref interleaved, hint);
        }

        /// <summary>
        /// Creates an interleaved VBO that contains Vector3, Vector3 and Vector2 data (typically position, normal and UV data).
        /// </summary>
        /// <param name="target">The VBO buffer target.</param>
        /// <param name="data1">The first array of Vector3 data (usually position).</param>
        /// <param name="data2">The second array of Vector3 data (usually normal).</param>
        /// <param name="data3">The Vector2 data (usually UV).</param>
        /// <param name="hint">Specifies expected usage pattern of the data store.</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateInterleavedVBO(BufferTarget target, Vector3[] data1, Vector3[] data2, Vector2[] data3, BufferUsageHint hint)
        {
            if (data2.Length != data1.Length || data3.Length != data1.Length) throw new Exception("Data lengths must be identical to construct an interleaved VBO.");

            float[] interleaved = new float[data1.Length * 8];

            for (int i = 0, j = 0; i < data1.Length; i++)
            {
                interleaved[j++] = data1[i].X;
                interleaved[j++] = data1[i].Y;
                interleaved[j++] = data1[i].Z;

                interleaved[j++] = data2[i].X;
                interleaved[j++] = data2[i].Y;
                interleaved[j++] = data2[i].Z;

                interleaved[j++] = data3[i].X;
                interleaved[j++] = data3[i].Y;
            }

            return CreateVBO<float>(target, ref interleaved, hint);
        }

        /// <summary>
        /// Creates an interleaved VBO that contains Vector3, Vector3 and Vector3 data (typically position, normal and tangent data).
        /// </summary>
        /// <param name="target">The VBO buffer target.</param>
        /// <param name="data1">The first array of Vector3 data (usually position).</param>
        /// <param name="data2">The second array of Vector3 data (usually normal).</param>
        /// <param name="data3">The third array of Vector3 data (usually tangent).</param>
        /// <param name="hint">Specifies expected usage pattern of the data store.</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateInterleavedVBO(BufferTarget target, Vector3[] data1, Vector3[] data2, Vector3[] data3, BufferUsageHint hint)
        {
            if (data2.Length != data1.Length || data3.Length != data1.Length) throw new Exception("Data lengths must be identical to construct an interleaved VBO.");

            float[] interleaved = new float[data1.Length * 9];

            for (int i = 0, j = 0; i < data1.Length; i++)
            {
                interleaved[j++] = data1[i].X;
                interleaved[j++] = data1[i].Y;
                interleaved[j++] = data1[i].Z;

                interleaved[j++] = data2[i].X;
                interleaved[j++] = data2[i].Y;
                interleaved[j++] = data2[i].Z;

                interleaved[j++] = data3[i].X;
                interleaved[j++] = data3[i].Y;
                interleaved[j++] = data3[i].Z;
            }

            return CreateVBO<float>(target, ref interleaved, hint);
        }

        /// <summary>
        /// Creates an interleaved VBO that contains Vector3, Vector3, Vector3 and Vector2 data (typically position, normal, tangent and UV data).
        /// </summary>
        /// <param name="target">The VBO buffer target.</param>
        /// <param name="data1">The first array of Vector3 data (usually position).</param>
        /// <param name="data2">The second array of Vector3 data (usually normal).</param>
        /// <param name="data3">The third array of Vector3 data (usually tangent).</param>
        /// <param name="data4">The Vector2 data (usually UV).</param>
        /// <param name="hint">Specifies expected usage pattern of the data store.</param>
        /// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
        public static uint CreateInterleavedVBO(BufferTarget target, Vector3[] data1, Vector3[] data2, Vector3[] data3, Vector2[] data4, BufferUsageHint hint)
        {
            if (data2.Length != data1.Length || data3.Length != data1.Length || data4.Length != data1.Length)
            {
                throw new Exception("Data lengths must be identical to construct an interleaved VBO.");
            }

            float[] interleaved = new float[data1.Length * 11];

            for (int i = 0, j = 0; i < data1.Length; i++)
            {
                interleaved[j++] = data1[i].X;
                interleaved[j++] = data1[i].Y;
                interleaved[j++] = data1[i].Z;

                interleaved[j++] = data2[i].X;
                interleaved[j++] = data2[i].Y;
                interleaved[j++] = data2[i].Z;

                interleaved[j++] = data3[i].X;
                interleaved[j++] = data3[i].Y;
                interleaved[j++] = data3[i].Z;

                interleaved[j++] = data4[i].X;
                interleaved[j++] = data4[i].Y;
            }

            return CreateVBO<float>(target, ref interleaved, hint);
        }

//        /// <summary>
//        /// Creates a vertex array object based on a series of attribute arrays and and attribute names.
//        /// </summary>
//        /// <param name="program">The shader program that contains the attributes to be bound to.</param>
//        /// <param name="vbo">The VBO containing all of the attribute data.</param>
//        /// <param name="sizes">An array of sizes which correspond to the size of each attribute.</param>
//        /// <param name="types">An array of the attribute pointer types.</param>
//        /// <param name="targets">An array of the buffer targets.</param>
//        /// <param name="names">An array of the attribute names.</param>
//        /// <param name="stride">The stride of the VBO.</param>
//        /// <param name="eboHandle">The element buffer handle.</param>
//        /// <returns>The vertex array object (VAO) ID.</returns>
//        public static uint CreateVAO(ShaderProgram program, uint vbo, int[] sizes, Enums.VertexAttribPointerType[] types, Enums.BufferTarget[] targets, string[] names, int stride, uint eboHandle)
//        {
//            uint vaoHandle = GenVertexArray();
//            BindVertexArray(vaoHandle);
//
//            int offset = 0;
//
//            for (uint i = 0; i < names.Length; i++)
//            {
//                EnableVertexAttribArray(i);
//                BindBuffer(targets[i], vbo);
//                VertexAttribPointer(i, sizes[i], types[i], true, stride, new IntPtr(offset));
//                BindAttribLocation(program.ProgramID, i, names[i]);
//            }
//
//            BindBuffer(Enums.BufferTarget.ElementArrayBuffer, eboHandle);
//            BindVertexArray(0);
//
//            return vaoHandle;
//        }

        /// <summary>
        /// Gets the current OpenGL version (returns a cached result on subsequent calls).
        /// </summary>
        /// <returns>The current OpenGL version, or 0 on an error.</returns>
        public static int Version()
        {
            if (version != 0) return version; // cache the version information

            try
            {
                string versionString = GetString(StringName.Version);

                version = int.Parse(versionString.Substring(0, versionString.IndexOf('.')));
                return version;
            }
            catch (Exception)
            {
                //Console.WriteLine("Error while retrieving the OpenGL version.");
                return 0;
            }
        }

//        /// <summary>
//        /// Installs a program object as part of current rendering state.
//        /// </summary>
//        /// <param name="Program">Specifies the handle of the program object whose executables are to be used as part of current rendering state.</param>
//        public static void UseProgram(ShaderProgram Program)
//        {
//            UseProgram(Program.ProgramID);
//        }
//
//        /// <summary>
//        /// Bind a named texture to a texturing target
//        /// </summary>
//        /// <param name="Texture">Specifies the texture.</param>
//        public static void BindTexture(Texture Texture)
//        {
//            BindTexture(Texture.TextureTarget, Texture.TextureID);
//        }
//
//        /// <summary>
//        /// Get the index of a uniform block in the provided shader program.
//        /// Note:  This method will use the provided shader program, so make sure to
//        /// store which program is currently active and reload it if required.
//        /// </summary>
//        /// <param name="program">The shader program that contains the uniform block.</param>
//        /// <param name="uniformBlockName">The uniform block name.</param>
//        /// <returns>The index of the uniform block.</returns>
//        public static uint GetUniformBlockIndex(ShaderProgram program, string uniformBlockName)
//        {
//            return GetUniformBlockIndex(program.ProgramID, uniformBlockName);
//        }
//
//        /// <summary>
//        /// Binds a VBO based on the buffer target.
//        /// </summary>
//        /// <param name="buffer">The VBO to bind.</param>
//        public static void BindBuffer<T>(VBO<T> buffer)
//            where T : struct
//        {
//            BindBuffer(buffer.BufferTarget, buffer.ID);
//        }
//
//        /// <summary>
//        /// Binds a VBO to a shader attribute.
//        /// </summary>
//        /// <param name="buffer">The VBO to bind to the shader attribute.</param>
//        /// <param name="program">The shader program whose attribute will be bound to.</param>
//        /// <param name="attributeName">The name of the shader attribute to be bound to.</param>
//        public static void BindBufferToShaderAttribute<T>(VBO<T> buffer, ShaderProgram program, string attributeName)
//            where T : struct
//        {
//            uint location = (uint)GetAttribLocation(program.ProgramID, attributeName);
//
//            EnableVertexAttribArray(location);
//            BindBuffer(buffer);
//            VertexAttribPointer(location, buffer.Size, buffer.PointerType, true, Marshal.SizeOf<T>(), IntPtr.Zero);
//        }

        /// <summary>
        /// Delete a single OpenGL buffer.
        /// </summary>
        /// <param name="buffer">The OpenGL buffer to delete.</param>
        public static void DeleteBuffer(uint buffer)
        {
#if MEMORY_LOGGER
            MemoryLogger.DestroyVBO(buffer);
#endif

            uint1[0] = buffer;
            DeleteBuffers(1, uint1);
            uint1[0] = 0;
        }

//        /// <summary>
//        /// Set a uniform mat4 in the shader.
//        /// Uses a cached float[] to reduce memory usage.
//        /// </summary>
//        /// <param name="location">The location of the uniform in the shader.</param>
//        /// <param name="param">The Matrix4 to load into the shader uniform.</param>
//        public static void UniformMatrix4fv(int location, Matrix4x4 param)
//        {
////            matrix4Float[0] = param.M11; matrix4Float[1] = param.M12; matrix4Float[2] = param.M13; matrix4Float[3] = param.M14;
////            matrix4Float[4] = param.M21; matrix4Float[5] = param.M22; matrix4Float[6] = param.M23; matrix4Float[7] = param.M24;
////            matrix4Float[8] = param.M31; matrix4Float[9] = param.M32; matrix4Float[10] = param.M33; matrix4Float[11] = param.M34;
////            matrix4Float[12] = param.M41; matrix4Float[13] = param.M42; matrix4Float[14] = param.M43; matrix4Float[15] = param.M43;
//
//            // use the statically allocated float[] for setting the uniform
//            matrix4Float[0] = param[0].X; matrix4Float[1] = param[0].Y; matrix4Float[2] = param[0].Z; matrix4Float[3] = param[0].W;
//            matrix4Float[4] = param[1].X; matrix4Float[5] = param[1].Y; matrix4Float[6] = param[1].Z; matrix4Float[7] = param[1].W;
//            matrix4Float[8] = param[2].X; matrix4Float[9] = param[2].Y; matrix4Float[10] = param[2].Z; matrix4Float[11] = param[2].W;
//            matrix4Float[12] = param[3].X; matrix4Float[13] = param[3].Y; matrix4Float[14] = param[3].Z; matrix4Float[15] = param[3].W;
//
//            UniformMatrix4fv(location, 1, false, matrix4Float);
//        }

//        /// <summary>
//        /// Set a uniform mat3 in the shader.
//        /// Uses a cached float[] to reduce memory usage.
//        /// </summary>
//        /// <param name="location">The location of the uniform in the shader.</param>
//        /// <param name="param">The Matrix3 to load into the shader uniform.</param>
//        public static void UniformMatrix3fv(int location, Matrix3 param)
//        {
//            // use the statically allocated float[] for setting the uniform
//            matrix3Float[0] = param[0].X; matrix3Float[1] = param[0].Y; matrix3Float[2] = param[0].Z;
//            matrix3Float[3] = param[1].X; matrix3Float[4] = param[1].Y; matrix3Float[5] = param[1].Z;
//            matrix3Float[6] = param[2].X; matrix3Float[7] = param[2].Y; matrix3Float[8] = param[2].Z;
//
//            UniformMatrix3fv(location, 1, false, matrix3Float);
//        }

        /// <summary>
        /// Updates a subset of the buffer object's data store.
        /// </summary>
        /// <typeparam name="T">The type of data in the data array.</typeparam>
        /// <param name="vboID">The VBO whose buffer will be updated.</param>
        /// <param name="target">Specifies the target buffer object.  Must be ArrayBuffer, ElementArrayBuffer, PixelPackBuffer or PixelUnpackBuffer.</param>
        /// <param name="data">The new data that will be copied to the data store.</param>
        /// <param name="length">The size in bytes of the data store region being replaced.</param>
        public static void BufferSubData<T>(uint vboID, BufferTarget target, T[] data, int length)
            where T : struct
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                BindBuffer(target, vboID);
                BufferSubData(target, IntPtr.Zero, (IntPtr)(Marshal.SizeOf(data[0]) * length), handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

		public static void ActiveShaderProgram(uint pipeline, uint program)
		{
			Delegates.glActiveShaderProgram(pipeline, program);
		}

		[Obsolete("ActiveTexture(TextureUnit) is deprecated, please use ActiveTexture(int) instead.")]
		public static void ActiveTexture(TextureUnit texture)
		{
			Delegates.glActiveTexture((int)texture);
		}

		public static void AttachShader(uint program, uint shader)
		{
			Delegates.glAttachShader(program, shader);
		}

		public static void BeginConditionalRender(uint id, ConditionalRenderType mode)
		{
			Delegates.glBeginConditionalRender(id, mode);
		}

		public static void EndConditionalRender()
		{
			Delegates.glEndConditionalRender();
		}

		public static void BeginQuery(QueryTarget target, uint id)
		{
			Delegates.glBeginQuery(target, id);
		}

		public static void EndQuery(QueryTarget target)
		{
			Delegates.glEndQuery(target);
		}

		public static void BeginQueryIndexed(QueryTarget target, uint index, uint id)
		{
			Delegates.glBeginQueryIndexed(target, index, id);
		}

		public static void EndQueryIndexed(QueryTarget target, uint index)
		{
			Delegates.glEndQueryIndexed(target, index);
		}

		public static void BeginTransformFeedback(BeginFeedbackMode primitiveMode)
		{
			Delegates.glBeginTransformFeedback(primitiveMode);
		}

		public static void EndTransformFeedback()
		{
			Delegates.glEndTransformFeedback();
		}

		public static void BindAttribLocation(uint program, uint index, string name)
		{
			Delegates.glBindAttribLocation(program, index, name);
		}

		public static void BindAttribLocation(uint program, int index, string name)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glBindAttribLocation(program, (uint)index, name);
		}

		public static void BindBuffer(BufferTarget target, uint buffer)
		{
			Delegates.glBindBuffer(target, buffer);
		}

		public static void BindBufferBase(BufferTarget target, uint index, uint buffer)
		{
			Delegates.glBindBufferBase(target, index, buffer);
		}

		public static void BindBufferRange(BufferTarget target, uint index, uint buffer, IntPtr offset, IntPtr size)
		{
			Delegates.glBindBufferRange(target, index, buffer, offset, size);
		}

		public static void BindBuffersBase(BufferTarget target, uint first, int count, uint[] buffers)
		{
			Delegates.glBindBuffersBase(target, first, count, buffers);
		}

		public static void BindBuffersRange(BufferTarget target, uint first, int count, uint[] buffers, IntPtr[] offsets, IntPtr[] sizes)
		{
			Delegates.glBindBuffersRange(target, first, count, buffers, offsets, sizes);
		}

		public static void BindFragDataLocation(uint program, uint colorNumber, string name)
		{
			Delegates.glBindFragDataLocation(program, colorNumber, name);
		}

		public static void BindFragDataLocationIndexed(uint program, uint colorNumber, uint index, string name)
		{
			Delegates.glBindFragDataLocationIndexed(program, colorNumber, index, name);
		}

		public static void BindFramebuffer(FramebufferTarget target, uint framebuffer)
		{
			Delegates.glBindFramebuffer(target, framebuffer);
		}

		public static void BindImageTexture(uint unit, uint texture, int level, bool layered, int layer, BufferAccess access, PixelInternalFormat format)
		{
			Delegates.glBindImageTexture(unit, texture, level, layered, layer, access, format);
		}

		public static void BindImageTextures(uint first, int count, uint[] textures)
		{
			Delegates.glBindImageTextures(first, count, textures);
		}

		public static void BindProgramPipeline(uint pipeline)
		{
			Delegates.glBindProgramPipeline(pipeline);
		}

		public static void BindRenderbuffer(RenderbufferTarget target, uint renderbuffer)
		{
			Delegates.glBindRenderbuffer(target, renderbuffer);
		}

		public static void BindSampler(uint unit, uint sampler)
		{
			Delegates.glBindSampler(unit, sampler);
		}

		public static void BindSamplers(uint first, int count, uint[] samplers)
		{
			Delegates.glBindSamplers(first, count, samplers);
		}

		public static void BindTexture(TextureTarget target, uint texture)
		{
			Delegates.glBindTexture(target, texture);
		}

		public static void BindTextures(uint first, int count, uint[] textures)
		{
			Delegates.glBindTextures(first, count, textures);
		}

		public static void BindTextureUnit(uint unit, uint texture)
		{
			Delegates.glBindTextureUnit(unit, texture);
		}

		public static void BindTransformFeedback(NvTransformFeedback2 target, uint id)
		{
			Delegates.glBindTransformFeedback(target, id);
		}

		public static void BindVertexArray(uint array)
		{
			Delegates.glBindVertexArray(array);
		}

		public static void BindVertexBuffer(uint bindingindex, uint buffer, IntPtr offset, IntPtr stride)
		{
			Delegates.glBindVertexBuffer(bindingindex, buffer, offset, stride);
		}

		public static void VertexArrayVertexBuffer(uint vaobj, uint bindingindex, uint buffer, IntPtr offset, int stride)
		{
			Delegates.glVertexArrayVertexBuffer(vaobj, bindingindex, buffer, offset, stride);
		}

		public static void BindVertexBuffers(uint first, int count, uint[] buffers, IntPtr[] offsets, int[] strides)
		{
			Delegates.glBindVertexBuffers(first, count, buffers, offsets, strides);
		}

		public static void VertexArrayVertexBuffers(uint vaobj, uint first, int count, uint[] buffers, IntPtr[] offsets, int[] strides)
		{
			Delegates.glVertexArrayVertexBuffers(vaobj, first, count, buffers, offsets, strides);
		}

		public static void BlendColor(float red, float green, float blue, float alpha)
		{
			Delegates.glBlendColor(red, green, blue, alpha);
		}

		public static void BlendEquation(BlendEquationMode mode)
		{
			Delegates.glBlendEquation(mode);
		}

		public static void BlendEquationi(uint buf, BlendEquationMode mode)
		{
			Delegates.glBlendEquationi(buf, mode);
		}

		public static void BlendEquationSeparate(BlendEquationMode modeRGB, BlendEquationMode modeAlpha)
		{
			Delegates.glBlendEquationSeparate(modeRGB, modeAlpha);
		}

		public static void BlendEquationSeparatei(uint buf, BlendEquationMode modeRGB, BlendEquationMode modeAlpha)
		{
			Delegates.glBlendEquationSeparatei(buf, modeRGB, modeAlpha);
		}

		public static void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
		{
			Delegates.glBlendFunc(sfactor, dfactor);
		}

		public static void BlendFunci(uint buf, BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
		{
			Delegates.glBlendFunci(buf, sfactor, dfactor);
		}

		public static void BlendFuncSeparate(BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha)
		{
			Delegates.glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
		}

		public static void BlendFuncSeparatei(uint buf, BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha)
		{
			Delegates.glBlendFuncSeparatei(buf, srcRGB, dstRGB, srcAlpha, dstAlpha);
		}

		public static void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter)
		{
			Delegates.glBlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
		}

		public static void BlitNamedFramebuffer(uint readFramebuffer, uint drawFramebuffer, int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter)
		{
			Delegates.glBlitNamedFramebuffer(readFramebuffer, drawFramebuffer, srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
		}

		public static void BufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage)
		{
			Delegates.glBufferData(target, size, data, usage);
		}

		public static void NamedBufferData(uint buffer, int size, IntPtr data, BufferUsageHint usage)
		{
			Delegates.glNamedBufferData(buffer, size, data, usage);
		}

		public static void BufferStorage(BufferTarget target, IntPtr size, IntPtr data, uint flags)
		{
			Delegates.glBufferStorage(target, size, data, flags);
		}

		public static void NamedBufferStorage(uint buffer, int size, IntPtr data, uint flags)
		{
			Delegates.glNamedBufferStorage(buffer, size, data, flags);
		}

		public static void BufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data)
		{
			Delegates.glBufferSubData(target, offset, size, data);
		}

		public static void NamedBufferSubData(uint buffer, IntPtr offset, int size, IntPtr data)
		{
			Delegates.glNamedBufferSubData(buffer, offset, size, data);
		}

		public static FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target)
		{
			return Delegates.glCheckFramebufferStatus(target);
		}

		public static FramebufferErrorCode CheckNamedFramebufferStatus(uint framebuffer, FramebufferTarget target)
		{
			return Delegates.glCheckNamedFramebufferStatus(framebuffer, target);
		}

		public static void ClampColor(ClampColorTarget target, ClampColorMode clamp)
		{
			Delegates.glClampColor(target, clamp);
		}

		public static void Clear(ClearBufferMask mask)
		{
			Delegates.glClear(mask);
		}

		public static void ClearBufferiv(ClearBuffer buffer, int drawbuffer, int[] value)
		{
			Delegates.glClearBufferiv(buffer, drawbuffer, value);
		}

		public static void ClearBufferuiv(ClearBuffer buffer, int drawbuffer, uint[] value)
		{
			Delegates.glClearBufferuiv(buffer, drawbuffer, value);
		}

		public static void ClearBufferfv(ClearBuffer buffer, int drawbuffer, float[] value)
		{
			Delegates.glClearBufferfv(buffer, drawbuffer, value);
		}

		public static void ClearBufferfi(ClearBuffer buffer, int drawbuffer, float depth, int stencil)
		{
			Delegates.glClearBufferfi(buffer, drawbuffer, depth, stencil);
		}

		public static void ClearNamedFramebufferiv(uint framebuffer, ClearBuffer buffer, int drawbuffer, int[] value)
		{
			Delegates.glClearNamedFramebufferiv(framebuffer, buffer, drawbuffer, value);
		}

		public static void ClearNamedFramebufferuiv(uint framebuffer, ClearBuffer buffer, int drawbuffer, uint[] value)
		{
			Delegates.glClearNamedFramebufferuiv(framebuffer, buffer, drawbuffer, value);
		}

		public static void ClearNamedFramebufferfv(uint framebuffer, ClearBuffer buffer, int drawbuffer, float[] value)
		{
			Delegates.glClearNamedFramebufferfv(framebuffer, buffer, drawbuffer, value);
		}

		public static void ClearNamedFramebufferfi(uint framebuffer, ClearBuffer buffer, int drawbuffer, float depth, int stencil)
		{
			Delegates.glClearNamedFramebufferfi(framebuffer, buffer, drawbuffer, depth, stencil);
		}

		public static void ClearBufferData(BufferTarget target, SizedInternalFormat internalFormat, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearBufferData(target, internalFormat, format, type, data);
		}

		public static void ClearNamedBufferData(uint buffer, SizedInternalFormat internalFormat, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearNamedBufferData(buffer, internalFormat, format, type, data);
		}

		public static void ClearBufferSubData(BufferTarget target, SizedInternalFormat internalFormat, IntPtr offset, IntPtr size, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearBufferSubData(target, internalFormat, offset, size, format, type, data);
		}

		public static void ClearNamedBufferSubData(uint buffer, SizedInternalFormat internalFormat, IntPtr offset, int size, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearNamedBufferSubData(buffer, internalFormat, offset, size, format, type, data);
		}

		public static void ClearColor(float red, float green, float blue, float alpha)
		{
			Delegates.glClearColor(red, green, blue, alpha);
		}

		public static void ClearDepth(double depth)
		{
			Delegates.glClearDepth(depth);
		}

		public static void ClearDepthf(float depth)
		{
			Delegates.glClearDepthf(depth);
		}

		public static void ClearStencil(int s)
		{
			Delegates.glClearStencil(s);
		}

		public static void ClearTexImage(uint texture, int level, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearTexImage(texture, level, format, type, data);
		}

		public static void ClearTexSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			Delegates.glClearTexSubImage(texture, level, xoffset, yoffset, zoffset, width, height, depth, format, type, data);
		}

		public static ArbSync ClientWaitSync(IntPtr sync, uint flags, ulong timeout)
		{
			return Delegates.glClientWaitSync(sync, flags, timeout);
		}

		public static void ClipControl(ClipControlOrigin origin, ClipControlDepth depth)
		{
			Delegates.glClipControl(origin, depth);
		}

		public static void ColorMask(bool red, bool green, bool blue, bool alpha)
		{
			Delegates.glColorMask(red, green, blue, alpha);
		}

		public static void ColorMaski(uint buf, bool red, bool green, bool blue, bool alpha)
		{
			Delegates.glColorMaski(buf, red, green, blue, alpha);
		}

		public static void CompileShader(uint shader)
		{
			Delegates.glCompileShader(shader);
		}

		public static void CompressedTexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int border, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexImage1D(target, level, internalFormat, width, border, imageSize, data);
		}

		public static void CompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int border, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexImage2D(target, level, internalFormat, width, height, border, imageSize, data);
		}

		public static void CompressedTexImage3D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int depth, int border, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexImage3D(target, level, internalFormat, width, height, depth, border, imageSize, data);
		}

		public static void CompressedTexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexSubImage1D(target, level, xoffset, width, format, imageSize, data);
		}

		public static void CompressedTextureSubImage1D(uint texture, int level, int xoffset, int width, PixelInternalFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTextureSubImage1D(texture, level, xoffset, width, format, imageSize, data);
		}

		public static void CompressedTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexSubImage2D(target, level, xoffset, yoffset, width, height, format, imageSize, data);
		}

		public static void CompressedTextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelInternalFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, imageSize, data);
		}

		public static void CompressedTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTexSubImage3D(target, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);
		}

		public static void CompressedTextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelInternalFormat format, int imageSize, IntPtr data)
		{
			Delegates.glCompressedTextureSubImage3D(texture, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);
		}

		public static void CopyBufferSubData(BufferTarget readTarget, BufferTarget writeTarget, IntPtr readOffset, IntPtr writeOffset, IntPtr size)
		{
			Delegates.glCopyBufferSubData(readTarget, writeTarget, readOffset, writeOffset, size);
		}

		public static void CopyNamedBufferSubData(uint readBuffer, uint writeBuffer, IntPtr readOffset, IntPtr writeOffset, int size)
		{
			Delegates.glCopyNamedBufferSubData(readBuffer, writeBuffer, readOffset, writeOffset, size);
		}

		public static void CopyImageSubData(uint srcName, BufferTarget srcTarget, int srcLevel, int srcX, int srcY, int srcZ, uint dstName, BufferTarget dstTarget, int dstLevel, int dstX, int dstY, int dstZ, int srcWidth, int srcHeight, int srcDepth)
		{
			Delegates.glCopyImageSubData(srcName, srcTarget, srcLevel, srcX, srcY, srcZ, dstName, dstTarget, dstLevel, dstX, dstY, dstZ, srcWidth, srcHeight, srcDepth);
		}

		public static void CopyTexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int border)
		{
			Delegates.glCopyTexImage1D(target, level, internalFormat, x, y, width, border);
		}

		public static void CopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int height, int border)
		{
			Delegates.glCopyTexImage2D(target, level, internalFormat, x, y, width, height, border);
		}

		public static void CopyTexSubImage1D(TextureTarget target, int level, int xoffset, int x, int y, int width)
		{
			Delegates.glCopyTexSubImage1D(target, level, xoffset, x, y, width);
		}

		public static void CopyTextureSubImage1D(uint texture, int level, int xoffset, int x, int y, int width)
		{
			Delegates.glCopyTextureSubImage1D(texture, level, xoffset, x, y, width);
		}

		public static void CopyTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int x, int y, int width, int height)
		{
			Delegates.glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
		}

		public static void CopyTextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int x, int y, int width, int height)
		{
			Delegates.glCopyTextureSubImage2D(texture, level, xoffset, yoffset, x, y, width, height);
		}

		public static void CopyTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height)
		{
			Delegates.glCopyTexSubImage3D(target, level, xoffset, yoffset, zoffset, x, y, width, height);
		}

		public static void CopyTextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height)
		{
			Delegates.glCopyTextureSubImage3D(texture, level, xoffset, yoffset, zoffset, x, y, width, height);
		}

		public static void CreateBuffers(int n, uint[] buffers)
		{
			Delegates.glCreateBuffers(n, buffers);
		}

		public static void CreateFramebuffers(int n, uint[] ids)
		{
			Delegates.glCreateFramebuffers(n, ids);
		}

		public static uint CreateProgram()
		{
			return Delegates.glCreateProgram();
		}

		public static void CreateProgramPipelines(int n, uint[] pipelines)
		{
			Delegates.glCreateProgramPipelines(n, pipelines);
		}

		public static void CreateQueries(QueryTarget target, int n, uint[] ids)
		{
			Delegates.glCreateQueries(target, n, ids);
		}

		public static void CreateRenderbuffers(int n, uint[] renderbuffers)
		{
			Delegates.glCreateRenderbuffers(n, renderbuffers);
		}

		public static void CreateSamplers(int n, uint[] samplers)
		{
			Delegates.glCreateSamplers(n, samplers);
		}

		public static uint CreateShader(ShaderType shaderType)
		{
			return Delegates.glCreateShader(shaderType);
		}

		public static uint CreateShaderProgramv(ShaderType type, int count, string[] strings)
		{
			return Delegates.glCreateShaderProgramv(type, count, strings);
		}

		public static void CreateTextures(TextureTarget target, int n, uint[] textures)
		{
			if (Delegates.glCreateTextures == null)
			{
				Delegates.glGenTextures(n, textures);
				for (int i = 0; i < n; i++)
				{
					Delegates.glBindTexture(target, textures[i]);
				}

				return;
			}

			Delegates.glCreateTextures(target, n, textures);
		}

		public static void CreateTransformFeedbacks(int n, uint[] ids)
		{
			Delegates.glCreateTransformFeedbacks(n, ids);
		}

		public static void CreateVertexArrays(int n, uint[] arrays)
		{
			Delegates.glCreateVertexArrays(n, arrays);
		}

		public static void CullFace(CullFaceMode mode)
		{
			Delegates.glCullFace(mode);
		}

		public static void DeleteBuffers(int n, uint[] buffers)
		{
			Delegates.glDeleteBuffers(n, buffers);
		}

		public static void DeleteFramebuffers(int n, uint[] framebuffers)
		{
			Delegates.glDeleteFramebuffers(n, framebuffers);
		}

		public static void DeleteProgram(uint program)
		{
			Delegates.glDeleteProgram(program);
		}

		public static void DeleteProgramPipelines(int n, uint[] pipelines)
		{
			Delegates.glDeleteProgramPipelines(n, pipelines);
		}

		public static void DeleteQueries(int n, uint[] ids)
		{
			Delegates.glDeleteQueries(n, ids);
		}

		public static void DeleteRenderbuffers(int n, uint[] renderbuffers)
		{
			Delegates.glDeleteRenderbuffers(n, renderbuffers);
		}

		public static void DeleteSamplers(int n, uint[] samplers)
		{
			Delegates.glDeleteSamplers(n, samplers);
		}

		public static void DeleteShader(uint shader)
		{
			Delegates.glDeleteShader(shader);
		}

		public static void DeleteSync(IntPtr sync)
		{
			Delegates.glDeleteSync(sync);
		}

		public static void DeleteTextures(int n, uint[] textures)
		{
			Delegates.glDeleteTextures(n, textures);
		}

		public static void DeleteTransformFeedbacks(int n, uint[] ids)
		{
			Delegates.glDeleteTransformFeedbacks(n, ids);
		}

		public static void DeleteVertexArrays(int n, uint[] arrays)
		{
			Delegates.glDeleteVertexArrays(n, arrays);
		}

		public static void DepthFunc(DepthFunction func)
		{
			Delegates.glDepthFunc(func);
		}

		public static void DepthMask(bool flag)
		{
			Delegates.glDepthMask(flag);
		}

		public static void DepthRange(double nearVal, double farVal)
		{
			Delegates.glDepthRange(nearVal, farVal);
		}

		public static void DepthRangef(float nearVal, float farVal)
		{
			Delegates.glDepthRangef(nearVal, farVal);
		}

		public static void DepthRangeArrayv(uint first, int count, double[] v)
		{
			Delegates.glDepthRangeArrayv(first, count, v);
		}

		public static void DepthRangeIndexed(uint index, double nearVal, double farVal)
		{
			Delegates.glDepthRangeIndexed(index, nearVal, farVal);
		}

		public static void DetachShader(uint program, uint shader)
		{
			Delegates.glDetachShader(program, shader);
		}

		public static void DispatchCompute(uint num_groups_x, uint num_groups_y, uint num_groups_z)
		{
			Delegates.glDispatchCompute(num_groups_x, num_groups_y, num_groups_z);
		}

		public static void DispatchComputeIndirect(IntPtr indirect)
		{
			Delegates.glDispatchComputeIndirect(indirect);
		}

		public static void DrawArrays(BeginMode mode, int first, int count)
		{
			Delegates.glDrawArrays(mode, first, count);
		}

		public static void DrawArraysIndirect(BeginMode mode, IntPtr indirect)
		{
			Delegates.glDrawArraysIndirect(mode, indirect);
		}

		public static void DrawArraysInstanced(BeginMode mode, int first, int count, int primcount)
		{
			Delegates.glDrawArraysInstanced(mode, first, count, primcount);
		}

		public static void DrawArraysInstancedBaseInstance(BeginMode mode, int first, int count, int primcount, uint baseinstance)
		{
			Delegates.glDrawArraysInstancedBaseInstance(mode, first, count, primcount, baseinstance);
		}

		public static void DrawBuffer(DrawBufferMode buf)
		{
			Delegates.glDrawBuffer(buf);
		}

		public static void NamedFramebufferDrawBuffer(uint framebuffer, DrawBufferMode buf)
		{
			Delegates.glNamedFramebufferDrawBuffer(framebuffer, buf);
		}

		public static void DrawBuffers(int n, DrawBuffersEnum[] bufs)
		{
			Delegates.glDrawBuffers(n, bufs);
		}

		public static void NamedFramebufferDrawBuffers(uint framebuffer, int n, DrawBufferMode[] bufs)
		{
			Delegates.glNamedFramebufferDrawBuffers(framebuffer, n, bufs);
		}

		public static void DrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices)
		{
			Delegates.glDrawElements(mode, count, type, indices);
		}

		public static void DrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int basevertex)
		{
			Delegates.glDrawElementsBaseVertex(mode, count, type, indices, basevertex);
		}

		public static void DrawElementsIndirect(BeginMode mode, DrawElementsType type, IntPtr indirect)
		{
			Delegates.glDrawElementsIndirect(mode, type, indirect);
		}

		public static void DrawElementsInstanced(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount)
		{
			Delegates.glDrawElementsInstanced(mode, count, type, indices, primcount);
		}

		public static void DrawElementsInstancedBaseInstance(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, uint baseinstance)
		{
			Delegates.glDrawElementsInstancedBaseInstance(mode, count, type, indices, primcount, baseinstance);
		}

		public static void DrawElementsInstancedBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex)
		{
			Delegates.glDrawElementsInstancedBaseVertex(mode, count, type, indices, primcount, basevertex);
		}

		public static void DrawElementsInstancedBaseVertexBaseInstance(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex, uint baseinstance)
		{
			Delegates.glDrawElementsInstancedBaseVertexBaseInstance(mode, count, type, indices, primcount, basevertex, baseinstance);
		}

		public static void DrawRangeElements(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices)
		{
			Delegates.glDrawRangeElements(mode, start, end, count, type, indices);
		}

		public static void DrawRangeElementsBaseVertex(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices, int basevertex)
		{
			Delegates.glDrawRangeElementsBaseVertex(mode, start, end, count, type, indices, basevertex);
		}

		public static void DrawTransformFeedback(NvTransformFeedback2 mode, uint id)
		{
			Delegates.glDrawTransformFeedback(mode, id);
		}

		public static void DrawTransformFeedbackInstanced(BeginMode mode, uint id, int primcount)
		{
			Delegates.glDrawTransformFeedbackInstanced(mode, id, primcount);
		}

		public static void DrawTransformFeedbackStream(NvTransformFeedback2 mode, uint id, uint stream)
		{
			Delegates.glDrawTransformFeedbackStream(mode, id, stream);
		}

		public static void DrawTransformFeedbackStreamInstanced(BeginMode mode, uint id, uint stream, int primcount)
		{
			Delegates.glDrawTransformFeedbackStreamInstanced(mode, id, stream, primcount);
		}

		public static void Enable(EnableCap cap)
		{
			Delegates.glEnable(cap);
		}

		public static void Disable(EnableCap cap)
		{
			Delegates.glDisable(cap);
		}

		public static void Enablei(EnableCap cap, uint index)
		{
			Delegates.glEnablei(cap, index);
		}

		public static void Disablei(EnableCap cap, uint index)
		{
			Delegates.glDisablei(cap, index);
		}

		public static void EnableVertexAttribArray(uint index)
		{
			Delegates.glEnableVertexAttribArray(index);
		}

		public static void EnableVertexAttribArray(int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glEnableVertexAttribArray((uint)index);
		}

		public static void DisableVertexAttribArray(uint index)
		{
			Delegates.glDisableVertexAttribArray(index);
		}

		public static void DisableVertexAttribArray(int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glDisableVertexAttribArray((uint)index);
		}

		public static void EnableVertexArrayAttrib(uint vaobj, uint index)
		{
			Delegates.glEnableVertexArrayAttrib(vaobj, index);
		}

		public static void EnableVertexArrayAttrib(uint vaobj, int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glEnableVertexArrayAttrib(vaobj, (uint)index);
		}

		public static void DisableVertexArrayAttrib(uint vaobj, uint index)
		{
			Delegates.glDisableVertexArrayAttrib(vaobj, index);
		}

		public static void DisableVertexArrayAttrib(uint vaobj, int index)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glDisableVertexArrayAttrib(vaobj, (uint)index);
		}

		public static IntPtr FenceSync(ArbSync condition, uint flags)
		{
			return Delegates.glFenceSync(condition, flags);
		}

		public static void Finish()
		{
			Delegates.glFinish();
		}

		public static void Flush()
		{
			Delegates.glFlush();
		}

		public static void FlushMappedBufferRange(BufferTarget target, IntPtr offset, IntPtr length)
		{
			Delegates.glFlushMappedBufferRange(target, offset, length);
		}

		public static void FlushMappedNamedBufferRange(uint buffer, IntPtr offset, int length)
		{
			Delegates.glFlushMappedNamedBufferRange(buffer, offset, length);
		}

		public static void FramebufferParameteri(FramebufferTarget target, FramebufferPName pname, int param)
		{
			Delegates.glFramebufferParameteri(target, pname, param);
		}

		public static void NamedFramebufferParameteri(uint framebuffer, FramebufferPName pname, int param)
		{
			Delegates.glNamedFramebufferParameteri(framebuffer, pname, param);
		}

		public static void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer)
		{
			Delegates.glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
		}

		public static void NamedFramebufferRenderbuffer(uint framebuffer, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer)
		{
			Delegates.glNamedFramebufferRenderbuffer(framebuffer, attachment, renderbuffertarget, renderbuffer);
		}

		public static void FramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level)
		{
			Delegates.glFramebufferTexture(target, attachment, texture, level);
		}

		public static void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
		{
			Delegates.glFramebufferTexture1D(target, attachment, textarget, texture, level);
		}

		public static void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
		{
			Delegates.glFramebufferTexture2D(target, attachment, textarget, texture, level);
		}

		public static void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level, int layer)
		{
			Delegates.glFramebufferTexture3D(target, attachment, textarget, texture, level, layer);
		}

		public static void NamedFramebufferTexture(uint framebuffer, FramebufferAttachment attachment, uint texture, int level)
		{
			Delegates.glNamedFramebufferTexture(framebuffer, attachment, texture, level);
		}

		public static void FramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer)
		{
			Delegates.glFramebufferTextureLayer(target, attachment, texture, level, layer);
		}

		public static void NamedFramebufferTextureLayer(uint framebuffer, FramebufferAttachment attachment, uint texture, int level, int layer)
		{
			Delegates.glNamedFramebufferTextureLayer(framebuffer, attachment, texture, level, layer);
		}

		public static void FrontFace(FrontFaceDirection mode)
		{
			Delegates.glFrontFace(mode);
		}

		public static void GenBuffers(int n, [OutAttribute] uint[] buffers)
		{
			Delegates.glGenBuffers(n, buffers);
		}

		public static void GenerateMipmap(GenerateMipmapTarget target)
		{
			Delegates.glGenerateMipmap(target);
		}

		public static void GenerateTextureMipmap(uint texture)
		{
			Delegates.glGenerateTextureMipmap(texture);
		}

		public static void GenFramebuffers(int n, [OutAttribute] uint[] ids)
		{
			Delegates.glGenFramebuffers(n, ids);
		}

		public static void GenProgramPipelines(int n, [OutAttribute] uint[] pipelines)
		{
			Delegates.glGenProgramPipelines(n, pipelines);
		}

		public static void GenQueries(int n, [OutAttribute] uint[] ids)
		{
			Delegates.glGenQueries(n, ids);
		}

		public static void GenRenderbuffers(int n, [OutAttribute] uint[] renderbuffers)
		{
			Delegates.glGenRenderbuffers(n, renderbuffers);
		}

		public static void GenSamplers(int n, [OutAttribute] uint[] samplers)
		{
			Delegates.glGenSamplers(n, samplers);
		}

		public static void GenTextures(int n, [OutAttribute] uint[] textures)
		{
			Delegates.glGenTextures(n, textures);
		}

		public static void GenTransformFeedbacks(int n, [OutAttribute] uint[] ids)
		{
			Delegates.glGenTransformFeedbacks(n, ids);
		}

		public static void GenVertexArrays(int n, [OutAttribute] uint[] arrays)
		{
			Delegates.glGenVertexArrays(n, arrays);
		}

		public static void GetBooleanv(GetPName pname, [OutAttribute] bool[] data)
		{
			Delegates.glGetBooleanv(pname, data);
		}

		public static void GetDoublev(GetPName pname, [OutAttribute] double[] data)
		{
			Delegates.glGetDoublev(pname, data);
		}

		public static void GetFloatv(GetPName pname, [OutAttribute] float[] data)
		{
			Delegates.glGetFloatv(pname, data);
		}

		public static void GetIntegerv(GetPName pname, [OutAttribute] int[] data)
		{
			Delegates.glGetIntegerv(pname, data);
		}

		public static void GetInteger64v(ArbSync pname, [OutAttribute] long[] data)
		{
			Delegates.glGetInteger64v(pname, data);
		}

		public static void GetBooleani_v(GetPName target, uint index, [OutAttribute] bool[] data)
		{
			Delegates.glGetBooleani_v(target, index, data);
		}

		public static void GetIntegeri_v(GetPName target, uint index, [OutAttribute] int[] data)
		{
			Delegates.glGetIntegeri_v(target, index, data);
		}

		public static void GetFloati_v(GetPName target, uint index, [OutAttribute] float[] data)
		{
			Delegates.glGetFloati_v(target, index, data);
		}

		public static void GetDoublei_v(GetPName target, uint index, [OutAttribute] double[] data)
		{
			Delegates.glGetDoublei_v(target, index, data);
		}

		public static void GetInteger64i_v(GetPName target, uint index, [OutAttribute] long[] data)
		{
			Delegates.glGetInteger64i_v(target, index, data);
		}

		public static void GetActiveAtomicCounterBufferiv(uint program, uint bufferIndex, AtomicCounterParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetActiveAtomicCounterBufferiv(program, bufferIndex, pname, @params);
		}

		public static void GetActiveAttrib(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveAttribType[] type, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetActiveAttrib(program, index, bufSize, length, size, type, name);
		}

		public static void GetActiveAttrib(uint program, int index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveAttribType[] type, [OutAttribute] System.Text.StringBuilder name)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetActiveAttrib(program, (uint)index, bufSize, length, size, type, name);
		}

		public static void GetActiveSubroutineName(uint program, ShaderType shadertype, uint index, int bufsize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetActiveSubroutineName(program, shadertype, index, bufsize, length, name);
		}

		public static void GetActiveSubroutineUniformiv(uint program, ShaderType shadertype, uint index, SubroutineParameterName pname, [OutAttribute] int[] values)
		{
			Delegates.glGetActiveSubroutineUniformiv(program, shadertype, index, pname, values);
		}

		public static void GetActiveSubroutineUniformiv(uint program, ShaderType shadertype, int index, SubroutineParameterName pname, [OutAttribute] int[] values)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetActiveSubroutineUniformiv(program, shadertype, (uint)index, pname, values);
		}

		public static void GetActiveSubroutineUniformName(uint program, ShaderType shadertype, uint index, int bufsize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetActiveSubroutineUniformName(program, shadertype, index, bufsize, length, name);
		}

		public static void GetActiveSubroutineUniformName(uint program, ShaderType shadertype, int index, int bufsize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetActiveSubroutineUniformName(program, shadertype, (uint)index, bufsize, length, name);
		}

		public static void GetActiveUniform(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveUniformType[] type, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetActiveUniform(program, index, bufSize, length, size, type, name);
		}

		public static void GetActiveUniform(uint program, int index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveUniformType[] type, [OutAttribute] System.Text.StringBuilder name)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetActiveUniform(program, (uint)index, bufSize, length, size, type, name);
		}

		public static void GetActiveUniformBlockiv(uint program, uint uniformBlockIndex, ActiveUniformBlockParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetActiveUniformBlockiv(program, uniformBlockIndex, pname, @params);
		}

		public static void GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder uniformBlockName)
		{
			Delegates.glGetActiveUniformBlockName(program, uniformBlockIndex, bufSize, length, uniformBlockName);
		}

		public static void GetActiveUniformName(uint program, uint uniformIndex, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder uniformName)
		{
			Delegates.glGetActiveUniformName(program, uniformIndex, bufSize, length, uniformName);
		}

		public static void GetActiveUniformsiv(uint program, int uniformCount, [OutAttribute] uint[] uniformIndices, ActiveUniformType pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetActiveUniformsiv(program, uniformCount, uniformIndices, pname, @params);
		}

		public static void GetAttachedShaders(uint program, int maxCount, [OutAttribute] int[] count, [OutAttribute] uint[] shaders)
		{
			Delegates.glGetAttachedShaders(program, maxCount, count, shaders);
		}

		public static int GetAttribLocation(uint program, string name)
		{
			return Delegates.glGetAttribLocation(program, name);
		}

		public static void GetBufferParameteriv(BufferTarget target, BufferParameterName value, [OutAttribute] int[] data)
		{
			Delegates.glGetBufferParameteriv(target, value, data);
		}

		public static void GetBufferParameteri64v(BufferTarget target, BufferParameterName value, [OutAttribute] long[] data)
		{
			Delegates.glGetBufferParameteri64v(target, value, data);
		}

		public static void GetNamedBufferParameteriv(uint buffer, BufferParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetNamedBufferParameteriv(buffer, pname, @params);
		}

		public static void GetNamedBufferParameteri64v(uint buffer, BufferParameterName pname, [OutAttribute] long[] @params)
		{
			Delegates.glGetNamedBufferParameteri64v(buffer, pname, @params);
		}

		public static void GetBufferPointerv(BufferTarget target, BufferPointer pname, [OutAttribute] IntPtr @params)
		{
			Delegates.glGetBufferPointerv(target, pname, @params);
		}

		public static void GetNamedBufferPointerv(uint buffer, BufferPointer pname, [OutAttribute] IntPtr @params)
		{
			Delegates.glGetNamedBufferPointerv(buffer, pname, @params);
		}

		public static void GetBufferSubData(BufferTarget target, IntPtr offset, IntPtr size, [OutAttribute] IntPtr data)
		{
			Delegates.glGetBufferSubData(target, offset, size, data);
		}

		public static void GetNamedBufferSubData(uint buffer, IntPtr offset, int size, [OutAttribute] IntPtr data)
		{
			Delegates.glGetNamedBufferSubData(buffer, offset, size, data);
		}

		public static void GetCompressedTexImage(TextureTarget target, int level, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetCompressedTexImage(target, level, pixels);
		}

		public static void GetnCompressedTexImage(TextureTarget target, int level, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetnCompressedTexImage(target, level, bufSize, pixels);
		}

		public static void GetCompressedTextureImage(uint texture, int level, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetCompressedTextureImage(texture, level, bufSize, pixels);
		}

		public static void GetCompressedTextureSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetCompressedTextureSubImage(texture, level, xoffset, yoffset, zoffset, width, height, depth, bufSize, pixels);
		}

		public static ErrorCode GetError()
		{
			return Delegates.glGetError();
		}

		public static int GetFragDataIndex(uint program, string name)
		{
			return Delegates.glGetFragDataIndex(program, name);
		}

		public static int GetFragDataLocation(uint program, string name)
		{
			return Delegates.glGetFragDataLocation(program, name);
		}

		public static void GetFramebufferAttachmentParameteriv(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetFramebufferAttachmentParameteriv(target, attachment, pname, @params);
		}

		public static void GetNamedFramebufferAttachmentParameteriv(uint framebuffer, FramebufferAttachment attachment, FramebufferParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetNamedFramebufferAttachmentParameteriv(framebuffer, attachment, pname, @params);
		}

		public static void GetFramebufferParameteriv(FramebufferTarget target, FramebufferPName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetFramebufferParameteriv(target, pname, @params);
		}

		public static void GetNamedFramebufferParameteriv(uint framebuffer, FramebufferPName pname, [OutAttribute] int[] param)
		{
			Delegates.glGetNamedFramebufferParameteriv(framebuffer, pname, param);
		}

		public static GraphicResetStatus GetGraphicsResetStatus()
		{
			return Delegates.glGetGraphicsResetStatus();
		}

		public static void GetInternalformativ(TextureTarget target, PixelInternalFormat internalFormat, GetPName pname, int bufSize, [OutAttribute] int[] @params)
		{
			Delegates.glGetInternalformativ(target, internalFormat, pname, bufSize, @params);
		}

		public static void GetInternalformati64v(TextureTarget target, PixelInternalFormat internalFormat, GetPName pname, int bufSize, [OutAttribute] long[] @params)
		{
			Delegates.glGetInternalformati64v(target, internalFormat, pname, bufSize, @params);
		}

		public static void GetMultisamplefv(GetMultisamplePName pname, uint index, [OutAttribute] float[] val)
		{
			Delegates.glGetMultisamplefv(pname, index, val);
		}

		public static void GetObjectLabel(ObjectLabel identifier, uint name, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder label)
		{
			Delegates.glGetObjectLabel(identifier, name, bufSize, length, label);
		}

		public static void GetObjectPtrLabel([OutAttribute] IntPtr ptr, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder label)
		{
			Delegates.glGetObjectPtrLabel(ptr, bufSize, length, label);
		}

		public static void GetPointerv(GetPointerParameter pname, [OutAttribute] IntPtr @params)
		{
			Delegates.glGetPointerv(pname, @params);
		}

		public static void GetProgramiv(uint program, ProgramParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetProgramiv(program, pname, @params);
		}

		public static void GetProgramBinary(uint program, int bufsize, [OutAttribute] int[] length, [OutAttribute] int[] binaryFormat, [OutAttribute] IntPtr binary)
		{
			Delegates.glGetProgramBinary(program, bufsize, length, binaryFormat, binary);
		}

		public static void GetProgramInfoLog(uint program, int maxLength, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog)
		{
			Delegates.glGetProgramInfoLog(program, maxLength, length, infoLog);
		}

		public static void GetProgramInterfaceiv(uint program, ProgramInterface programInterface, ProgramInterfaceParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetProgramInterfaceiv(program, programInterface, pname, @params);
		}

		public static void GetProgramPipelineiv(uint pipeline, int pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetProgramPipelineiv(pipeline, pname, @params);
		}

		public static void GetProgramPipelineInfoLog(uint pipeline, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog)
		{
			Delegates.glGetProgramPipelineInfoLog(pipeline, bufSize, length, infoLog);
		}

		public static void GetProgramResourceiv(uint program, ProgramInterface programInterface, uint index, int propCount, [OutAttribute] ProgramResourceParameterName[] props, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] @params)
		{
			Delegates.glGetProgramResourceiv(program, programInterface, index, propCount, props, bufSize, length, @params);
		}

		public static uint GetProgramResourceIndex(uint program, ProgramInterface programInterface, string name)
		{
			return Delegates.glGetProgramResourceIndex(program, programInterface, name);
		}

		public static int GetProgramResourceLocation(uint program, ProgramInterface programInterface, string name)
		{
			return Delegates.glGetProgramResourceLocation(program, programInterface, name);
		}

		public static int GetProgramResourceLocationIndex(uint program, ProgramInterface programInterface, string name)
		{
			return Delegates.glGetProgramResourceLocationIndex(program, programInterface, name);
		}

		public static void GetProgramResourceName(uint program, ProgramInterface programInterface, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetProgramResourceName(program, programInterface, index, bufSize, length, name);
		}

		public static void GetProgramStageiv(uint program, ShaderType shadertype, ProgramStageParameterName pname, [OutAttribute] int[] values)
		{
			Delegates.glGetProgramStageiv(program, shadertype, pname, values);
		}

		public static void GetQueryIndexediv(QueryTarget target, uint index, GetQueryParam pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetQueryIndexediv(target, index, pname, @params);
		}

		public static void GetQueryiv(QueryTarget target, GetQueryParam pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetQueryiv(target, pname, @params);
		}

		public static void GetQueryObjectiv(uint id, GetQueryObjectParam pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetQueryObjectiv(id, pname, @params);
		}

		public static void GetQueryObjectuiv(uint id, GetQueryObjectParam pname, [OutAttribute] uint[] @params)
		{
			Delegates.glGetQueryObjectuiv(id, pname, @params);
		}

		public static void GetQueryObjecti64v(uint id, GetQueryObjectParam pname, [OutAttribute] long[] @params)
		{
			Delegates.glGetQueryObjecti64v(id, pname, @params);
		}

		public static void GetQueryObjectui64v(uint id, GetQueryObjectParam pname, [OutAttribute] ulong[] @params)
		{
			Delegates.glGetQueryObjectui64v(id, pname, @params);
		}

		public static void GetRenderbufferParameteriv(RenderbufferTarget target, RenderbufferParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetRenderbufferParameteriv(target, pname, @params);
		}

		public static void GetNamedRenderbufferParameteriv(uint renderbuffer, RenderbufferParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetNamedRenderbufferParameteriv(renderbuffer, pname, @params);
		}

		public static void GetSamplerParameterfv(uint sampler, TextureParameterName pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetSamplerParameterfv(sampler, pname, @params);
		}

		public static void GetSamplerParameteriv(uint sampler, TextureParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetSamplerParameteriv(sampler, pname, @params);
		}

		public static void GetSamplerParameterIiv(uint sampler, TextureParameterName pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetSamplerParameterIiv(sampler, pname, @params);
		}

		public static void GetSamplerParameterIuiv(uint sampler, TextureParameterName pname, [OutAttribute] uint[] @params)
		{
			Delegates.glGetSamplerParameterIuiv(sampler, pname, @params);
		}

		public static void GetShaderiv(uint shader, ShaderParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetShaderiv(shader, pname, @params);
		}

		public static void GetShaderInfoLog(uint shader, int maxLength, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog)
		{
			Delegates.glGetShaderInfoLog(shader, maxLength, length, infoLog);
		}

		public static void GetShaderPrecisionFormat(ShaderType shaderType, int precisionType, [OutAttribute] int[] range, [OutAttribute] int[] precision)
		{
			Delegates.glGetShaderPrecisionFormat(shaderType, precisionType, range, precision);
		}

		public static void GetShaderSource(uint shader, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder source)
		{
			Delegates.glGetShaderSource(shader, bufSize, length, source);
		}

		public static string GetString(StringName name)
		{
			return Marshal.PtrToStringAnsi(Delegates.glGetString(name));
		}

		public static string GetStringi(StringName name, uint index)
		{
			return Marshal.PtrToStringAnsi(Delegates.glGetStringi(name, index));
		}

		public static uint GetSubroutineIndex(uint program, ShaderType shadertype, string name)
		{
			return Delegates.glGetSubroutineIndex(program, shadertype, name);
		}

		public static int GetSubroutineUniformLocation(uint program, ShaderType shadertype, string name)
		{
			return Delegates.glGetSubroutineUniformLocation(program, shadertype, name);
		}

		public static void GetSynciv(IntPtr sync, ArbSync pname, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] values)
		{
			Delegates.glGetSynciv(sync, pname, bufSize, length, values);
		}

		public static void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetTexImage(target, level, format, type, pixels);
		}

		public static void GetnTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetnTexImage(target, level, format, type, bufSize, pixels);
		}

		public static void GetTextureImage(uint texture, int level, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetTextureImage(texture, level, format, type, bufSize, pixels);
		}

		public static void GetTexLevelParameterfv(GetPName target, int level, GetTextureLevelParameter pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetTexLevelParameterfv(target, level, pname, @params);
		}

		public static void GetTexLevelParameteriv(GetPName target, int level, GetTextureLevelParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTexLevelParameteriv(target, level, pname, @params);
		}

		public static void GetTextureLevelParameterfv(uint texture, int level, GetTextureLevelParameter pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetTextureLevelParameterfv(texture, level, pname, @params);
		}

		public static void GetTextureLevelParameteriv(uint texture, int level, GetTextureLevelParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTextureLevelParameteriv(texture, level, pname, @params);
		}

		public static void GetTexParameterfv(TextureTarget target, GetTextureParameter pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetTexParameterfv(target, pname, @params);
		}

		public static void GetTexParameteriv(TextureTarget target, GetTextureParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTexParameteriv(target, pname, @params);
		}

		public static void GetTexParameterIiv(TextureTarget target, GetTextureParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTexParameterIiv(target, pname, @params);
		}

		public static void GetTexParameterIuiv(TextureTarget target, GetTextureParameter pname, [OutAttribute] uint[] @params)
		{
			Delegates.glGetTexParameterIuiv(target, pname, @params);
		}

		public static void GetTextureParameterfv(uint texture, GetTextureParameter pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetTextureParameterfv(texture, pname, @params);
		}

		public static void GetTextureParameteriv(uint texture, GetTextureParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTextureParameteriv(texture, pname, @params);
		}

		public static void GetTextureParameterIiv(uint texture, GetTextureParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetTextureParameterIiv(texture, pname, @params);
		}

		public static void GetTextureParameterIuiv(uint texture, GetTextureParameter pname, [OutAttribute] uint[] @params)
		{
			Delegates.glGetTextureParameterIuiv(texture, pname, @params);
		}

		public static void GetTextureSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels)
		{
			Delegates.glGetTextureSubImage(texture, level, xoffset, yoffset, zoffset, width, height, depth, format, type, bufSize, pixels);
		}

		public static void GetTransformFeedbackiv(uint xfb, TransformFeedbackParameterName pname, [OutAttribute] int[] param)
		{
			Delegates.glGetTransformFeedbackiv(xfb, pname, param);
		}

		public static void GetTransformFeedbacki_v(uint xfb, TransformFeedbackParameterName pname, uint index, [OutAttribute] int[] param)
		{
			Delegates.glGetTransformFeedbacki_v(xfb, pname, index, param);
		}

		public static void GetTransformFeedbacki64_v(uint xfb, TransformFeedbackParameterName pname, uint index, [OutAttribute] long[] param)
		{
			Delegates.glGetTransformFeedbacki64_v(xfb, pname, index, param);
		}

		public static void GetTransformFeedbackVarying(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveAttribType[] type, [OutAttribute] System.Text.StringBuilder name)
		{
			Delegates.glGetTransformFeedbackVarying(program, index, bufSize, length, size, type, name);
		}

		public static void GetUniformfv(uint program, int location, [OutAttribute] float[] @params)
		{
			Delegates.glGetUniformfv(program, location, @params);
		}

		public static void GetUniformiv(uint program, int location, [OutAttribute] int[] @params)
		{
			Delegates.glGetUniformiv(program, location, @params);
		}

		public static void GetUniformuiv(uint program, int location, [OutAttribute] uint[] @params)
		{
			Delegates.glGetUniformuiv(program, location, @params);
		}

		public static void GetUniformdv(uint program, int location, [OutAttribute] double[] @params)
		{
			Delegates.glGetUniformdv(program, location, @params);
		}

		public static void GetnUniformfv(uint program, int location, int bufSize, [OutAttribute] float[] @params)
		{
			Delegates.glGetnUniformfv(program, location, bufSize, @params);
		}

		public static void GetnUniformiv(uint program, int location, int bufSize, [OutAttribute] int[] @params)
		{
			Delegates.glGetnUniformiv(program, location, bufSize, @params);
		}

		public static void GetnUniformuiv(uint program, int location, int bufSize, [OutAttribute] uint[] @params)
		{
			Delegates.glGetnUniformuiv(program, location, bufSize, @params);
		}

		public static void GetnUniformdv(uint program, int location, int bufSize, [OutAttribute] double[] @params)
		{
			Delegates.glGetnUniformdv(program, location, bufSize, @params);
		}

		public static uint GetUniformBlockIndex(uint program, string uniformBlockName)
		{
			UseProgram(program);
			return Delegates.glGetUniformBlockIndex(program, uniformBlockName);
		}

		public static void GetUniformIndices(uint program, int uniformCount, string uniformNames, [OutAttribute] uint[] uniformIndices)
		{
			Delegates.glGetUniformIndices(program, uniformCount, uniformNames, uniformIndices);
		}

		public static int GetUniformLocation(uint program, string name)
		{
			return Delegates.glGetUniformLocation(program, name);
		}

		public static void GetUniformSubroutineuiv(ShaderType shadertype, int location, [OutAttribute] uint[] values)
		{
			Delegates.glGetUniformSubroutineuiv(shadertype, location, values);
		}

		public static void GetVertexArrayIndexed64iv(uint vaobj, uint index, VertexAttribParameter pname, [OutAttribute] long[] param)
		{
			Delegates.glGetVertexArrayIndexed64iv(vaobj, index, pname, param);
		}

		public static void GetVertexArrayIndexediv(uint vaobj, uint index, VertexAttribParameter pname, [OutAttribute] int[] param)
		{
			Delegates.glGetVertexArrayIndexediv(vaobj, index, pname, param);
		}

		public static void GetVertexArrayiv(uint vaobj, VertexAttribParameter pname, [OutAttribute] int[] param)
		{
			Delegates.glGetVertexArrayiv(vaobj, pname, param);
		}

		public static void GetVertexAttribdv(uint index, VertexAttribParameter pname, [OutAttribute] double[] @params)
		{
			Delegates.glGetVertexAttribdv(index, pname, @params);
		}

		public static void GetVertexAttribdv(int index, VertexAttribParameter pname, [OutAttribute] double[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribdv((uint)index, pname, @params);
		}

		public static void GetVertexAttribfv(uint index, VertexAttribParameter pname, [OutAttribute] float[] @params)
		{
			Delegates.glGetVertexAttribfv(index, pname, @params);
		}

		public static void GetVertexAttribfv(int index, VertexAttribParameter pname, [OutAttribute] float[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribfv((uint)index, pname, @params);
		}

		public static void GetVertexAttribiv(uint index, VertexAttribParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetVertexAttribiv(index, pname, @params);
		}

		public static void GetVertexAttribiv(int index, VertexAttribParameter pname, [OutAttribute] int[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribiv((uint)index, pname, @params);
		}

		public static void GetVertexAttribIiv(uint index, VertexAttribParameter pname, [OutAttribute] int[] @params)
		{
			Delegates.glGetVertexAttribIiv(index, pname, @params);
		}

		public static void GetVertexAttribIiv(int index, VertexAttribParameter pname, [OutAttribute] int[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribIiv((uint)index, pname, @params);
		}

		public static void GetVertexAttribIuiv(uint index, VertexAttribParameter pname, [OutAttribute] uint[] @params)
		{
			Delegates.glGetVertexAttribIuiv(index, pname, @params);
		}

		public static void GetVertexAttribIuiv(int index, VertexAttribParameter pname, [OutAttribute] uint[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribIuiv((uint)index, pname, @params);
		}

		public static void GetVertexAttribLdv(uint index, VertexAttribParameter pname, [OutAttribute] double[] @params)
		{
			Delegates.glGetVertexAttribLdv(index, pname, @params);
		}

		public static void GetVertexAttribLdv(int index, VertexAttribParameter pname, [OutAttribute] double[] @params)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribLdv((uint)index, pname, @params);
		}

		public static void GetVertexAttribPointerv(uint index, VertexAttribPointerParameter pname, [OutAttribute] IntPtr pointer)
		{
			Delegates.glGetVertexAttribPointerv(index, pname, pointer);
		}

		public static void GetVertexAttribPointerv(int index, VertexAttribPointerParameter pname, [OutAttribute] IntPtr pointer)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glGetVertexAttribPointerv((uint)index, pname, pointer);
		}

		public static void Hint(HintTarget target, HintMode mode)
		{
			Delegates.glHint(target, mode);
		}

		public static void InvalidateBufferData(uint buffer)
		{
			Delegates.glInvalidateBufferData(buffer);
		}

		public static void InvalidateBufferSubData(uint buffer, IntPtr offset, IntPtr length)
		{
			Delegates.glInvalidateBufferSubData(buffer, offset, length);
		}

		public static void InvalidateFramebuffer(FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments)
		{
			Delegates.glInvalidateFramebuffer(target, numAttachments, attachments);
		}

		public static void InvalidateNamedFramebufferData(uint framebuffer, int numAttachments, FramebufferAttachment[] attachments)
		{
			Delegates.glInvalidateNamedFramebufferData(framebuffer, numAttachments, attachments);
		}

		public static void InvalidateSubFramebuffer(FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments, int x, int y, int width, int height)
		{
			Delegates.glInvalidateSubFramebuffer(target, numAttachments, attachments, x, y, width, height);
		}

		public static void InvalidateNamedFramebufferSubData(uint framebuffer, int numAttachments, FramebufferAttachment[] attachments, int x, int y, int width, int height)
		{
			Delegates.glInvalidateNamedFramebufferSubData(framebuffer, numAttachments, attachments, x, y, width, height);
		}

		public static void InvalidateTexImage(uint texture, int level)
		{
			Delegates.glInvalidateTexImage(texture, level);
		}

		public static void InvalidateTexSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth)
		{
			Delegates.glInvalidateTexSubImage(texture, level, xoffset, yoffset, zoffset, width, height, depth);
		}

		public static bool IsBuffer(uint buffer)
		{
			return Delegates.glIsBuffer(buffer);
		}

		public static bool IsEnabled(EnableCap cap)
		{
			return Delegates.glIsEnabled(cap);
		}

		public static bool IsEnabledi(EnableCap cap, uint index)
		{
			return Delegates.glIsEnabledi(cap, index);
		}

		public static bool IsFramebuffer(uint framebuffer)
		{
			return Delegates.glIsFramebuffer(framebuffer);
		}

		public static bool IsProgram(uint program)
		{
			return Delegates.glIsProgram(program);
		}

		public static bool IsProgramPipeline(uint pipeline)
		{
			return Delegates.glIsProgramPipeline(pipeline);
		}

		public static bool IsQuery(uint id)
		{
			return Delegates.glIsQuery(id);
		}

		public static bool IsRenderbuffer(uint renderbuffer)
		{
			return Delegates.glIsRenderbuffer(renderbuffer);
		}

		public static bool IsSampler(uint id)
		{
			return Delegates.glIsSampler(id);
		}

		public static bool IsShader(uint shader)
		{
			return Delegates.glIsShader(shader);
		}

		public static bool IsSync(IntPtr sync)
		{
			return Delegates.glIsSync(sync);
		}

		public static bool IsTexture(uint texture)
		{
			return Delegates.glIsTexture(texture);
		}

		public static bool IsTransformFeedback(uint id)
		{
			return Delegates.glIsTransformFeedback(id);
		}

		public static bool IsVertexArray(uint array)
		{
			return Delegates.glIsVertexArray(array);
		}

		public static void LineWidth(float width)
		{
			Delegates.glLineWidth(width);
		}

		public static void LinkProgram(uint program)
		{
			Delegates.glLinkProgram(program);
		}

		public static void LogicOp(LogicOp opcode)
		{
			Delegates.glLogicOp(opcode);
		}

		public static IntPtr MapBuffer(BufferTarget target, BufferAccess access)
		{
			return Delegates.glMapBuffer(target, access);
		}

		public static IntPtr MapNamedBuffer(uint buffer, BufferAccess access)
		{
			return Delegates.glMapNamedBuffer(buffer, access);
		}

		public static IntPtr MapBufferRange(BufferTarget target, IntPtr offset, IntPtr length, BufferAccessMask access)
		{
			return Delegates.glMapBufferRange(target, offset, length, access);
		}

		public static IntPtr MapNamedBufferRange(uint buffer, IntPtr offset, int length, uint access)
		{
			return Delegates.glMapNamedBufferRange(buffer, offset, length, access);
		}

		public static void MemoryBarrier(uint barriers)
		{
			Delegates.glMemoryBarrier(barriers);
		}

		public static void MemoryBarrierByRegion(uint barriers)
		{
			Delegates.glMemoryBarrierByRegion(barriers);
		}

		public static void MinSampleShading(float value)
		{
			Delegates.glMinSampleShading(value);
		}

		public static void MultiDrawArrays(BeginMode mode, int[] first, int[] count, int drawcount)
		{
			Delegates.glMultiDrawArrays(mode, first, count, drawcount);
		}

		public static void MultiDrawArraysIndirect(BeginMode mode, IntPtr indirect, int drawcount, int stride)
		{
			Delegates.glMultiDrawArraysIndirect(mode, indirect, drawcount, stride);
		}

		public static void MultiDrawElements(BeginMode mode, int[] count, DrawElementsType type, IntPtr indices, int drawcount)
		{
			Delegates.glMultiDrawElements(mode, count, type, indices, drawcount);
		}

		public static void MultiDrawElementsBaseVertex(BeginMode mode, int[] count, DrawElementsType type, IntPtr indices, int drawcount, int[] basevertex)
		{
			Delegates.glMultiDrawElementsBaseVertex(mode, count, type, indices, drawcount, basevertex);
		}

		public static void MultiDrawElementsIndirect(BeginMode mode, DrawElementsType type, IntPtr indirect, int drawcount, int stride)
		{
			Delegates.glMultiDrawElementsIndirect(mode, type, indirect, drawcount, stride);
		}

		public static void ObjectLabel(ObjectLabel identifier, uint name, int length, string label)
		{
			Delegates.glObjectLabel(identifier, name, length, label);
		}

		public static void ObjectPtrLabel(IntPtr ptr, int length, string label)
		{
			Delegates.glObjectPtrLabel(ptr, length, label);
		}

		public static void PatchParameteri(int pname, int value)
		{
			Delegates.glPatchParameteri(pname, value);
		}

		public static void PatchParameterfv(int pname, float[] values)
		{
			Delegates.glPatchParameterfv(pname, values);
		}

		public static void PixelStoref(PixelStoreParameter pname, float param)
		{
			Delegates.glPixelStoref(pname, param);
		}

		public static void PixelStorei(PixelStoreParameter pname, int param)
		{
			Delegates.glPixelStorei(pname, param);
		}

		public static void PointParameterf(PointParameterName pname, float param)
		{
			Delegates.glPointParameterf(pname, param);
		}

		public static void PointParameteri(PointParameterName pname, int param)
		{
			Delegates.glPointParameteri(pname, param);
		}

		public static void PointParameterfv(PointParameterName pname, float[] @params)
		{
			Delegates.glPointParameterfv(pname, @params);
		}

		public static void PointParameteriv(PointParameterName pname, int[] @params)
		{
			Delegates.glPointParameteriv(pname, @params);
		}

		public static void PointSize(float size)
		{
			Delegates.glPointSize(size);
		}

		public static void PolygonMode(MaterialFace face, PolygonMode mode)
		{
			Delegates.glPolygonMode(face, mode);
		}

		public static void PolygonOffset(float factor, float units)
		{
			Delegates.glPolygonOffset(factor, units);
		}

		public static void PrimitiveRestartIndex(uint index)
		{
			Delegates.glPrimitiveRestartIndex(index);
		}

		public static void ProgramBinary(uint program, int binaryFormat, IntPtr binary, int length)
		{
			Delegates.glProgramBinary(program, binaryFormat, binary, length);
		}

		public static void ProgramParameteri(uint program, ProgramParameterPName pname, int value)
		{
			Delegates.glProgramParameteri(program, pname, value);
		}

		public static void ProgramUniform1f(uint program, int location, float v0)
		{
			Delegates.glProgramUniform1f(program, location, v0);
		}

		public static void ProgramUniform2f(uint program, int location, float v0, float v1)
		{
			Delegates.glProgramUniform2f(program, location, v0, v1);
		}

		public static void ProgramUniform3f(uint program, int location, float v0, float v1, float v2)
		{
			Delegates.glProgramUniform3f(program, location, v0, v1, v2);
		}

		public static void ProgramUniform4f(uint program, int location, float v0, float v1, float v2, float v3)
		{
			Delegates.glProgramUniform4f(program, location, v0, v1, v2, v3);
		}

		public static void ProgramUniform1i(uint program, int location, int v0)
		{
			Delegates.glProgramUniform1i(program, location, v0);
		}

		public static void ProgramUniform2i(uint program, int location, int v0, int v1)
		{
			Delegates.glProgramUniform2i(program, location, v0, v1);
		}

		public static void ProgramUniform3i(uint program, int location, int v0, int v1, int v2)
		{
			Delegates.glProgramUniform3i(program, location, v0, v1, v2);
		}

		public static void ProgramUniform4i(uint program, int location, int v0, int v1, int v2, int v3)
		{
			Delegates.glProgramUniform4i(program, location, v0, v1, v2, v3);
		}

		public static void ProgramUniform1ui(uint program, int location, uint v0)
		{
			Delegates.glProgramUniform1ui(program, location, v0);
		}

		public static void ProgramUniform2ui(uint program, int location, int v0, uint v1)
		{
			Delegates.glProgramUniform2ui(program, location, v0, v1);
		}

		public static void ProgramUniform3ui(uint program, int location, int v0, int v1, uint v2)
		{
			Delegates.glProgramUniform3ui(program, location, v0, v1, v2);
		}

		public static void ProgramUniform4ui(uint program, int location, int v0, int v1, int v2, uint v3)
		{
			Delegates.glProgramUniform4ui(program, location, v0, v1, v2, v3);
		}

		public static void ProgramUniform1fv(uint program, int location, int count, float[] value)
		{
			Delegates.glProgramUniform1fv(program, location, count, value);
		}

		public static void ProgramUniform2fv(uint program, int location, int count, float[] value)
		{
			Delegates.glProgramUniform2fv(program, location, count, value);
		}

		public static void ProgramUniform3fv(uint program, int location, int count, float[] value)
		{
			Delegates.glProgramUniform3fv(program, location, count, value);
		}

		public static void ProgramUniform4fv(uint program, int location, int count, float[] value)
		{
			Delegates.glProgramUniform4fv(program, location, count, value);
		}

		public static void ProgramUniform1iv(uint program, int location, int count, int[] value)
		{
			Delegates.glProgramUniform1iv(program, location, count, value);
		}

		public static void ProgramUniform2iv(uint program, int location, int count, int[] value)
		{
			Delegates.glProgramUniform2iv(program, location, count, value);
		}

		public static void ProgramUniform3iv(uint program, int location, int count, int[] value)
		{
			Delegates.glProgramUniform3iv(program, location, count, value);
		}

		public static void ProgramUniform4iv(uint program, int location, int count, int[] value)
		{
			Delegates.glProgramUniform4iv(program, location, count, value);
		}

		public static void ProgramUniform1uiv(uint program, int location, int count, uint[] value)
		{
			Delegates.glProgramUniform1uiv(program, location, count, value);
		}

		public static void ProgramUniform2uiv(uint program, int location, int count, uint[] value)
		{
			Delegates.glProgramUniform2uiv(program, location, count, value);
		}

		public static void ProgramUniform3uiv(uint program, int location, int count, uint[] value)
		{
			Delegates.glProgramUniform3uiv(program, location, count, value);
		}

		public static void ProgramUniform4uiv(uint program, int location, int count, uint[] value)
		{
			Delegates.glProgramUniform4uiv(program, location, count, value);
		}

		public static void ProgramUniformMatrix2fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix2fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix3fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix3fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix4fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix4fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix2x3fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix2x3fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix3x2fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix3x2fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix2x4fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix2x4fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix4x2fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix4x2fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix3x4fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix3x4fv(program, location, count, transpose, value);
		}

		public static void ProgramUniformMatrix4x3fv(uint program, int location, int count, bool transpose, float[] value)
		{
			Delegates.glProgramUniformMatrix4x3fv(program, location, count, transpose, value);
		}

		public static void ProvokingVertex(ProvokingVertexMode provokeMode)
		{
			Delegates.glProvokingVertex(provokeMode);
		}

		public static void QueryCounter(uint id, QueryTarget target)
		{
			Delegates.glQueryCounter(id, target);
		}

		public static void ReadBuffer(ReadBufferMode mode)
		{
			Delegates.glReadBuffer(mode);
		}

		public static void NamedFramebufferReadBuffer(ReadBufferMode framebuffer, BeginMode mode)
		{
			Delegates.glNamedFramebufferReadBuffer(framebuffer, mode);
		}

		public static void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, int[] data)
		{
			Delegates.glReadPixels(x, y, width, height, format, type, data);
		}

		public static void ReadnPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, int bufSize, int[] data)
		{
			Delegates.glReadnPixels(x, y, width, height, format, type, bufSize, data);
		}

		public static void RenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalFormat, int width, int height)
		{
			Delegates.glRenderbufferStorage(target, internalFormat, width, height);
		}

		public static void NamedRenderbufferStorage(uint renderbuffer, RenderbufferStorage internalFormat, int width, int height)
		{
			Delegates.glNamedRenderbufferStorage(renderbuffer, internalFormat, width, height);
		}

		public static void RenderbufferStorageMultisample(RenderbufferTarget target, int samples, RenderbufferStorage internalFormat, int width, int height)
		{
			Delegates.glRenderbufferStorageMultisample(target, samples, internalFormat, width, height);
		}

		public static void NamedRenderbufferStorageMultisample(uint renderbuffer, int samples, RenderbufferStorage internalFormat, int width, int height)
		{
			Delegates.glNamedRenderbufferStorageMultisample(renderbuffer, samples, internalFormat, width, height);
		}

		public static void SampleCoverage(float value, bool invert)
		{
			Delegates.glSampleCoverage(value, invert);
		}

		public static void SampleMaski(uint maskNumber, uint mask)
		{
			Delegates.glSampleMaski(maskNumber, mask);
		}

		public static void SamplerParameterf(uint sampler, TextureParameterName pname, float param)
		{
			Delegates.glSamplerParameterf(sampler, pname, param);
		}

		public static void SamplerParameteri(uint sampler, TextureParameterName pname, int param)
		{
			Delegates.glSamplerParameteri(sampler, pname, param);
		}

		public static void SamplerParameterfv(uint sampler, TextureParameterName pname, float[] @params)
		{
			Delegates.glSamplerParameterfv(sampler, pname, @params);
		}

		public static void SamplerParameteriv(uint sampler, TextureParameterName pname, int[] @params)
		{
			Delegates.glSamplerParameteriv(sampler, pname, @params);
		}

		public static void SamplerParameterIiv(uint sampler, TextureParameterName pname, int[] @params)
		{
			Delegates.glSamplerParameterIiv(sampler, pname, @params);
		}

		public static void SamplerParameterIuiv(uint sampler, TextureParameterName pname, uint[] @params)
		{
			Delegates.glSamplerParameterIuiv(sampler, pname, @params);
		}

		public static void Scissor(int x, int y, int width, int height)
		{
			Delegates.glScissor(x, y, width, height);
		}

		public static void ScissorArrayv(uint first, int count, int[] v)
		{
			Delegates.glScissorArrayv(first, count, v);
		}

		public static void ScissorIndexed(uint index, int left, int bottom, int width, int height)
		{
			Delegates.glScissorIndexed(index, left, bottom, width, height);
		}

		public static void ScissorIndexedv(uint index, int[] v)
		{
			Delegates.glScissorIndexedv(index, v);
		}

		public static void ShaderBinary(int count, uint[] shaders, int binaryFormat, IntPtr binary, int length)
		{
			Delegates.glShaderBinary(count, shaders, binaryFormat, binary, length);
		}

		public static void ShaderSource(uint shader, int count, string[] @string, int[] length)
		{
			Delegates.glShaderSource(shader, count, @string, length);
		}

		public static void ShaderStorageBlockBinding(uint program, uint storageBlockIndex, uint storageBlockBinding)
		{
			Delegates.glShaderStorageBlockBinding(program, storageBlockIndex, storageBlockBinding);
		}

		public static void StencilFunc(StencilFunction func, int @ref, uint mask)
		{
			Delegates.glStencilFunc(func, @ref, mask);
		}

		public static void StencilFuncSeparate(StencilFace face, StencilFunction func, int @ref, uint mask)
		{
			Delegates.glStencilFuncSeparate(face, func, @ref, mask);
		}

		public static void StencilMask(uint mask)
		{
			Delegates.glStencilMask(mask);
		}

		public static void StencilMaskSeparate(StencilFace face, uint mask)
		{
			Delegates.glStencilMaskSeparate(face, mask);
		}

		public static void StencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass)
		{
			Delegates.glStencilOp(sfail, dpfail, dppass);
		}

		public static void StencilOpSeparate(StencilFace face, StencilOp sfail, StencilOp dpfail, StencilOp dppass)
		{
			Delegates.glStencilOpSeparate(face, sfail, dpfail, dppass);
		}

		public static void TexBuffer(TextureBufferTarget target, SizedInternalFormat internalFormat, uint buffer)
		{
			Delegates.glTexBuffer(target, internalFormat, buffer);
		}

		public static void TextureBuffer(uint texture, SizedInternalFormat internalFormat, uint buffer)
		{
			Delegates.glTextureBuffer(texture, internalFormat, buffer);
		}

		public static void TexBufferRange(BufferTarget target, SizedInternalFormat internalFormat, uint buffer, IntPtr offset, IntPtr size)
		{
			Delegates.glTexBufferRange(target, internalFormat, buffer, offset, size);
		}

		public static void TextureBufferRange(uint texture, SizedInternalFormat internalFormat, uint buffer, IntPtr offset, int size)
		{
			Delegates.glTextureBufferRange(texture, internalFormat, buffer, offset, size);
		}

		public static void TexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int border, PixelFormat format, PixelType type, IntPtr data)
		{
			Delegates.glTexImage1D(target, level, internalFormat, width, border, format, type, data);
		}

		public static void TexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr data)
		{
			Delegates.glTexImage2D(target, level, (int) internalFormat, width, height, border, (int) format, type, data);
		}

//		public static void TexImage2D(TextureTarget target, int level, PixelFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType type, IntPtr data)
//		{
//			Delegates.glTexImage2D(target, level, (int) internalFormat, width, height, border, (int) format, type, data);
//		}

		public static void TexImage2DMultisample(TextureTargetMultisample target, int samples, PixelInternalFormat internalFormat, int width, int height, bool fixedsamplelocations)
		{
			Delegates.glTexImage2DMultisample(target, samples, internalFormat, width, height, fixedsamplelocations);
		}

		public static void TexImage3D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int depth, int border, PixelFormat format, PixelType type, IntPtr data)
		{
			Delegates.glTexImage3D(target, level, internalFormat, width, height, depth, border, format, type, data);
		}

		public static void TexImage3DMultisample(TextureTargetMultisample target, int samples, PixelInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations)
		{
			Delegates.glTexImage3DMultisample(target, samples, internalFormat, width, height, depth, fixedsamplelocations);
		}

		public static void TexParameterf(TextureTarget target, TextureParameterName pname, float param)
		{
			Delegates.glTexParameterf(target, pname, param);
		}

		public static void TexParameteri(TextureTarget target, TextureParameterName pname, int param)
		{
			Delegates.glTexParameteri(target, pname, param);
		}

		public static void TextureParameterf(uint texture, TextureParameter pname, float param)
		{
			Delegates.glTextureParameterf(texture, pname, param);
		}

		public static void TextureParameteri(uint texture, TextureParameter pname, int param)
		{
			Delegates.glTextureParameteri(texture, pname, param);
		}

		public static void TexParameterfv(TextureTarget target, TextureParameterName pname, float[] @params)
		{
			Delegates.glTexParameterfv(target, pname, @params);
		}

		public static void TexParameteriv(TextureTarget target, TextureParameterName pname, int[] @params)
		{
			Delegates.glTexParameteriv(target, pname, @params);
		}

		public static void TexParameterIiv(TextureTarget target, TextureParameterName pname, int[] @params)
		{
			Delegates.glTexParameterIiv(target, pname, @params);
		}

		public static void TexParameterIuiv(TextureTarget target, TextureParameterName pname, uint[] @params)
		{
			Delegates.glTexParameterIuiv(target, pname, @params);
		}

		public static void TextureParameterfv(uint texture, TextureParameter pname, float[] paramtexture)
		{
			Delegates.glTextureParameterfv(texture, pname, paramtexture);
		}

		public static void TextureParameteriv(uint texture, TextureParameter pname, int[] param)
		{
			Delegates.glTextureParameteriv(texture, pname, param);
		}

		public static void TextureParameterIiv(uint texture, TextureParameter pname, int[] @params)
		{
			Delegates.glTextureParameterIiv(texture, pname, @params);
		}

		public static void TextureParameterIuiv(uint texture, TextureParameter pname, uint[] @params)
		{
			Delegates.glTextureParameterIuiv(texture, pname, @params);
		}

		public static void TexStorage1D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width)
		{
			Delegates.glTexStorage1D(target, levels, internalFormat, width);
		}

		public static void TextureStorage1D(uint texture, int levels, SizedInternalFormat internalFormat, int width)
		{
			Delegates.glTextureStorage1D(texture, levels, internalFormat, width);
		}

		public static void TexStorage2D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width, int height)
		{
			Delegates.glTexStorage2D(target, levels, internalFormat, width, height);
		}

		public static void TextureStorage2D(uint texture, int levels, SizedInternalFormat internalFormat, int width, int height)
		{
			Delegates.glTextureStorage2D(texture, levels, internalFormat, width, height);
		}

		public static void TexStorage2DMultisample(TextureTarget target, int samples, SizedInternalFormat internalFormat, int width, int height, bool fixedsamplelocations)
		{
			Delegates.glTexStorage2DMultisample(target, samples, internalFormat, width, height, fixedsamplelocations);
		}

		public static void TextureStorage2DMultisample(uint texture, int samples, SizedInternalFormat internalFormat, int width, int height, bool fixedsamplelocations)
		{
			Delegates.glTextureStorage2DMultisample(texture, samples, internalFormat, width, height, fixedsamplelocations);
		}

		public static void TexStorage3D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width, int height, int depth)
		{
			Delegates.glTexStorage3D(target, levels, internalFormat, width, height, depth);
		}

		public static void TextureStorage3D(uint texture, int levels, SizedInternalFormat internalFormat, int width, int height, int depth)
		{
			Delegates.glTextureStorage3D(texture, levels, internalFormat, width, height, depth);
		}

		public static void TexStorage3DMultisample(TextureTarget target, int samples, SizedInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations)
		{
			Delegates.glTexStorage3DMultisample(target, samples, internalFormat, width, height, depth, fixedsamplelocations);
		}

		public static void TextureStorage3DMultisample(uint texture, int samples, SizedInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations)
		{
			Delegates.glTextureStorage3DMultisample(texture, samples, internalFormat, width, height, depth, fixedsamplelocations);
		}

		public static void TexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTexSubImage1D(target, level, xoffset, width, format, type, pixels);
		}

		public static void TextureSubImage1D(uint texture, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTextureSubImage1D(texture, level, xoffset, width, format, type, pixels);
		}

		public static void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
		}

		public static void TextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTextureSubImage2D(texture, level, xoffset, yoffset, width, height, format, type, pixels);
		}

		public static void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTexSubImage3D(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);
		}

		public static void TextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr pixels)
		{
			Delegates.glTextureSubImage3D(texture, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);
		}

		public static void TextureBarrier()
		{
			Delegates.glTextureBarrier();
		}

		public static void TextureView(uint texture, TextureTarget target, uint origtexture, PixelInternalFormat internalFormat, uint minlevel, uint numlevels, uint minlayer, uint numlayers)
		{
			Delegates.glTextureView(texture, target, origtexture, internalFormat, minlevel, numlevels, minlayer, numlayers);
		}

		public static void TransformFeedbackBufferBase(uint xfb, uint index, uint buffer)
		{
			Delegates.glTransformFeedbackBufferBase(xfb, index, buffer);
		}

		public static void TransformFeedbackBufferRange(uint xfb, uint index, uint buffer, IntPtr offset, int size)
		{
			Delegates.glTransformFeedbackBufferRange(xfb, index, buffer, offset, size);
		}

		public static void TransformFeedbackVaryings(uint program, int count, string[] varyings, TransformFeedbackMode bufferMode)
		{
			Delegates.glTransformFeedbackVaryings(program, count, varyings, bufferMode);
		}

		public static void Uniform1f(int location, float v0)
		{
			Delegates.glUniform1f(location, v0);
		}

		public static void Uniform2f(int location, float v0, float v1)
		{
			Delegates.glUniform2f(location, v0, v1);
		}

		public static void Uniform3f(int location, float v0, float v1, float v2)
		{
			Delegates.glUniform3f(location, v0, v1, v2);
		}

		public static void Uniform4f(int location, float v0, float v1, float v2, float v3)
		{
			Delegates.glUniform4f(location, v0, v1, v2, v3);
		}

		public static void Uniform1i(int location, int v0)
		{
			Delegates.glUniform1i(location, v0);
		}

		public static void Uniform2i(int location, int v0, int v1)
		{
			Delegates.glUniform2i(location, v0, v1);
		}

		public static void Uniform3i(int location, int v0, int v1, int v2)
		{
			Delegates.glUniform3i(location, v0, v1, v2);
		}

		public static void Uniform4i(int location, int v0, int v1, int v2, int v3)
		{
			Delegates.glUniform4i(location, v0, v1, v2, v3);
		}

		public static void Uniform1ui(int location, uint v0)
		{
			Delegates.glUniform1ui(location, v0);
		}

		public static void Uniform2ui(int location, uint v0, uint v1)
		{
			Delegates.glUniform2ui(location, v0, v1);
		}

		public static void Uniform3ui(int location, uint v0, uint v1, uint v2)
		{
			Delegates.glUniform3ui(location, v0, v1, v2);
		}

		public static void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3)
		{
			Delegates.glUniform4ui(location, v0, v1, v2, v3);
		}

		public static void Uniform1fv(int location, int count, float[] value)
		{
			Delegates.glUniform1fv(location, count, value);
		}

		public static void Uniform2fv(int location, int count, float[] value)
		{
			Delegates.glUniform2fv(location, count, value);
		}

		public static void Uniform3fv(int location, int count, float[] value)
		{
			Delegates.glUniform3fv(location, count, value);
		}

		public static void Uniform4fv(int location, int count, float[] value)
		{
			Delegates.glUniform4fv(location, count, value);
		}

		public static void Uniform1iv(int location, int count, int[] value)
		{
			Delegates.glUniform1iv(location, count, value);
		}

		public static void Uniform2iv(int location, int count, int[] value)
		{
			Delegates.glUniform2iv(location, count, value);
		}

		public static void Uniform3iv(int location, int count, int[] value)
		{
			Delegates.glUniform3iv(location, count, value);
		}

		public static void Uniform4iv(int location, int count, int[] value)
		{
			Delegates.glUniform4iv(location, count, value);
		}

		public static void Uniform1uiv(int location, int count, uint[] value)
		{
			Delegates.glUniform1uiv(location, count, value);
		}

		public static void Uniform2uiv(int location, int count, uint[] value)
		{
			Delegates.glUniform2uiv(location, count, value);
		}

		public static void Uniform3uiv(int location, int count, uint[] value)
		{
			Delegates.glUniform3uiv(location, count, value);
		}

		public static void Uniform4uiv(int location, int count, uint[] value)
		{
			Delegates.glUniform4uiv(location, count, value);
		}

		public static void UniformMatrix2fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix2fv(location, count, transpose, value);
		}

		public static void UniformMatrix3fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix3fv(location, count, transpose, value);
		}

		public static void UniformMatrix4fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix4fv(location, count, transpose, value);
		}

		public static void UniformMatrix2x3fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix2x3fv(location, count, transpose, value);
		}

		public static void UniformMatrix3x2fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix3x2fv(location, count, transpose, value);
		}

		public static void UniformMatrix2x4fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix2x4fv(location, count, transpose, value);
		}

		public static void UniformMatrix4x2fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix4x2fv(location, count, transpose, value);
		}

		public static void UniformMatrix3x4fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix3x4fv(location, count, transpose, value);
		}

		public static void UniformMatrix4x3fv(int location, int count, bool transpose, float[] value)
		{
			Delegates.glUniformMatrix4x3fv(location, count, transpose, value);
		}

		public static void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding)
		{
			Delegates.glUniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
		}

		public static void UniformSubroutinesuiv(ShaderType shadertype, int count, uint[] indices)
		{
			Delegates.glUniformSubroutinesuiv(shadertype, count, indices);
		}

		public static bool UnmapBuffer(BufferTarget target)
		{
			return Delegates.glUnmapBuffer(target);
		}

		public static bool UnmapNamedBuffer(uint buffer)
		{
			return Delegates.glUnmapNamedBuffer(buffer);
		}

		public static void UseProgram(uint program)
		{
			currentProgram = program;
			Delegates.glUseProgram(program);
		}

		public static void UseProgramStages(uint pipeline, uint stages, uint program)
		{
			currentProgram = program;
			Delegates.glUseProgramStages(pipeline, stages, program);
		}

		public static void ValidateProgram(uint program)
		{
			Delegates.glValidateProgram(program);
		}

		public static void ValidateProgramPipeline(uint pipeline)
		{
			Delegates.glValidateProgramPipeline(pipeline);
		}

		public static void VertexArrayElementBuffer(uint vaobj, uint buffer)
		{
			Delegates.glVertexArrayElementBuffer(vaobj, buffer);
		}

		public static void VertexAttrib1f(uint index, float v0)
		{
			Delegates.glVertexAttrib1f(index, v0);
		}

		public static void VertexAttrib1f(int index, float v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1f((uint)index, v0);
		}

		public static void VertexAttrib1s(uint index, short v0)
		{
			Delegates.glVertexAttrib1s(index, v0);
		}

		public static void VertexAttrib1s(int index, short v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1s((uint)index, v0);
		}

		public static void VertexAttrib1d(uint index, double v0)
		{
			Delegates.glVertexAttrib1d(index, v0);
		}

		public static void VertexAttrib1d(int index, double v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1d((uint)index, v0);
		}

		public static void VertexAttribI1i(uint index, int v0)
		{
			Delegates.glVertexAttribI1i(index, v0);
		}

		public static void VertexAttribI1i(int index, int v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI1i((uint)index, v0);
		}

		public static void VertexAttribI1ui(uint index, uint v0)
		{
			Delegates.glVertexAttribI1ui(index, v0);
		}

		public static void VertexAttribI1ui(int index, uint v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI1ui((uint)index, v0);
		}

		public static void VertexAttrib2f(uint index, float v0, float v1)
		{
			Delegates.glVertexAttrib2f(index, v0, v1);
		}

		public static void VertexAttrib2f(int index, float v0, float v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2f((uint)index, v0, v1);
		}

		public static void VertexAttrib2s(uint index, short v0, short v1)
		{
			Delegates.glVertexAttrib2s(index, v0, v1);
		}

		public static void VertexAttrib2s(int index, short v0, short v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2s((uint)index, v0, v1);
		}

		public static void VertexAttrib2d(uint index, double v0, double v1)
		{
			Delegates.glVertexAttrib2d(index, v0, v1);
		}

		public static void VertexAttrib2d(int index, double v0, double v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2d((uint)index, v0, v1);
		}

		public static void VertexAttribI2i(uint index, int v0, int v1)
		{
			Delegates.glVertexAttribI2i(index, v0, v1);
		}

		public static void VertexAttribI2i(int index, int v0, int v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI2i((uint)index, v0, v1);
		}

		public static void VertexAttribI2ui(uint index, uint v0, uint v1)
		{
			Delegates.glVertexAttribI2ui(index, v0, v1);
		}

		public static void VertexAttribI2ui(int index, uint v0, uint v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI2ui((uint)index, v0, v1);
		}

		public static void VertexAttrib3f(uint index, float v0, float v1, float v2)
		{
			Delegates.glVertexAttrib3f(index, v0, v1, v2);
		}

		public static void VertexAttrib3f(int index, float v0, float v1, float v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3f((uint)index, v0, v1, v2);
		}

		public static void VertexAttrib3s(uint index, short v0, short v1, short v2)
		{
			Delegates.glVertexAttrib3s(index, v0, v1, v2);
		}

		public static void VertexAttrib3s(int index, short v0, short v1, short v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3s((uint)index, v0, v1, v2);
		}

		public static void VertexAttrib3d(uint index, double v0, double v1, double v2)
		{
			Delegates.glVertexAttrib3d(index, v0, v1, v2);
		}

		public static void VertexAttrib3d(int index, double v0, double v1, double v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3d((uint)index, v0, v1, v2);
		}

		public static void VertexAttribI3i(uint index, int v0, int v1, int v2)
		{
			Delegates.glVertexAttribI3i(index, v0, v1, v2);
		}

		public static void VertexAttribI3i(int index, int v0, int v1, int v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI3i((uint)index, v0, v1, v2);
		}

		public static void VertexAttribI3ui(uint index, uint v0, uint v1, uint v2)
		{
			Delegates.glVertexAttribI3ui(index, v0, v1, v2);
		}

		public static void VertexAttribI3ui(int index, uint v0, uint v1, uint v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI3ui((uint)index, v0, v1, v2);
		}

		public static void VertexAttrib4f(uint index, float v0, float v1, float v2, float v3)
		{
			Delegates.glVertexAttrib4f(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4f(int index, float v0, float v1, float v2, float v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4f((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4s(uint index, short v0, short v1, short v2, short v3)
		{
			Delegates.glVertexAttrib4s(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4s(int index, short v0, short v1, short v2, short v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4s((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4d(uint index, double v0, double v1, double v2, double v3)
		{
			Delegates.glVertexAttrib4d(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4d(int index, double v0, double v1, double v2, double v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4d((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4Nub(uint index, byte v0, byte v1, byte v2, byte v3)
		{
			Delegates.glVertexAttrib4Nub(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4Nub(int index, byte v0, byte v1, byte v2, byte v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nub((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4i(uint index, int v0, int v1, int v2, int v3)
		{
			Delegates.glVertexAttribI4i(index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4i(int index, int v0, int v1, int v2, int v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4i((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4ui(uint index, uint v0, uint v1, uint v2, uint v3)
		{
			Delegates.glVertexAttribI4ui(index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4ui(int index, uint v0, uint v1, uint v2, uint v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4ui((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttribL1d(uint index, double v0)
		{
			Delegates.glVertexAttribL1d(index, v0);
		}

		public static void VertexAttribL1d(int index, double v0)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL1d((uint)index, v0);
		}

		public static void VertexAttribL2d(uint index, double v0, double v1)
		{
			Delegates.glVertexAttribL2d(index, v0, v1);
		}

		public static void VertexAttribL2d(int index, double v0, double v1)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL2d((uint)index, v0, v1);
		}

		public static void VertexAttribL3d(uint index, double v0, double v1, double v2)
		{
			Delegates.glVertexAttribL3d(index, v0, v1, v2);
		}

		public static void VertexAttribL3d(int index, double v0, double v1, double v2)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL3d((uint)index, v0, v1, v2);
		}

		public static void VertexAttribL4d(uint index, double v0, double v1, double v2, double v3)
		{
			Delegates.glVertexAttribL4d(index, v0, v1, v2, v3);
		}

		public static void VertexAttribL4d(int index, double v0, double v1, double v2, double v3)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL4d((uint)index, v0, v1, v2, v3);
		}

		public static void VertexAttrib1fv(uint index, float[] v)
		{
			Delegates.glVertexAttrib1fv(index, v);
		}

		public static void VertexAttrib1fv(int index, float[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1fv((uint)index, v);
		}

		public static void VertexAttrib1sv(uint index, short[] v)
		{
			Delegates.glVertexAttrib1sv(index, v);
		}

		public static void VertexAttrib1sv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1sv((uint)index, v);
		}

		public static void VertexAttrib1dv(uint index, double[] v)
		{
			Delegates.glVertexAttrib1dv(index, v);
		}

		public static void VertexAttrib1dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib1dv((uint)index, v);
		}

		public static void VertexAttribI1iv(uint index, int[] v)
		{
			Delegates.glVertexAttribI1iv(index, v);
		}

		public static void VertexAttribI1iv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI1iv((uint)index, v);
		}

		public static void VertexAttribI1uiv(uint index, uint[] v)
		{
			Delegates.glVertexAttribI1uiv(index, v);
		}

		public static void VertexAttribI1uiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI1uiv((uint)index, v);
		}

		public static void VertexAttrib2fv(uint index, float[] v)
		{
			Delegates.glVertexAttrib2fv(index, v);
		}

		public static void VertexAttrib2fv(int index, float[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2fv((uint)index, v);
		}

		public static void VertexAttrib2sv(uint index, short[] v)
		{
			Delegates.glVertexAttrib2sv(index, v);
		}

		public static void VertexAttrib2sv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2sv((uint)index, v);
		}

		public static void VertexAttrib2dv(uint index, double[] v)
		{
			Delegates.glVertexAttrib2dv(index, v);
		}

		public static void VertexAttrib2dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib2dv((uint)index, v);
		}

		public static void VertexAttribI2iv(uint index, int[] v)
		{
			Delegates.glVertexAttribI2iv(index, v);
		}

		public static void VertexAttribI2iv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI2iv((uint)index, v);
		}

		public static void VertexAttribI2uiv(uint index, uint[] v)
		{
			Delegates.glVertexAttribI2uiv(index, v);
		}

		public static void VertexAttribI2uiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI2uiv((uint)index, v);
		}

		public static void VertexAttrib3fv(uint index, float[] v)
		{
			Delegates.glVertexAttrib3fv(index, v);
		}

		public static void VertexAttrib3fv(int index, float[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3fv((uint)index, v);
		}

		public static void VertexAttrib3sv(uint index, short[] v)
		{
			Delegates.glVertexAttrib3sv(index, v);
		}

		public static void VertexAttrib3sv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3sv((uint)index, v);
		}

		public static void VertexAttrib3dv(uint index, double[] v)
		{
			Delegates.glVertexAttrib3dv(index, v);
		}

		public static void VertexAttrib3dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib3dv((uint)index, v);
		}

		public static void VertexAttribI3iv(uint index, int[] v)
		{
			Delegates.glVertexAttribI3iv(index, v);
		}

		public static void VertexAttribI3iv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI3iv((uint)index, v);
		}

		public static void VertexAttribI3uiv(uint index, uint[] v)
		{
			Delegates.glVertexAttribI3uiv(index, v);
		}

		public static void VertexAttribI3uiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI3uiv((uint)index, v);
		}

		public static void VertexAttrib4fv(uint index, float[] v)
		{
			Delegates.glVertexAttrib4fv(index, v);
		}

		public static void VertexAttrib4fv(int index, float[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4fv((uint)index, v);
		}

		public static void VertexAttrib4sv(uint index, short[] v)
		{
			Delegates.glVertexAttrib4sv(index, v);
		}

		public static void VertexAttrib4sv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4sv((uint)index, v);
		}

		public static void VertexAttrib4dv(uint index, double[] v)
		{
			Delegates.glVertexAttrib4dv(index, v);
		}

		public static void VertexAttrib4dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4dv((uint)index, v);
		}

		public static void VertexAttrib4iv(uint index, int[] v)
		{
			Delegates.glVertexAttrib4iv(index, v);
		}

		public static void VertexAttrib4iv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4iv((uint)index, v);
		}

		public static void VertexAttrib4bv(uint index, sbyte[] v)
		{
			Delegates.glVertexAttrib4bv(index, v);
		}

		public static void VertexAttrib4bv(int index, sbyte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4bv((uint)index, v);
		}

		public static void VertexAttrib4ubv(uint index, byte[] v)
		{
			Delegates.glVertexAttrib4ubv(index, v);
		}

		public static void VertexAttrib4ubv(int index, byte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4ubv((uint)index, v);
		}

		public static void VertexAttrib4usv(uint index, ushort[] v)
		{
			Delegates.glVertexAttrib4usv(index, v);
		}

		public static void VertexAttrib4usv(int index, ushort[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4usv((uint)index, v);
		}

		public static void VertexAttrib4uiv(uint index, uint[] v)
		{
			Delegates.glVertexAttrib4uiv(index, v);
		}

		public static void VertexAttrib4uiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4uiv((uint)index, v);
		}

		public static void VertexAttrib4Nbv(uint index, sbyte[] v)
		{
			Delegates.glVertexAttrib4Nbv(index, v);
		}

		public static void VertexAttrib4Nbv(int index, sbyte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nbv((uint)index, v);
		}

		public static void VertexAttrib4Nsv(uint index, short[] v)
		{
			Delegates.glVertexAttrib4Nsv(index, v);
		}

		public static void VertexAttrib4Nsv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nsv((uint)index, v);
		}

		public static void VertexAttrib4Niv(uint index, int[] v)
		{
			Delegates.glVertexAttrib4Niv(index, v);
		}

		public static void VertexAttrib4Niv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Niv((uint)index, v);
		}

		public static void VertexAttrib4Nubv(uint index, byte[] v)
		{
			Delegates.glVertexAttrib4Nubv(index, v);
		}

		public static void VertexAttrib4Nubv(int index, byte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nubv((uint)index, v);
		}

		public static void VertexAttrib4Nusv(uint index, ushort[] v)
		{
			Delegates.glVertexAttrib4Nusv(index, v);
		}

		public static void VertexAttrib4Nusv(int index, ushort[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nusv((uint)index, v);
		}

		public static void VertexAttrib4Nuiv(uint index, uint[] v)
		{
			Delegates.glVertexAttrib4Nuiv(index, v);
		}

		public static void VertexAttrib4Nuiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttrib4Nuiv((uint)index, v);
		}

		public static void VertexAttribI4bv(uint index, sbyte[] v)
		{
			Delegates.glVertexAttribI4bv(index, v);
		}

		public static void VertexAttribI4bv(int index, sbyte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4bv((uint)index, v);
		}

		public static void VertexAttribI4ubv(uint index, byte[] v)
		{
			Delegates.glVertexAttribI4ubv(index, v);
		}

		public static void VertexAttribI4ubv(int index, byte[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4ubv((uint)index, v);
		}

		public static void VertexAttribI4sv(uint index, short[] v)
		{
			Delegates.glVertexAttribI4sv(index, v);
		}

		public static void VertexAttribI4sv(int index, short[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4sv((uint)index, v);
		}

		public static void VertexAttribI4usv(uint index, ushort[] v)
		{
			Delegates.glVertexAttribI4usv(index, v);
		}

		public static void VertexAttribI4usv(int index, ushort[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4usv((uint)index, v);
		}

		public static void VertexAttribI4iv(uint index, int[] v)
		{
			Delegates.glVertexAttribI4iv(index, v);
		}

		public static void VertexAttribI4iv(int index, int[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4iv((uint)index, v);
		}

		public static void VertexAttribI4uiv(uint index, uint[] v)
		{
			Delegates.glVertexAttribI4uiv(index, v);
		}

		public static void VertexAttribI4uiv(int index, uint[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribI4uiv((uint)index, v);
		}

		public static void VertexAttribL1dv(uint index, double[] v)
		{
			Delegates.glVertexAttribL1dv(index, v);
		}

		public static void VertexAttribL1dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL1dv((uint)index, v);
		}

		public static void VertexAttribL2dv(uint index, double[] v)
		{
			Delegates.glVertexAttribL2dv(index, v);
		}

		public static void VertexAttribL2dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL2dv((uint)index, v);
		}

		public static void VertexAttribL3dv(uint index, double[] v)
		{
			Delegates.glVertexAttribL3dv(index, v);
		}

		public static void VertexAttribL3dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL3dv((uint)index, v);
		}

		public static void VertexAttribL4dv(uint index, double[] v)
		{
			Delegates.glVertexAttribL4dv(index, v);
		}

		public static void VertexAttribL4dv(int index, double[] v)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribL4dv((uint)index, v);
		}

		public static void VertexAttribP1ui(uint index, VertexAttribPType type, bool normalized, uint value)
		{
			Delegates.glVertexAttribP1ui(index, type, normalized, value);
		}

		public static void VertexAttribP1ui(int index, VertexAttribPType type, bool normalized, uint value)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribP1ui((uint)index, type, normalized, value);
		}

		public static void VertexAttribP2ui(uint index, VertexAttribPType type, bool normalized, uint value)
		{
			Delegates.glVertexAttribP2ui(index, type, normalized, value);
		}

		public static void VertexAttribP2ui(int index, VertexAttribPType type, bool normalized, uint value)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribP2ui((uint)index, type, normalized, value);
		}

		public static void VertexAttribP3ui(uint index, VertexAttribPType type, bool normalized, uint value)
		{
			Delegates.glVertexAttribP3ui(index, type, normalized, value);
		}

		public static void VertexAttribP3ui(int index, VertexAttribPType type, bool normalized, uint value)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribP3ui((uint)index, type, normalized, value);
		}

		public static void VertexAttribP4ui(uint index, VertexAttribPType type, bool normalized, uint value)
		{
			Delegates.glVertexAttribP4ui(index, type, normalized, value);
		}

		public static void VertexAttribP4ui(int index, VertexAttribPType type, bool normalized, uint value)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribP4ui((uint)index, type, normalized, value);
		}

		public static void VertexAttribBinding(uint attribindex, uint bindingindex)
		{
			Delegates.glVertexAttribBinding(attribindex, bindingindex);
		}

		public static void VertexArrayAttribBinding(uint vaobj, uint attribindex, uint bindingindex)
		{
			Delegates.glVertexArrayAttribBinding(vaobj, attribindex, bindingindex);
		}

		public static void VertexAttribDivisor(uint index, uint divisor)
		{
			Delegates.glVertexAttribDivisor(index, divisor);
		}

		public static void VertexAttribDivisor(int index, uint divisor)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribDivisor((uint)index, divisor);
		}

		public static void VertexAttribFormat(uint attribindex, int size, VertexAttribFormat type, bool normalized, uint relativeoffset)
		{
			Delegates.glVertexAttribFormat(attribindex, size, type, normalized, relativeoffset);
		}

		public static void VertexAttribIFormat(uint attribindex, int size, VertexAttribFormat type, uint relativeoffset)
		{
			Delegates.glVertexAttribIFormat(attribindex, size, type, relativeoffset);
		}

		public static void VertexAttribLFormat(uint attribindex, int size, VertexAttribFormat type, uint relativeoffset)
		{
			Delegates.glVertexAttribLFormat(attribindex, size, type, relativeoffset);
		}

		public static void VertexArrayAttribFormat(uint vaobj, uint attribindex, int size, VertexAttribFormat type, bool normalized, uint relativeoffset)
		{
			Delegates.glVertexArrayAttribFormat(vaobj, attribindex, size, type, normalized, relativeoffset);
		}

		public static void VertexArrayAttribIFormat(uint vaobj, uint attribindex, int size, VertexAttribFormat type, uint relativeoffset)
		{
			Delegates.glVertexArrayAttribIFormat(vaobj, attribindex, size, type, relativeoffset);
		}

		public static void VertexArrayAttribLFormat(uint vaobj, uint attribindex, int size, VertexAttribFormat type, uint relativeoffset)
		{
			Delegates.glVertexArrayAttribLFormat(vaobj, attribindex, size, type, relativeoffset);
		}

		public static void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer)
		{
			Delegates.glVertexAttribPointer(index, size, type, normalized, stride, pointer);
		}

		public static void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribPointer((uint)index, size, type, normalized, stride, pointer);
		}

		public static void VertexAttribIPointer(uint index, int size, VertexAttribPointerType type, int stride, IntPtr pointer)
		{
			Delegates.glVertexAttribIPointer(index, size, type, stride, pointer);
		}

		public static void VertexAttribIPointer(int index, int size, VertexAttribPointerType type, int stride, IntPtr pointer)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribIPointer((uint)index, size, type, stride, pointer);
		}

		public static void VertexAttribLPointer(uint index, int size, VertexAttribPointerType type, int stride, IntPtr pointer)
		{
			Delegates.glVertexAttribLPointer(index, size, type, stride, pointer);
		}

		public static void VertexAttribLPointer(int index, int size, VertexAttribPointerType type, int stride, IntPtr pointer)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
			Delegates.glVertexAttribLPointer((uint)index, size, type, stride, pointer);
		}

		public static void VertexBindingDivisor(uint bindingindex, uint divisor)
		{
			Delegates.glVertexBindingDivisor(bindingindex, divisor);
		}

		public static void VertexArrayBindingDivisor(uint vaobj, uint bindingindex, uint divisor)
		{
			Delegates.glVertexArrayBindingDivisor(vaobj, bindingindex, divisor);
		}

		public static void Viewport(int x, int y, int width, int height)
		{
			Delegates.glViewport(x, y, width, height);
		}

		public static void ViewportArrayv(uint first, int count, float[] v)
		{
			Delegates.glViewportArrayv(first, count, v);
		}

		public static void ViewportIndexedf(uint index, float x, float y, float w, float h)
		{
			Delegates.glViewportIndexedf(index, x, y, w, h);
		}

		public static void ViewportIndexedfv(uint index, float[] v)
		{
			Delegates.glViewportIndexedfv(index, v);
		}

		public static void WaitSync(IntPtr sync, uint flags, ulong timeout)
		{
			Delegates.glWaitSync(sync, flags, timeout);
		}

		public static void DebugMessageCallback(Delegates.DebugProc callback, IntPtr userParam)
        {
            IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            if (Delegates.glDebugMessageCallback != null)
            {
	            Delegates.glDebugMessageCallback(callbackPtr, userParam);
            }
            else if (Delegates.glDebugMessageCallbackKHR != null)
            {
	            Delegates.glDebugMessageCallbackKHR(callbackPtr, userParam);
            }
        }

		public static void SpecializeShader(uint shader, string entryPoint, uint numSpecializationConstants, IntPtr constantIndex, IntPtr constantValue)
		{
			Delegates.glSpecializeShader(shader, entryPoint, numSpecializationConstants, constantIndex, constantValue);
		}
    }
}