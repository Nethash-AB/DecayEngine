using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace DecayEngine.OpenGL.OpenGLInterop
{
    public class Delegates
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void ActiveShaderProgram(uint pipeline, uint program);
        internal static ActiveShaderProgram glActiveShaderProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ActiveTexture(int texture);
        internal static ActiveTexture glActiveTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void AttachShader(uint program, uint shader);
        internal static AttachShader glAttachShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BeginConditionalRender(uint id, ConditionalRenderType mode);
        internal static BeginConditionalRender glBeginConditionalRender;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EndConditionalRender();
        internal static EndConditionalRender glEndConditionalRender;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BeginQuery(QueryTarget target, uint id);
        internal static BeginQuery glBeginQuery;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EndQuery(QueryTarget target);
        internal static EndQuery glEndQuery;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BeginQueryIndexed(QueryTarget target, uint index, uint id);
        internal static BeginQueryIndexed glBeginQueryIndexed;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EndQueryIndexed(QueryTarget target, uint index);
        internal static EndQueryIndexed glEndQueryIndexed;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BeginTransformFeedback(BeginFeedbackMode primitiveMode);
        internal static BeginTransformFeedback glBeginTransformFeedback;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EndTransformFeedback();
        internal static EndTransformFeedback glEndTransformFeedback;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindAttribLocation(uint program, uint index, string name);
        internal static BindAttribLocation glBindAttribLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindBuffer(BufferTarget target, uint buffer);
        internal static BindBuffer glBindBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindBufferBase(BufferTarget target, uint index, uint buffer);
        internal static BindBufferBase glBindBufferBase;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindBufferRange(BufferTarget target, uint index, uint buffer, IntPtr offset, IntPtr size);
        internal static BindBufferRange glBindBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindBuffersBase(BufferTarget target, uint first, int count, uint[] buffers);
        internal static BindBuffersBase glBindBuffersBase;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindBuffersRange(BufferTarget target, uint first, int count, uint[] buffers, IntPtr[] offsets, IntPtr[] sizes);
        internal static BindBuffersRange glBindBuffersRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindFragDataLocation(uint program, uint colorNumber, string name);
        internal static BindFragDataLocation glBindFragDataLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindFragDataLocationIndexed(uint program, uint colorNumber, uint index, string name);
        internal static BindFragDataLocationIndexed glBindFragDataLocationIndexed;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindFramebuffer(FramebufferTarget target, uint framebuffer);
        internal static BindFramebuffer glBindFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindImageTexture(uint unit, uint texture, int level, bool layered, int layer, BufferAccess access, PixelInternalFormat format);
        internal static BindImageTexture glBindImageTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindImageTextures(uint first, int count, uint[] textures);
        internal static BindImageTextures glBindImageTextures;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindProgramPipeline(uint pipeline);
        internal static BindProgramPipeline glBindProgramPipeline;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindRenderbuffer(RenderbufferTarget target, uint renderbuffer);
        internal static BindRenderbuffer glBindRenderbuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindSampler(uint unit, uint sampler);
        internal static BindSampler glBindSampler;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindSamplers(uint first, int count, uint[] samplers);
        internal static BindSamplers glBindSamplers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindTexture(TextureTarget target, uint texture);
        internal static BindTexture glBindTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindTextures(uint first, int count, uint[] textures);
        internal static BindTextures glBindTextures;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindTextureUnit(uint unit, uint texture);
        internal static BindTextureUnit glBindTextureUnit;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindTransformFeedback(NvTransformFeedback2 target, uint id);
        internal static BindTransformFeedback glBindTransformFeedback;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindVertexArray(uint array);
        internal static BindVertexArray glBindVertexArray;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindVertexBuffer(uint bindingindex, uint buffer, IntPtr offset, IntPtr stride);
        internal static BindVertexBuffer glBindVertexBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayVertexBuffer(uint vaobj, uint bindingindex, uint buffer, IntPtr offset, int stride);
        internal static VertexArrayVertexBuffer glVertexArrayVertexBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BindVertexBuffers(uint first, int count, uint[] buffers, IntPtr[] offsets, int[] strides);
        internal static BindVertexBuffers glBindVertexBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayVertexBuffers(uint vaobj, uint first, int count, uint[] buffers, IntPtr[] offsets, int[] strides);
        internal static VertexArrayVertexBuffers glVertexArrayVertexBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendColor(float red, float green, float blue, float alpha);
        internal static BlendColor glBlendColor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendEquation(BlendEquationMode mode);
        internal static BlendEquation glBlendEquation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendEquationi(uint buf, BlendEquationMode mode);
        internal static BlendEquationi glBlendEquationi;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendEquationSeparate(BlendEquationMode modeRGB, BlendEquationMode modeAlpha);
        internal static BlendEquationSeparate glBlendEquationSeparate;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendEquationSeparatei(uint buf, BlendEquationMode modeRGB, BlendEquationMode modeAlpha);
        internal static BlendEquationSeparatei glBlendEquationSeparatei;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
        internal static BlendFunc glBlendFunc;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendFunci(uint buf, BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
        internal static BlendFunci glBlendFunci;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendFuncSeparate(BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha);
        internal static BlendFuncSeparate glBlendFuncSeparate;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendFuncSeparatei(uint buf, BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha);
        internal static BlendFuncSeparatei glBlendFuncSeparatei;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter);
        internal static BlitFramebuffer glBlitFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlitNamedFramebuffer(uint readFramebuffer, uint drawFramebuffer, int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter);
        internal static BlitNamedFramebuffer glBlitNamedFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage);
        internal static BufferData glBufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedBufferData(uint buffer, int size, IntPtr data, BufferUsageHint usage);
        internal static NamedBufferData glNamedBufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BufferStorage(BufferTarget target, IntPtr size, IntPtr data, uint flags);
        internal static BufferStorage glBufferStorage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedBufferStorage(uint buffer, int size, IntPtr data, uint flags);
        internal static NamedBufferStorage glNamedBufferStorage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
        internal static BufferSubData glBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedBufferSubData(uint buffer, IntPtr offset, int size, IntPtr data);
        internal static NamedBufferSubData glNamedBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target);
        internal static CheckFramebufferStatus glCheckFramebufferStatus;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate FramebufferErrorCode CheckNamedFramebufferStatus(uint framebuffer, FramebufferTarget target);
        internal static CheckNamedFramebufferStatus glCheckNamedFramebufferStatus;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClampColor(ClampColorTarget target, ClampColorMode clamp);
        internal static ClampColor glClampColor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Clear(ClearBufferMask mask);
        internal static Clear glClear;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferiv(ClearBuffer buffer, int drawbuffer, int[] value);
        internal static ClearBufferiv glClearBufferiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferuiv(ClearBuffer buffer, int drawbuffer, uint[] value);
        internal static ClearBufferuiv glClearBufferuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferfv(ClearBuffer buffer, int drawbuffer, float[] value);
        internal static ClearBufferfv glClearBufferfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferfi(ClearBuffer buffer, int drawbuffer, float depth, int stencil);
        internal static ClearBufferfi glClearBufferfi;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedFramebufferiv(uint framebuffer, ClearBuffer buffer, int drawbuffer, int[] value);
        internal static ClearNamedFramebufferiv glClearNamedFramebufferiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedFramebufferuiv(uint framebuffer, ClearBuffer buffer, int drawbuffer, uint[] value);
        internal static ClearNamedFramebufferuiv glClearNamedFramebufferuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedFramebufferfv(uint framebuffer, ClearBuffer buffer, int drawbuffer, float[] value);
        internal static ClearNamedFramebufferfv glClearNamedFramebufferfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedFramebufferfi(uint framebuffer, ClearBuffer buffer, int drawbuffer, float depth, int stencil);
        internal static ClearNamedFramebufferfi glClearNamedFramebufferfi;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferData(BufferTarget target, SizedInternalFormat internalFormat, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearBufferData glClearBufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedBufferData(uint buffer, SizedInternalFormat internalFormat, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearNamedBufferData glClearNamedBufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearBufferSubData(BufferTarget target, SizedInternalFormat internalFormat, IntPtr offset, IntPtr size, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearBufferSubData glClearBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearNamedBufferSubData(uint buffer, SizedInternalFormat internalFormat, IntPtr offset, int size, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearNamedBufferSubData glClearNamedBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearColor(float red, float green, float blue, float alpha);
        internal static ClearColor glClearColor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearDepth(double depth);
        internal static ClearDepth glClearDepth;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearDepthf(float depth);
        internal static ClearDepthf glClearDepthf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearStencil(int s);
        internal static ClearStencil glClearStencil;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearTexImage(uint texture, int level, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearTexImage glClearTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClearTexSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelInternalFormat format, PixelType type, IntPtr data);
        internal static ClearTexSubImage glClearTexSubImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ArbSync ClientWaitSync(IntPtr sync, uint flags, ulong timeout);
        internal static ClientWaitSync glClientWaitSync;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ClipControl(ClipControlOrigin origin, ClipControlDepth depth);
        internal static ClipControl glClipControl;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ColorMask(bool red, bool green, bool blue, bool alpha);
        internal static ColorMask glColorMask;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ColorMaski(uint buf, bool red, bool green, bool blue, bool alpha);
        internal static ColorMaski glColorMaski;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompileShader(uint shader);
        internal static CompileShader glCompileShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int border, int imageSize, IntPtr data);
        internal static CompressedTexImage1D glCompressedTexImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int border, int imageSize, IntPtr data);
        internal static CompressedTexImage2D glCompressedTexImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexImage3D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int depth, int border, int imageSize, IntPtr data);
        internal static CompressedTexImage3D glCompressedTexImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, int imageSize, IntPtr data);
        internal static CompressedTexSubImage1D glCompressedTexSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTextureSubImage1D(uint texture, int level, int xoffset, int width, PixelInternalFormat format, int imageSize, IntPtr data);
        internal static CompressedTextureSubImage1D glCompressedTextureSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, int imageSize, IntPtr data);
        internal static CompressedTexSubImage2D glCompressedTexSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelInternalFormat format, int imageSize, IntPtr data);
        internal static CompressedTextureSubImage2D glCompressedTextureSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, int imageSize, IntPtr data);
        internal static CompressedTexSubImage3D glCompressedTexSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompressedTextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelInternalFormat format, int imageSize, IntPtr data);
        internal static CompressedTextureSubImage3D glCompressedTextureSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyBufferSubData(BufferTarget readTarget, BufferTarget writeTarget, IntPtr readOffset, IntPtr writeOffset, IntPtr size);
        internal static CopyBufferSubData glCopyBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyNamedBufferSubData(uint readBuffer, uint writeBuffer, IntPtr readOffset, IntPtr writeOffset, int size);
        internal static CopyNamedBufferSubData glCopyNamedBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyImageSubData(uint srcName, BufferTarget srcTarget, int srcLevel, int srcX, int srcY, int srcZ, uint dstName, BufferTarget dstTarget, int dstLevel, int dstX, int dstY, int dstZ, int srcWidth, int srcHeight, int srcDepth);
        internal static CopyImageSubData glCopyImageSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int border);
        internal static CopyTexImage1D glCopyTexImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int height, int border);
        internal static CopyTexImage2D glCopyTexImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTexSubImage1D(TextureTarget target, int level, int xoffset, int x, int y, int width);
        internal static CopyTexSubImage1D glCopyTexSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTextureSubImage1D(uint texture, int level, int xoffset, int x, int y, int width);
        internal static CopyTextureSubImage1D glCopyTextureSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int x, int y, int width, int height);
        internal static CopyTexSubImage2D glCopyTexSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int x, int y, int width, int height);
        internal static CopyTextureSubImage2D glCopyTextureSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);
        internal static CopyTexSubImage3D glCopyTexSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CopyTextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);
        internal static CopyTextureSubImage3D glCopyTextureSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateBuffers(int n, uint[] buffers);
        internal static CreateBuffers glCreateBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateFramebuffers(int n, uint[] ids);
        internal static CreateFramebuffers glCreateFramebuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint CreateProgram();
        internal static CreateProgram glCreateProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateProgramPipelines(int n, uint[] pipelines);
        internal static CreateProgramPipelines glCreateProgramPipelines;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateQueries(QueryTarget target, int n, uint[] ids);
        internal static CreateQueries glCreateQueries;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateRenderbuffers(int n, uint[] renderbuffers);
        internal static CreateRenderbuffers glCreateRenderbuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateSamplers(int n, uint[] samplers);
        internal static CreateSamplers glCreateSamplers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint CreateShader(ShaderType shaderType);
        internal static CreateShader glCreateShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint CreateShaderProgramv(ShaderType type, int count, string[] strings);
        internal static CreateShaderProgramv glCreateShaderProgramv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateTextures(TextureTarget target, int n, uint[] textures);
        internal static CreateTextures glCreateTextures;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateTransformFeedbacks(int n, uint[] ids);
        internal static CreateTransformFeedbacks glCreateTransformFeedbacks;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CreateVertexArrays(int n, uint[] arrays);
        internal static CreateVertexArrays glCreateVertexArrays;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CullFace(CullFaceMode mode);
        internal static CullFace glCullFace;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteBuffers(int n, uint[] buffers);
        internal static DeleteBuffers glDeleteBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteFramebuffers(int n, uint[] framebuffers);
        internal static DeleteFramebuffers glDeleteFramebuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteProgram(uint program);
        internal static DeleteProgram glDeleteProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteProgramPipelines(int n, uint[] pipelines);
        internal static DeleteProgramPipelines glDeleteProgramPipelines;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteQueries(int n, uint[] ids);
        internal static DeleteQueries glDeleteQueries;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteRenderbuffers(int n, uint[] renderbuffers);
        internal static DeleteRenderbuffers glDeleteRenderbuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteSamplers(int n, uint[] samplers);
        internal static DeleteSamplers glDeleteSamplers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteShader(uint shader);
        internal static DeleteShader glDeleteShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteSync(IntPtr sync);
        internal static DeleteSync glDeleteSync;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteTextures(int n, uint[] textures);
        internal static DeleteTextures glDeleteTextures;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteTransformFeedbacks(int n, uint[] ids);
        internal static DeleteTransformFeedbacks glDeleteTransformFeedbacks;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteVertexArrays(int n, uint[] arrays);
        internal static DeleteVertexArrays glDeleteVertexArrays;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthFunc(DepthFunction func);
        internal static DepthFunc glDepthFunc;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthMask(bool flag);
        internal static DepthMask glDepthMask;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthRange(double nearVal, double farVal);
        internal static DepthRange glDepthRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthRangef(float nearVal, float farVal);
        internal static DepthRangef glDepthRangef;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthRangeArrayv(uint first, int count, double[] v);
        internal static DepthRangeArrayv glDepthRangeArrayv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DepthRangeIndexed(uint index, double nearVal, double farVal);
        internal static DepthRangeIndexed glDepthRangeIndexed;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DetachShader(uint program, uint shader);
        internal static DetachShader glDetachShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DispatchCompute(uint num_groups_x, uint num_groups_y, uint num_groups_z);
        internal static DispatchCompute glDispatchCompute;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DispatchComputeIndirect(IntPtr indirect);
        internal static DispatchComputeIndirect glDispatchComputeIndirect;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawArrays(BeginMode mode, int first, int count);
        internal static DrawArrays glDrawArrays;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawArraysIndirect(BeginMode mode, IntPtr indirect);
        internal static DrawArraysIndirect glDrawArraysIndirect;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawArraysInstanced(BeginMode mode, int first, int count, int primcount);
        internal static DrawArraysInstanced glDrawArraysInstanced;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawArraysInstancedBaseInstance(BeginMode mode, int first, int count, int primcount, uint baseinstance);
        internal static DrawArraysInstancedBaseInstance glDrawArraysInstancedBaseInstance;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawBuffer(DrawBufferMode buf);
        internal static DrawBuffer glDrawBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferDrawBuffer(uint framebuffer, DrawBufferMode buf);
        internal static NamedFramebufferDrawBuffer glNamedFramebufferDrawBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawBuffers(int n, DrawBuffersEnum[] bufs);
        internal static DrawBuffers glDrawBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferDrawBuffers(uint framebuffer, int n, DrawBufferMode[] bufs);
        internal static NamedFramebufferDrawBuffers glNamedFramebufferDrawBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices);
        internal static DrawElements glDrawElements;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int basevertex);
        internal static DrawElementsBaseVertex glDrawElementsBaseVertex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsIndirect(BeginMode mode, DrawElementsType type, IntPtr indirect);
        internal static DrawElementsIndirect glDrawElementsIndirect;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsInstanced(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount);
        internal static DrawElementsInstanced glDrawElementsInstanced;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsInstancedBaseInstance(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, uint baseinstance);
        internal static DrawElementsInstancedBaseInstance glDrawElementsInstancedBaseInstance;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsInstancedBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex);
        internal static DrawElementsInstancedBaseVertex glDrawElementsInstancedBaseVertex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawElementsInstancedBaseVertexBaseInstance(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex, uint baseinstance);
        internal static DrawElementsInstancedBaseVertexBaseInstance glDrawElementsInstancedBaseVertexBaseInstance;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawRangeElements(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices);
        internal static DrawRangeElements glDrawRangeElements;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawRangeElementsBaseVertex(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices, int basevertex);
        internal static DrawRangeElementsBaseVertex glDrawRangeElementsBaseVertex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawTransformFeedback(NvTransformFeedback2 mode, uint id);
        internal static DrawTransformFeedback glDrawTransformFeedback;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawTransformFeedbackInstanced(BeginMode mode, uint id, int primcount);
        internal static DrawTransformFeedbackInstanced glDrawTransformFeedbackInstanced;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawTransformFeedbackStream(NvTransformFeedback2 mode, uint id, uint stream);
        internal static DrawTransformFeedbackStream glDrawTransformFeedbackStream;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DrawTransformFeedbackStreamInstanced(BeginMode mode, uint id, uint stream, int primcount);
        internal static DrawTransformFeedbackStreamInstanced glDrawTransformFeedbackStreamInstanced;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Enable(EnableCap cap);
        internal static Enable glEnable;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Disable(EnableCap cap);
        internal static Disable glDisable;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Enablei(EnableCap cap, uint index);
        internal static Enablei glEnablei;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Disablei(EnableCap cap, uint index);
        internal static Disablei glDisablei;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EnableVertexAttribArray(uint index);
        internal static EnableVertexAttribArray glEnableVertexAttribArray;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DisableVertexAttribArray(uint index);
        internal static DisableVertexAttribArray glDisableVertexAttribArray;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void EnableVertexArrayAttrib(uint vaobj, uint index);
        internal static EnableVertexArrayAttrib glEnableVertexArrayAttrib;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DisableVertexArrayAttrib(uint vaobj, uint index);
        internal static DisableVertexArrayAttrib glDisableVertexArrayAttrib;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr FenceSync(ArbSync condition, uint flags);
        internal static FenceSync glFenceSync;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Finish();
        internal static Finish glFinish;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Flush();
        internal static Flush glFlush;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FlushMappedBufferRange(BufferTarget target, IntPtr offset, IntPtr length);
        internal static FlushMappedBufferRange glFlushMappedBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FlushMappedNamedBufferRange(uint buffer, IntPtr offset, int length);
        internal static FlushMappedNamedBufferRange glFlushMappedNamedBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferParameteri(FramebufferTarget target, FramebufferPName pname, int param);
        internal static FramebufferParameteri glFramebufferParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferParameteri(uint framebuffer, FramebufferPName pname, int param);
        internal static NamedFramebufferParameteri glNamedFramebufferParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer);
        internal static FramebufferRenderbuffer glFramebufferRenderbuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferRenderbuffer(uint framebuffer, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer);
        internal static NamedFramebufferRenderbuffer glNamedFramebufferRenderbuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level);
        internal static FramebufferTexture glFramebufferTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level);
        internal static FramebufferTexture1D glFramebufferTexture1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level);
        internal static FramebufferTexture2D glFramebufferTexture2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level, int layer);
        internal static FramebufferTexture3D glFramebufferTexture3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferTexture(uint framebuffer, FramebufferAttachment attachment, uint texture, int level);
        internal static NamedFramebufferTexture glNamedFramebufferTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer);
        internal static FramebufferTextureLayer glFramebufferTextureLayer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferTextureLayer(uint framebuffer, FramebufferAttachment attachment, uint texture, int level, int layer);
        internal static NamedFramebufferTextureLayer glNamedFramebufferTextureLayer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void FrontFace(FrontFaceDirection mode);
        internal static FrontFace glFrontFace;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenBuffers(int n, [OutAttribute] uint[] buffers);
        internal static GenBuffers glGenBuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenerateMipmap(GenerateMipmapTarget target);
        internal static GenerateMipmap glGenerateMipmap;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenerateTextureMipmap(uint texture);
        internal static GenerateTextureMipmap glGenerateTextureMipmap;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenFramebuffers(int n, [OutAttribute] uint[] ids);
        internal static GenFramebuffers glGenFramebuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenProgramPipelines(int n, [OutAttribute] uint[] pipelines);
        internal static GenProgramPipelines glGenProgramPipelines;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenQueries(int n, [OutAttribute] uint[] ids);
        internal static GenQueries glGenQueries;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenRenderbuffers(int n, [OutAttribute] uint[] renderbuffers);
        internal static GenRenderbuffers glGenRenderbuffers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenSamplers(int n, [OutAttribute] uint[] samplers);
        internal static GenSamplers glGenSamplers;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenTextures(int n, [OutAttribute] uint[] textures);
        internal static GenTextures glGenTextures;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenTransformFeedbacks(int n, [OutAttribute] uint[] ids);
        internal static GenTransformFeedbacks glGenTransformFeedbacks;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GenVertexArrays(int n, [OutAttribute] uint[] arrays);
        internal static GenVertexArrays glGenVertexArrays;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBooleanv(GetPName pname, [OutAttribute] bool[] data);
        internal static GetBooleanv glGetBooleanv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetDoublev(GetPName pname, [OutAttribute] double[] data);
        internal static GetDoublev glGetDoublev;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetFloatv(GetPName pname, [OutAttribute] float[] data);
        internal static GetFloatv glGetFloatv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetIntegerv(GetPName pname, [OutAttribute] int[] data);
        internal static GetIntegerv glGetIntegerv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetInteger64v(ArbSync pname, [OutAttribute] long[] data);
        internal static GetInteger64v glGetInteger64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBooleani_v(GetPName target, uint index, [OutAttribute] bool[] data);
        internal static GetBooleani_v glGetBooleani_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetIntegeri_v(GetPName target, uint index, [OutAttribute] int[] data);
        internal static GetIntegeri_v glGetIntegeri_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetFloati_v(GetPName target, uint index, [OutAttribute] float[] data);
        internal static GetFloati_v glGetFloati_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetDoublei_v(GetPName target, uint index, [OutAttribute] double[] data);
        internal static GetDoublei_v glGetDoublei_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetInteger64i_v(GetPName target, uint index, [OutAttribute] long[] data);
        internal static GetInteger64i_v glGetInteger64i_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveAtomicCounterBufferiv(uint program, uint bufferIndex, AtomicCounterParameterName pname, [OutAttribute] int[] @params);
        internal static GetActiveAtomicCounterBufferiv glGetActiveAtomicCounterBufferiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveAttrib(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveAttribType[] type, [OutAttribute] System.Text.StringBuilder name);
        internal static GetActiveAttrib glGetActiveAttrib;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveSubroutineName(uint program, ShaderType shadertype, uint index, int bufsize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name);
        internal static GetActiveSubroutineName glGetActiveSubroutineName;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveSubroutineUniformiv(uint program, ShaderType shadertype, uint index, SubroutineParameterName pname, [OutAttribute] int[] values);
        internal static GetActiveSubroutineUniformiv glGetActiveSubroutineUniformiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveSubroutineUniformName(uint program, ShaderType shadertype, uint index, int bufsize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name);
        internal static GetActiveSubroutineUniformName glGetActiveSubroutineUniformName;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveUniform(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveUniformType[] type, [OutAttribute] System.Text.StringBuilder name);
        internal static GetActiveUniform glGetActiveUniform;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveUniformBlockiv(uint program, uint uniformBlockIndex, ActiveUniformBlockParameter pname, [OutAttribute] int[] @params);
        internal static GetActiveUniformBlockiv glGetActiveUniformBlockiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder uniformBlockName);
        internal static GetActiveUniformBlockName glGetActiveUniformBlockName;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveUniformName(uint program, uint uniformIndex, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder uniformName);
        internal static GetActiveUniformName glGetActiveUniformName;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetActiveUniformsiv(uint program, int uniformCount, [OutAttribute] uint[] uniformIndices, ActiveUniformType pname, [OutAttribute] int[] @params);
        internal static GetActiveUniformsiv glGetActiveUniformsiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetAttachedShaders(uint program, int maxCount, [OutAttribute] int[] count, [OutAttribute] uint[] shaders);
        internal static GetAttachedShaders glGetAttachedShaders;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetAttribLocation(uint program, string name);
        internal static GetAttribLocation glGetAttribLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBufferParameteriv(BufferTarget target, BufferParameterName value, [OutAttribute] int[] data);
        internal static GetBufferParameteriv glGetBufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBufferParameteri64v(BufferTarget target, BufferParameterName value, [OutAttribute] long[] data);
        internal static GetBufferParameteri64v glGetBufferParameteri64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedBufferParameteriv(uint buffer, BufferParameterName pname, [OutAttribute] int[] @params);
        internal static GetNamedBufferParameteriv glGetNamedBufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedBufferParameteri64v(uint buffer, BufferParameterName pname, [OutAttribute] long[] @params);
        internal static GetNamedBufferParameteri64v glGetNamedBufferParameteri64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBufferPointerv(BufferTarget target, BufferPointer pname, [OutAttribute] IntPtr @params);
        internal static GetBufferPointerv glGetBufferPointerv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedBufferPointerv(uint buffer, BufferPointer pname, [OutAttribute] IntPtr @params);
        internal static GetNamedBufferPointerv glGetNamedBufferPointerv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetBufferSubData(BufferTarget target, IntPtr offset, IntPtr size, [OutAttribute] IntPtr data);
        internal static GetBufferSubData glGetBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedBufferSubData(uint buffer, IntPtr offset, int size, [OutAttribute] IntPtr data);
        internal static GetNamedBufferSubData glGetNamedBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetCompressedTexImage(TextureTarget target, int level, [OutAttribute] IntPtr pixels);
        internal static GetCompressedTexImage glGetCompressedTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnCompressedTexImage(TextureTarget target, int level, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetnCompressedTexImage glGetnCompressedTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetCompressedTextureImage(uint texture, int level, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetCompressedTextureImage glGetCompressedTextureImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetCompressedTextureSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetCompressedTextureSubImage glGetCompressedTextureSubImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ErrorCode GetError();
        internal static GetError glGetError;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetFragDataIndex(uint program, string name);
        internal static GetFragDataIndex glGetFragDataIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetFragDataLocation(uint program, string name);
        internal static GetFragDataLocation glGetFragDataLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetFramebufferAttachmentParameteriv(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, [OutAttribute] int[] @params);
        internal static GetFramebufferAttachmentParameteriv glGetFramebufferAttachmentParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedFramebufferAttachmentParameteriv(uint framebuffer, FramebufferAttachment attachment, FramebufferParameterName pname, [OutAttribute] int[] @params);
        internal static GetNamedFramebufferAttachmentParameteriv glGetNamedFramebufferAttachmentParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetFramebufferParameteriv(FramebufferTarget target, FramebufferPName pname, [OutAttribute] int[] @params);
        internal static GetFramebufferParameteriv glGetFramebufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedFramebufferParameteriv(uint framebuffer, FramebufferPName pname, [OutAttribute] int[] param);
        internal static GetNamedFramebufferParameteriv glGetNamedFramebufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate GraphicResetStatus GetGraphicsResetStatus();
        internal static GetGraphicsResetStatus glGetGraphicsResetStatus;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetInternalformativ(TextureTarget target, PixelInternalFormat internalFormat, GetPName pname, int bufSize, [OutAttribute] int[] @params);
        internal static GetInternalformativ glGetInternalformativ;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetInternalformati64v(TextureTarget target, PixelInternalFormat internalFormat, GetPName pname, int bufSize, [OutAttribute] long[] @params);
        internal static GetInternalformati64v glGetInternalformati64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetMultisamplefv(GetMultisamplePName pname, uint index, [OutAttribute] float[] val);
        internal static GetMultisamplefv glGetMultisamplefv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetObjectLabel(OpenGLInterop.ObjectLabel identifier, uint name, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder label);
        internal static GetObjectLabel glGetObjectLabel;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetObjectPtrLabel([OutAttribute] IntPtr ptr, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder label);
        internal static GetObjectPtrLabel glGetObjectPtrLabel;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetPointerv(GetPointerParameter pname, [OutAttribute] IntPtr @params);
        internal static GetPointerv glGetPointerv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramiv(uint program, ProgramParameter pname, [OutAttribute] int[] @params);
        internal static GetProgramiv glGetProgramiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramBinary(uint program, int bufsize, [OutAttribute] int[] length, [OutAttribute] int[] binaryFormat, [OutAttribute] IntPtr binary);
        internal static GetProgramBinary glGetProgramBinary;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramInfoLog(uint program, int maxLength, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog);
        internal static GetProgramInfoLog glGetProgramInfoLog;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramInterfaceiv(uint program, ProgramInterface programInterface, ProgramInterfaceParameterName pname, [OutAttribute] int[] @params);
        internal static GetProgramInterfaceiv glGetProgramInterfaceiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramPipelineiv(uint pipeline, int pname, [OutAttribute] int[] @params);
        internal static GetProgramPipelineiv glGetProgramPipelineiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramPipelineInfoLog(uint pipeline, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog);
        internal static GetProgramPipelineInfoLog glGetProgramPipelineInfoLog;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramResourceiv(uint program, ProgramInterface programInterface, uint index, int propCount, [OutAttribute] ProgramResourceParameterName[] props, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] @params);
        internal static GetProgramResourceiv glGetProgramResourceiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint GetProgramResourceIndex(uint program, ProgramInterface programInterface, string name);
        internal static GetProgramResourceIndex glGetProgramResourceIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetProgramResourceLocation(uint program, ProgramInterface programInterface, string name);
        internal static GetProgramResourceLocation glGetProgramResourceLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetProgramResourceLocationIndex(uint program, ProgramInterface programInterface, string name);
        internal static GetProgramResourceLocationIndex glGetProgramResourceLocationIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramResourceName(uint program, ProgramInterface programInterface, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder name);
        internal static GetProgramResourceName glGetProgramResourceName;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramStageiv(uint program, ShaderType shadertype, ProgramStageParameterName pname, [OutAttribute] int[] values);
        internal static GetProgramStageiv glGetProgramStageiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryIndexediv(QueryTarget target, uint index, GetQueryParam pname, [OutAttribute] int[] @params);
        internal static GetQueryIndexediv glGetQueryIndexediv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryiv(QueryTarget target, GetQueryParam pname, [OutAttribute] int[] @params);
        internal static GetQueryiv glGetQueryiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryObjectiv(uint id, GetQueryObjectParam pname, [OutAttribute] int[] @params);
        internal static GetQueryObjectiv glGetQueryObjectiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryObjectuiv(uint id, GetQueryObjectParam pname, [OutAttribute] uint[] @params);
        internal static GetQueryObjectuiv glGetQueryObjectuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryObjecti64v(uint id, GetQueryObjectParam pname, [OutAttribute] long[] @params);
        internal static GetQueryObjecti64v glGetQueryObjecti64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetQueryObjectui64v(uint id, GetQueryObjectParam pname, [OutAttribute] ulong[] @params);
        internal static GetQueryObjectui64v glGetQueryObjectui64v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetRenderbufferParameteriv(RenderbufferTarget target, RenderbufferParameterName pname, [OutAttribute] int[] @params);
        internal static GetRenderbufferParameteriv glGetRenderbufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetNamedRenderbufferParameteriv(uint renderbuffer, RenderbufferParameterName pname, [OutAttribute] int[] @params);
        internal static GetNamedRenderbufferParameteriv glGetNamedRenderbufferParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetSamplerParameterfv(uint sampler, TextureParameterName pname, [OutAttribute] float[] @params);
        internal static GetSamplerParameterfv glGetSamplerParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetSamplerParameteriv(uint sampler, TextureParameterName pname, [OutAttribute] int[] @params);
        internal static GetSamplerParameteriv glGetSamplerParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetSamplerParameterIiv(uint sampler, TextureParameterName pname, [OutAttribute] int[] @params);
        internal static GetSamplerParameterIiv glGetSamplerParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetSamplerParameterIuiv(uint sampler, TextureParameterName pname, [OutAttribute] uint[] @params);
        internal static GetSamplerParameterIuiv glGetSamplerParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderiv(uint shader, ShaderParameter pname, [OutAttribute] int[] @params);
        internal static GetShaderiv glGetShaderiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderInfoLog(uint shader, int maxLength, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder infoLog);
        internal static GetShaderInfoLog glGetShaderInfoLog;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderPrecisionFormat(ShaderType shaderType, int precisionType, [OutAttribute] int[] range, [OutAttribute] int[] precision);
        internal static GetShaderPrecisionFormat glGetShaderPrecisionFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderSource(uint shader, int bufSize, [OutAttribute] int[] length, [OutAttribute] System.Text.StringBuilder source);
        internal static GetShaderSource glGetShaderSource;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr GetString(StringName name);
        internal static GetString glGetString;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr GetStringi(StringName name, uint index);
        internal static GetStringi glGetStringi;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint GetSubroutineIndex(uint program, ShaderType shadertype, string name);
        internal static GetSubroutineIndex glGetSubroutineIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetSubroutineUniformLocation(uint program, ShaderType shadertype, string name);
        internal static GetSubroutineUniformLocation glGetSubroutineUniformLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetSynciv(IntPtr sync, ArbSync pname, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] values);
        internal static GetSynciv glGetSynciv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, [OutAttribute] IntPtr pixels);
        internal static GetTexImage glGetTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetnTexImage glGetnTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureImage(uint texture, int level, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetTextureImage glGetTextureImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexLevelParameterfv(GetPName target, int level, GetTextureLevelParameter pname, [OutAttribute] float[] @params);
        internal static GetTexLevelParameterfv glGetTexLevelParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexLevelParameteriv(GetPName target, int level, GetTextureLevelParameter pname, [OutAttribute] int[] @params);
        internal static GetTexLevelParameteriv glGetTexLevelParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureLevelParameterfv(uint texture, int level, GetTextureLevelParameter pname, [OutAttribute] float[] @params);
        internal static GetTextureLevelParameterfv glGetTextureLevelParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureLevelParameteriv(uint texture, int level, GetTextureLevelParameter pname, [OutAttribute] int[] @params);
        internal static GetTextureLevelParameteriv glGetTextureLevelParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexParameterfv(TextureTarget target, GetTextureParameter pname, [OutAttribute] float[] @params);
        internal static GetTexParameterfv glGetTexParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexParameteriv(TextureTarget target, GetTextureParameter pname, [OutAttribute] int[] @params);
        internal static GetTexParameteriv glGetTexParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexParameterIiv(TextureTarget target, GetTextureParameter pname, [OutAttribute] int[] @params);
        internal static GetTexParameterIiv glGetTexParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTexParameterIuiv(TextureTarget target, GetTextureParameter pname, [OutAttribute] uint[] @params);
        internal static GetTexParameterIuiv glGetTexParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureParameterfv(uint texture, GetTextureParameter pname, [OutAttribute] float[] @params);
        internal static GetTextureParameterfv glGetTextureParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureParameteriv(uint texture, GetTextureParameter pname, [OutAttribute] int[] @params);
        internal static GetTextureParameteriv glGetTextureParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureParameterIiv(uint texture, GetTextureParameter pname, [OutAttribute] int[] @params);
        internal static GetTextureParameterIiv glGetTextureParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureParameterIuiv(uint texture, GetTextureParameter pname, [OutAttribute] uint[] @params);
        internal static GetTextureParameterIuiv glGetTextureParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTextureSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, int bufSize, [OutAttribute] IntPtr pixels);
        internal static GetTextureSubImage glGetTextureSubImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTransformFeedbackiv(uint xfb, TransformFeedbackParameterName pname, [OutAttribute] int[] param);
        internal static GetTransformFeedbackiv glGetTransformFeedbackiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTransformFeedbacki_v(uint xfb, TransformFeedbackParameterName pname, uint index, [OutAttribute] int[] param);
        internal static GetTransformFeedbacki_v glGetTransformFeedbacki_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTransformFeedbacki64_v(uint xfb, TransformFeedbackParameterName pname, uint index, [OutAttribute] long[] param);
        internal static GetTransformFeedbacki64_v glGetTransformFeedbacki64_v;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetTransformFeedbackVarying(uint program, uint index, int bufSize, [OutAttribute] int[] length, [OutAttribute] int[] size, [OutAttribute] ActiveAttribType[] type, [OutAttribute] System.Text.StringBuilder name);
        internal static GetTransformFeedbackVarying glGetTransformFeedbackVarying;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformfv(uint program, int location, [OutAttribute] float[] @params);
        internal static GetUniformfv glGetUniformfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformiv(uint program, int location, [OutAttribute] int[] @params);
        internal static GetUniformiv glGetUniformiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformuiv(uint program, int location, [OutAttribute] uint[] @params);
        internal static GetUniformuiv glGetUniformuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformdv(uint program, int location, [OutAttribute] double[] @params);
        internal static GetUniformdv glGetUniformdv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnUniformfv(uint program, int location, int bufSize, [OutAttribute] float[] @params);
        internal static GetnUniformfv glGetnUniformfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnUniformiv(uint program, int location, int bufSize, [OutAttribute] int[] @params);
        internal static GetnUniformiv glGetnUniformiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnUniformuiv(uint program, int location, int bufSize, [OutAttribute] uint[] @params);
        internal static GetnUniformuiv glGetnUniformuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetnUniformdv(uint program, int location, int bufSize, [OutAttribute] double[] @params);
        internal static GetnUniformdv glGetnUniformdv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint GetUniformBlockIndex(uint program, string uniformBlockName);
        internal static GetUniformBlockIndex glGetUniformBlockIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformIndices(uint program, int uniformCount, string uniformNames, [OutAttribute] uint[] uniformIndices);
        internal static GetUniformIndices glGetUniformIndices;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int GetUniformLocation(uint program, string name);
        internal static GetUniformLocation glGetUniformLocation;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetUniformSubroutineuiv(ShaderType shadertype, int location, [OutAttribute] uint[] values);
        internal static GetUniformSubroutineuiv glGetUniformSubroutineuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexArrayIndexed64iv(uint vaobj, uint index, VertexAttribParameter pname, [OutAttribute] long[] param);
        internal static GetVertexArrayIndexed64iv glGetVertexArrayIndexed64iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexArrayIndexediv(uint vaobj, uint index, VertexAttribParameter pname, [OutAttribute] int[] param);
        internal static GetVertexArrayIndexediv glGetVertexArrayIndexediv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexArrayiv(uint vaobj, VertexAttribParameter pname, [OutAttribute] int[] param);
        internal static GetVertexArrayiv glGetVertexArrayiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribdv(uint index, VertexAttribParameter pname, [OutAttribute] double[] @params);
        internal static GetVertexAttribdv glGetVertexAttribdv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribfv(uint index, VertexAttribParameter pname, [OutAttribute] float[] @params);
        internal static GetVertexAttribfv glGetVertexAttribfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribiv(uint index, VertexAttribParameter pname, [OutAttribute] int[] @params);
        internal static GetVertexAttribiv glGetVertexAttribiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribIiv(uint index, VertexAttribParameter pname, [OutAttribute] int[] @params);
        internal static GetVertexAttribIiv glGetVertexAttribIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribIuiv(uint index, VertexAttribParameter pname, [OutAttribute] uint[] @params);
        internal static GetVertexAttribIuiv glGetVertexAttribIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribLdv(uint index, VertexAttribParameter pname, [OutAttribute] double[] @params);
        internal static GetVertexAttribLdv glGetVertexAttribLdv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetVertexAttribPointerv(uint index, VertexAttribPointerParameter pname, [OutAttribute] IntPtr pointer);
        internal static GetVertexAttribPointerv glGetVertexAttribPointerv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Hint(HintTarget target, HintMode mode);
        internal static Hint glHint;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateBufferData(uint buffer);
        internal static InvalidateBufferData glInvalidateBufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateBufferSubData(uint buffer, IntPtr offset, IntPtr length);
        internal static InvalidateBufferSubData glInvalidateBufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateFramebuffer(FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments);
        internal static InvalidateFramebuffer glInvalidateFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateNamedFramebufferData(uint framebuffer, int numAttachments, FramebufferAttachment[] attachments);
        internal static InvalidateNamedFramebufferData glInvalidateNamedFramebufferData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateSubFramebuffer(FramebufferTarget target, int numAttachments, FramebufferAttachment[] attachments, int x, int y, int width, int height);
        internal static InvalidateSubFramebuffer glInvalidateSubFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateNamedFramebufferSubData(uint framebuffer, int numAttachments, FramebufferAttachment[] attachments, int x, int y, int width, int height);
        internal static InvalidateNamedFramebufferSubData glInvalidateNamedFramebufferSubData;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateTexImage(uint texture, int level);
        internal static InvalidateTexImage glInvalidateTexImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void InvalidateTexSubImage(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth);
        internal static InvalidateTexSubImage glInvalidateTexSubImage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsBuffer(uint buffer);
        internal static IsBuffer glIsBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsEnabled(EnableCap cap);
        internal static IsEnabled glIsEnabled;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsEnabledi(EnableCap cap, uint index);
        internal static IsEnabledi glIsEnabledi;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsFramebuffer(uint framebuffer);
        internal static IsFramebuffer glIsFramebuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsProgram(uint program);
        internal static IsProgram glIsProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsProgramPipeline(uint pipeline);
        internal static IsProgramPipeline glIsProgramPipeline;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsQuery(uint id);
        internal static IsQuery glIsQuery;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsRenderbuffer(uint renderbuffer);
        internal static IsRenderbuffer glIsRenderbuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsSampler(uint id);
        internal static IsSampler glIsSampler;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsShader(uint shader);
        internal static IsShader glIsShader;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsSync(IntPtr sync);
        internal static IsSync glIsSync;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsTexture(uint texture);
        internal static IsTexture glIsTexture;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsTransformFeedback(uint id);
        internal static IsTransformFeedback glIsTransformFeedback;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool IsVertexArray(uint array);
        internal static IsVertexArray glIsVertexArray;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void LineWidth(float width);
        internal static LineWidth glLineWidth;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void LinkProgram(uint program);
        internal static LinkProgram glLinkProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void LogicOp(OpenGLInterop.LogicOp opcode);
        internal static LogicOp glLogicOp;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr MapBuffer(BufferTarget target, BufferAccess access);
        internal static MapBuffer glMapBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr MapNamedBuffer(uint buffer, BufferAccess access);
        internal static MapNamedBuffer glMapNamedBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr MapBufferRange(BufferTarget target, IntPtr offset, IntPtr length, BufferAccessMask access);
        internal static MapBufferRange glMapBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr MapNamedBufferRange(uint buffer, IntPtr offset, int length, uint access);
        internal static MapNamedBufferRange glMapNamedBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MemoryBarrier(uint barriers);
        internal static MemoryBarrier glMemoryBarrier;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MemoryBarrierByRegion(uint barriers);
        internal static MemoryBarrierByRegion glMemoryBarrierByRegion;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MinSampleShading(float value);
        internal static MinSampleShading glMinSampleShading;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MultiDrawArrays(BeginMode mode, int[] first, int[] count, int drawcount);
        internal static MultiDrawArrays glMultiDrawArrays;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MultiDrawArraysIndirect(BeginMode mode, IntPtr indirect, int drawcount, int stride);
        internal static MultiDrawArraysIndirect glMultiDrawArraysIndirect;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MultiDrawElements(BeginMode mode, int[] count, DrawElementsType type, IntPtr indices, int drawcount);
        internal static MultiDrawElements glMultiDrawElements;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MultiDrawElementsBaseVertex(BeginMode mode, int[] count, DrawElementsType type, IntPtr indices, int drawcount, int[] basevertex);
        internal static MultiDrawElementsBaseVertex glMultiDrawElementsBaseVertex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void MultiDrawElementsIndirect(BeginMode mode, DrawElementsType type, IntPtr indirect, int drawcount, int stride);
        internal static MultiDrawElementsIndirect glMultiDrawElementsIndirect;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ObjectLabel(OpenGLInterop.ObjectLabel identifier, uint name, int length, string label);
        internal static ObjectLabel glObjectLabel;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ObjectPtrLabel(IntPtr ptr, int length, string label);
        internal static ObjectPtrLabel glObjectPtrLabel;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PatchParameteri(int pname, int value);
        internal static PatchParameteri glPatchParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PatchParameterfv(int pname, float[] values);
        internal static PatchParameterfv glPatchParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PixelStoref(PixelStoreParameter pname, float param);
        internal static PixelStoref glPixelStoref;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PixelStorei(PixelStoreParameter pname, int param);
        internal static PixelStorei glPixelStorei;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PointParameterf(PointParameterName pname, float param);
        internal static PointParameterf glPointParameterf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PointParameteri(PointParameterName pname, int param);
        internal static PointParameteri glPointParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PointParameterfv(PointParameterName pname, float[] @params);
        internal static PointParameterfv glPointParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PointParameteriv(PointParameterName pname, int[] @params);
        internal static PointParameteriv glPointParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PointSize(float size);
        internal static PointSize glPointSize;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PolygonMode(MaterialFace face, OpenGLInterop.PolygonMode mode);
        internal static PolygonMode glPolygonMode;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PolygonOffset(float factor, float units);
        internal static PolygonOffset glPolygonOffset;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void PrimitiveRestartIndex(uint index);
        internal static PrimitiveRestartIndex glPrimitiveRestartIndex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramBinary(uint program, int binaryFormat, IntPtr binary, int length);
        internal static ProgramBinary glProgramBinary;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramParameteri(uint program, ProgramParameterPName pname, int value);
        internal static ProgramParameteri glProgramParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1f(uint program, int location, float v0);
        internal static ProgramUniform1f glProgramUniform1f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2f(uint program, int location, float v0, float v1);
        internal static ProgramUniform2f glProgramUniform2f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3f(uint program, int location, float v0, float v1, float v2);
        internal static ProgramUniform3f glProgramUniform3f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4f(uint program, int location, float v0, float v1, float v2, float v3);
        internal static ProgramUniform4f glProgramUniform4f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1i(uint program, int location, int v0);
        internal static ProgramUniform1i glProgramUniform1i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2i(uint program, int location, int v0, int v1);
        internal static ProgramUniform2i glProgramUniform2i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3i(uint program, int location, int v0, int v1, int v2);
        internal static ProgramUniform3i glProgramUniform3i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4i(uint program, int location, int v0, int v1, int v2, int v3);
        internal static ProgramUniform4i glProgramUniform4i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1ui(uint program, int location, uint v0);
        internal static ProgramUniform1ui glProgramUniform1ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2ui(uint program, int location, int v0, uint v1);
        internal static ProgramUniform2ui glProgramUniform2ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3ui(uint program, int location, int v0, int v1, uint v2);
        internal static ProgramUniform3ui glProgramUniform3ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4ui(uint program, int location, int v0, int v1, int v2, uint v3);
        internal static ProgramUniform4ui glProgramUniform4ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1fv(uint program, int location, int count, float[] value);
        internal static ProgramUniform1fv glProgramUniform1fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2fv(uint program, int location, int count, float[] value);
        internal static ProgramUniform2fv glProgramUniform2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3fv(uint program, int location, int count, float[] value);
        internal static ProgramUniform3fv glProgramUniform3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4fv(uint program, int location, int count, float[] value);
        internal static ProgramUniform4fv glProgramUniform4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1iv(uint program, int location, int count, int[] value);
        internal static ProgramUniform1iv glProgramUniform1iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2iv(uint program, int location, int count, int[] value);
        internal static ProgramUniform2iv glProgramUniform2iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3iv(uint program, int location, int count, int[] value);
        internal static ProgramUniform3iv glProgramUniform3iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4iv(uint program, int location, int count, int[] value);
        internal static ProgramUniform4iv glProgramUniform4iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform1uiv(uint program, int location, int count, uint[] value);
        internal static ProgramUniform1uiv glProgramUniform1uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform2uiv(uint program, int location, int count, uint[] value);
        internal static ProgramUniform2uiv glProgramUniform2uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform3uiv(uint program, int location, int count, uint[] value);
        internal static ProgramUniform3uiv glProgramUniform3uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniform4uiv(uint program, int location, int count, uint[] value);
        internal static ProgramUniform4uiv glProgramUniform4uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix2fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix2fv glProgramUniformMatrix2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix3fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix3fv glProgramUniformMatrix3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix4fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix4fv glProgramUniformMatrix4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix2x3fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix2x3fv glProgramUniformMatrix2x3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix3x2fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix3x2fv glProgramUniformMatrix3x2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix2x4fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix2x4fv glProgramUniformMatrix2x4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix4x2fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix4x2fv glProgramUniformMatrix4x2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix3x4fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix3x4fv glProgramUniformMatrix3x4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProgramUniformMatrix4x3fv(uint program, int location, int count, bool transpose, float[] value);
        internal static ProgramUniformMatrix4x3fv glProgramUniformMatrix4x3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ProvokingVertex(ProvokingVertexMode provokeMode);
        internal static ProvokingVertex glProvokingVertex;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void QueryCounter(uint id, QueryTarget target);
        internal static QueryCounter glQueryCounter;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ReadBuffer(ReadBufferMode mode);
        internal static ReadBuffer glReadBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedFramebufferReadBuffer(ReadBufferMode framebuffer, BeginMode mode);
        internal static NamedFramebufferReadBuffer glNamedFramebufferReadBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, int[] data);
        internal static ReadPixels glReadPixels;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ReadnPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, int bufSize, int[] data);
        internal static ReadnPixels glReadnPixels;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void RenderbufferStorage(RenderbufferTarget target, OpenGLInterop.RenderbufferStorage internalFormat, int width, int height);
        internal static RenderbufferStorage glRenderbufferStorage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedRenderbufferStorage(uint renderbuffer, OpenGLInterop.RenderbufferStorage internalFormat, int width, int height);
        internal static NamedRenderbufferStorage glNamedRenderbufferStorage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void RenderbufferStorageMultisample(RenderbufferTarget target, int samples, OpenGLInterop.RenderbufferStorage internalFormat, int width, int height);
        internal static RenderbufferStorageMultisample glRenderbufferStorageMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NamedRenderbufferStorageMultisample(uint renderbuffer, int samples, OpenGLInterop.RenderbufferStorage internalFormat, int width, int height);
        internal static NamedRenderbufferStorageMultisample glNamedRenderbufferStorageMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SampleCoverage(float value, bool invert);
        internal static SampleCoverage glSampleCoverage;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SampleMaski(uint maskNumber, uint mask);
        internal static SampleMaski glSampleMaski;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameterf(uint sampler, TextureParameterName pname, float param);
        internal static SamplerParameterf glSamplerParameterf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameteri(uint sampler, TextureParameterName pname, int param);
        internal static SamplerParameteri glSamplerParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameterfv(uint sampler, TextureParameterName pname, float[] @params);
        internal static SamplerParameterfv glSamplerParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameteriv(uint sampler, TextureParameterName pname, int[] @params);
        internal static SamplerParameteriv glSamplerParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameterIiv(uint sampler, TextureParameterName pname, int[] @params);
        internal static SamplerParameterIiv glSamplerParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void SamplerParameterIuiv(uint sampler, TextureParameterName pname, uint[] @params);
        internal static SamplerParameterIuiv glSamplerParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Scissor(int x, int y, int width, int height);
        internal static Scissor glScissor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ScissorArrayv(uint first, int count, int[] v);
        internal static ScissorArrayv glScissorArrayv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ScissorIndexed(uint index, int left, int bottom, int width, int height);
        internal static ScissorIndexed glScissorIndexed;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ScissorIndexedv(uint index, int[] v);
        internal static ScissorIndexedv glScissorIndexedv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ShaderBinary(int count, uint[] shaders, int binaryFormat, IntPtr binary, int length);
        internal static ShaderBinary glShaderBinary;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ShaderSource(uint shader, int count, string[] @string, int[] length);
        internal static ShaderSource glShaderSource;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ShaderStorageBlockBinding(uint program, uint storageBlockIndex, uint storageBlockBinding);
        internal static ShaderStorageBlockBinding glShaderStorageBlockBinding;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilFunc(StencilFunction func, int @ref, uint mask);
        internal static StencilFunc glStencilFunc;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilFuncSeparate(StencilFace face, StencilFunction func, int @ref, uint mask);
        internal static StencilFuncSeparate glStencilFuncSeparate;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilMask(uint mask);
        internal static StencilMask glStencilMask;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilMaskSeparate(StencilFace face, uint mask);
        internal static StencilMaskSeparate glStencilMaskSeparate;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilOp(OpenGLInterop.StencilOp sfail, OpenGLInterop.StencilOp dpfail, OpenGLInterop.StencilOp dppass);
        internal static StencilOp glStencilOp;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void StencilOpSeparate(StencilFace face, OpenGLInterop.StencilOp sfail, OpenGLInterop.StencilOp dpfail, OpenGLInterop.StencilOp dppass);
        internal static StencilOpSeparate glStencilOpSeparate;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexBuffer(TextureBufferTarget target, SizedInternalFormat internalFormat, uint buffer);
        internal static TexBuffer glTexBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureBuffer(uint texture, SizedInternalFormat internalFormat, uint buffer);
        internal static TextureBuffer glTextureBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexBufferRange(BufferTarget target, SizedInternalFormat internalFormat, uint buffer, IntPtr offset, IntPtr size);
        internal static TexBufferRange glTexBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureBufferRange(uint texture, SizedInternalFormat internalFormat, uint buffer, IntPtr offset, int size);
        internal static TextureBufferRange glTextureBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexImage1D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int border, PixelFormat format, PixelType type, IntPtr data);
        internal static TexImage1D glTexImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexImage2D(TextureTarget target, int level, int internalFormat, int width, int height, int border, int format, PixelType type, IntPtr data);
        internal static TexImage2D glTexImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexImage2DMultisample(TextureTargetMultisample target, int samples, PixelInternalFormat internalFormat, int width, int height, bool fixedsamplelocations);
        internal static TexImage2DMultisample glTexImage2DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexImage3D(TextureTarget target, int level, PixelInternalFormat internalFormat, int width, int height, int depth, int border, PixelFormat format, PixelType type, IntPtr data);
        internal static TexImage3D glTexImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexImage3DMultisample(TextureTargetMultisample target, int samples, PixelInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations);
        internal static TexImage3DMultisample glTexImage3DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameterf(TextureTarget target, TextureParameterName pname, float param);
        internal static TexParameterf glTexParameterf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameteri(TextureTarget target, TextureParameterName pname, int param);
        internal static TexParameteri glTexParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameterf(uint texture, TextureParameter pname, float param);
        internal static TextureParameterf glTextureParameterf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameteri(uint texture, TextureParameter pname, int param);
        internal static TextureParameteri glTextureParameteri;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameterfv(TextureTarget target, TextureParameterName pname, float[] @params);
        internal static TexParameterfv glTexParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameteriv(TextureTarget target, TextureParameterName pname, int[] @params);
        internal static TexParameteriv glTexParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameterIiv(TextureTarget target, TextureParameterName pname, int[] @params);
        internal static TexParameterIiv glTexParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexParameterIuiv(TextureTarget target, TextureParameterName pname, uint[] @params);
        internal static TexParameterIuiv glTexParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameterfv(uint texture, TextureParameter pname, float[] paramtexture);
        internal static TextureParameterfv glTextureParameterfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameteriv(uint texture, TextureParameter pname, int[] param);
        internal static TextureParameteriv glTextureParameteriv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameterIiv(uint texture, TextureParameter pname, int[] @params);
        internal static TextureParameterIiv glTextureParameterIiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureParameterIuiv(uint texture, TextureParameter pname, uint[] @params);
        internal static TextureParameterIuiv glTextureParameterIuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexStorage1D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width);
        internal static TexStorage1D glTexStorage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureStorage1D(uint texture, int levels, SizedInternalFormat internalFormat, int width);
        internal static TextureStorage1D glTextureStorage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexStorage2D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width, int height);
        internal static TexStorage2D glTexStorage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureStorage2D(uint texture, int levels, SizedInternalFormat internalFormat, int width, int height);
        internal static TextureStorage2D glTextureStorage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexStorage2DMultisample(TextureTarget target, int samples, SizedInternalFormat internalFormat, int width, int height, bool fixedsamplelocations);
        internal static TexStorage2DMultisample glTexStorage2DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureStorage2DMultisample(uint texture, int samples, SizedInternalFormat internalFormat, int width, int height, bool fixedsamplelocations);
        internal static TextureStorage2DMultisample glTextureStorage2DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexStorage3D(TextureTarget target, int levels, SizedInternalFormat internalFormat, int width, int height, int depth);
        internal static TexStorage3D glTexStorage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureStorage3D(uint texture, int levels, SizedInternalFormat internalFormat, int width, int height, int depth);
        internal static TextureStorage3D glTextureStorage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexStorage3DMultisample(TextureTarget target, int samples, SizedInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations);
        internal static TexStorage3DMultisample glTexStorage3DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureStorage3DMultisample(uint texture, int samples, SizedInternalFormat internalFormat, int width, int height, int depth, bool fixedsamplelocations);
        internal static TextureStorage3DMultisample glTextureStorage3DMultisample;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TexSubImage1D glTexSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureSubImage1D(uint texture, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TextureSubImage1D glTextureSubImage1D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TexSubImage2D glTexSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureSubImage2D(uint texture, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TextureSubImage2D glTextureSubImage2D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TexSubImage3D glTexSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureSubImage3D(uint texture, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr pixels);
        internal static TextureSubImage3D glTextureSubImage3D;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureBarrier();
        internal static TextureBarrier glTextureBarrier;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TextureView(uint texture, TextureTarget target, uint origtexture, PixelInternalFormat internalFormat, uint minlevel, uint numlevels, uint minlayer, uint numlayers);
        internal static TextureView glTextureView;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TransformFeedbackBufferBase(uint xfb, uint index, uint buffer);
        internal static TransformFeedbackBufferBase glTransformFeedbackBufferBase;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TransformFeedbackBufferRange(uint xfb, uint index, uint buffer, IntPtr offset, int size);
        internal static TransformFeedbackBufferRange glTransformFeedbackBufferRange;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void TransformFeedbackVaryings(uint program, int count, string[] varyings, TransformFeedbackMode bufferMode);
        internal static TransformFeedbackVaryings glTransformFeedbackVaryings;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1f(int location, float v0);
        internal static Uniform1f glUniform1f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2f(int location, float v0, float v1);
        internal static Uniform2f glUniform2f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3f(int location, float v0, float v1, float v2);
        internal static Uniform3f glUniform3f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4f(int location, float v0, float v1, float v2, float v3);
        internal static Uniform4f glUniform4f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1i(int location, int v0);
        internal static Uniform1i glUniform1i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2i(int location, int v0, int v1);
        internal static Uniform2i glUniform2i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3i(int location, int v0, int v1, int v2);
        internal static Uniform3i glUniform3i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4i(int location, int v0, int v1, int v2, int v3);
        internal static Uniform4i glUniform4i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1ui(int location, uint v0);
        internal static Uniform1ui glUniform1ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2ui(int location, uint v0, uint v1);
        internal static Uniform2ui glUniform2ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3ui(int location, uint v0, uint v1, uint v2);
        internal static Uniform3ui glUniform3ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        internal static Uniform4ui glUniform4ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1fv(int location, int count, float[] value);
        internal static Uniform1fv glUniform1fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2fv(int location, int count, float[] value);
        internal static Uniform2fv glUniform2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3fv(int location, int count, float[] value);
        internal static Uniform3fv glUniform3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4fv(int location, int count, float[] value);
        internal static Uniform4fv glUniform4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1iv(int location, int count, int[] value);
        internal static Uniform1iv glUniform1iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2iv(int location, int count, int[] value);
        internal static Uniform2iv glUniform2iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3iv(int location, int count, int[] value);
        internal static Uniform3iv glUniform3iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4iv(int location, int count, int[] value);
        internal static Uniform4iv glUniform4iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform1uiv(int location, int count, uint[] value);
        internal static Uniform1uiv glUniform1uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform2uiv(int location, int count, uint[] value);
        internal static Uniform2uiv glUniform2uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform3uiv(int location, int count, uint[] value);
        internal static Uniform3uiv glUniform3uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Uniform4uiv(int location, int count, uint[] value);
        internal static Uniform4uiv glUniform4uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix2fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix2fv glUniformMatrix2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix3fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix3fv glUniformMatrix3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix4fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix4fv glUniformMatrix4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix2x3fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix2x3fv glUniformMatrix2x3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix3x2fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix3x2fv glUniformMatrix3x2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix2x4fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix2x4fv glUniformMatrix2x4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix4x2fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix4x2fv glUniformMatrix4x2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix3x4fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix3x4fv glUniformMatrix3x4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformMatrix4x3fv(int location, int count, bool transpose, float[] value);
        internal static UniformMatrix4x3fv glUniformMatrix4x3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding);
        internal static UniformBlockBinding glUniformBlockBinding;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UniformSubroutinesuiv(ShaderType shadertype, int count, uint[] indices);
        internal static UniformSubroutinesuiv glUniformSubroutinesuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool UnmapBuffer(BufferTarget target);
        internal static UnmapBuffer glUnmapBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool UnmapNamedBuffer(uint buffer);
        internal static UnmapNamedBuffer glUnmapNamedBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UseProgram(uint program);
        internal static UseProgram glUseProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void UseProgramStages(uint pipeline, uint stages, uint program);
        internal static UseProgramStages glUseProgramStages;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ValidateProgram(uint program);
        internal static ValidateProgram glValidateProgram;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ValidateProgramPipeline(uint pipeline);
        internal static ValidateProgramPipeline glValidateProgramPipeline;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayElementBuffer(uint vaobj, uint buffer);
        internal static VertexArrayElementBuffer glVertexArrayElementBuffer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1f(uint index, float v0);
        internal static VertexAttrib1f glVertexAttrib1f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1s(uint index, short v0);
        internal static VertexAttrib1s glVertexAttrib1s;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1d(uint index, double v0);
        internal static VertexAttrib1d glVertexAttrib1d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI1i(uint index, int v0);
        internal static VertexAttribI1i glVertexAttribI1i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI1ui(uint index, uint v0);
        internal static VertexAttribI1ui glVertexAttribI1ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2f(uint index, float v0, float v1);
        internal static VertexAttrib2f glVertexAttrib2f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2s(uint index, short v0, short v1);
        internal static VertexAttrib2s glVertexAttrib2s;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2d(uint index, double v0, double v1);
        internal static VertexAttrib2d glVertexAttrib2d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI2i(uint index, int v0, int v1);
        internal static VertexAttribI2i glVertexAttribI2i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI2ui(uint index, uint v0, uint v1);
        internal static VertexAttribI2ui glVertexAttribI2ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3f(uint index, float v0, float v1, float v2);
        internal static VertexAttrib3f glVertexAttrib3f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3s(uint index, short v0, short v1, short v2);
        internal static VertexAttrib3s glVertexAttrib3s;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3d(uint index, double v0, double v1, double v2);
        internal static VertexAttrib3d glVertexAttrib3d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI3i(uint index, int v0, int v1, int v2);
        internal static VertexAttribI3i glVertexAttribI3i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI3ui(uint index, uint v0, uint v1, uint v2);
        internal static VertexAttribI3ui glVertexAttribI3ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4f(uint index, float v0, float v1, float v2, float v3);
        internal static VertexAttrib4f glVertexAttrib4f;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4s(uint index, short v0, short v1, short v2, short v3);
        internal static VertexAttrib4s glVertexAttrib4s;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4d(uint index, double v0, double v1, double v2, double v3);
        internal static VertexAttrib4d glVertexAttrib4d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nub(uint index, byte v0, byte v1, byte v2, byte v3);
        internal static VertexAttrib4Nub glVertexAttrib4Nub;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4i(uint index, int v0, int v1, int v2, int v3);
        internal static VertexAttribI4i glVertexAttribI4i;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4ui(uint index, uint v0, uint v1, uint v2, uint v3);
        internal static VertexAttribI4ui glVertexAttribI4ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL1d(uint index, double v0);
        internal static VertexAttribL1d glVertexAttribL1d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL2d(uint index, double v0, double v1);
        internal static VertexAttribL2d glVertexAttribL2d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL3d(uint index, double v0, double v1, double v2);
        internal static VertexAttribL3d glVertexAttribL3d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL4d(uint index, double v0, double v1, double v2, double v3);
        internal static VertexAttribL4d glVertexAttribL4d;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1fv(uint index, float[] v);
        internal static VertexAttrib1fv glVertexAttrib1fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1sv(uint index, short[] v);
        internal static VertexAttrib1sv glVertexAttrib1sv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib1dv(uint index, double[] v);
        internal static VertexAttrib1dv glVertexAttrib1dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI1iv(uint index, int[] v);
        internal static VertexAttribI1iv glVertexAttribI1iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI1uiv(uint index, uint[] v);
        internal static VertexAttribI1uiv glVertexAttribI1uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2fv(uint index, float[] v);
        internal static VertexAttrib2fv glVertexAttrib2fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2sv(uint index, short[] v);
        internal static VertexAttrib2sv glVertexAttrib2sv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib2dv(uint index, double[] v);
        internal static VertexAttrib2dv glVertexAttrib2dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI2iv(uint index, int[] v);
        internal static VertexAttribI2iv glVertexAttribI2iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI2uiv(uint index, uint[] v);
        internal static VertexAttribI2uiv glVertexAttribI2uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3fv(uint index, float[] v);
        internal static VertexAttrib3fv glVertexAttrib3fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3sv(uint index, short[] v);
        internal static VertexAttrib3sv glVertexAttrib3sv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib3dv(uint index, double[] v);
        internal static VertexAttrib3dv glVertexAttrib3dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI3iv(uint index, int[] v);
        internal static VertexAttribI3iv glVertexAttribI3iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI3uiv(uint index, uint[] v);
        internal static VertexAttribI3uiv glVertexAttribI3uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4fv(uint index, float[] v);
        internal static VertexAttrib4fv glVertexAttrib4fv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4sv(uint index, short[] v);
        internal static VertexAttrib4sv glVertexAttrib4sv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4dv(uint index, double[] v);
        internal static VertexAttrib4dv glVertexAttrib4dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4iv(uint index, int[] v);
        internal static VertexAttrib4iv glVertexAttrib4iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4bv(uint index, sbyte[] v);
        internal static VertexAttrib4bv glVertexAttrib4bv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4ubv(uint index, byte[] v);
        internal static VertexAttrib4ubv glVertexAttrib4ubv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4usv(uint index, ushort[] v);
        internal static VertexAttrib4usv glVertexAttrib4usv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4uiv(uint index, uint[] v);
        internal static VertexAttrib4uiv glVertexAttrib4uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nbv(uint index, sbyte[] v);
        internal static VertexAttrib4Nbv glVertexAttrib4Nbv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nsv(uint index, short[] v);
        internal static VertexAttrib4Nsv glVertexAttrib4Nsv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Niv(uint index, int[] v);
        internal static VertexAttrib4Niv glVertexAttrib4Niv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nubv(uint index, byte[] v);
        internal static VertexAttrib4Nubv glVertexAttrib4Nubv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nusv(uint index, ushort[] v);
        internal static VertexAttrib4Nusv glVertexAttrib4Nusv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttrib4Nuiv(uint index, uint[] v);
        internal static VertexAttrib4Nuiv glVertexAttrib4Nuiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4bv(uint index, sbyte[] v);
        internal static VertexAttribI4bv glVertexAttribI4bv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4ubv(uint index, byte[] v);
        internal static VertexAttribI4ubv glVertexAttribI4ubv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4sv(uint index, short[] v);
        internal static VertexAttribI4sv glVertexAttribI4sv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4usv(uint index, ushort[] v);
        internal static VertexAttribI4usv glVertexAttribI4usv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4iv(uint index, int[] v);
        internal static VertexAttribI4iv glVertexAttribI4iv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribI4uiv(uint index, uint[] v);
        internal static VertexAttribI4uiv glVertexAttribI4uiv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL1dv(uint index, double[] v);
        internal static VertexAttribL1dv glVertexAttribL1dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL2dv(uint index, double[] v);
        internal static VertexAttribL2dv glVertexAttribL2dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL3dv(uint index, double[] v);
        internal static VertexAttribL3dv glVertexAttribL3dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribL4dv(uint index, double[] v);
        internal static VertexAttribL4dv glVertexAttribL4dv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribP1ui(uint index, VertexAttribPType type, bool normalized, uint value);
        internal static VertexAttribP1ui glVertexAttribP1ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribP2ui(uint index, VertexAttribPType type, bool normalized, uint value);
        internal static VertexAttribP2ui glVertexAttribP2ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribP3ui(uint index, VertexAttribPType type, bool normalized, uint value);
        internal static VertexAttribP3ui glVertexAttribP3ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribP4ui(uint index, VertexAttribPType type, bool normalized, uint value);
        internal static VertexAttribP4ui glVertexAttribP4ui;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribBinding(uint attribindex, uint bindingindex);
        internal static VertexAttribBinding glVertexAttribBinding;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayAttribBinding(uint vaobj, uint attribindex, uint bindingindex);
        internal static VertexArrayAttribBinding glVertexArrayAttribBinding;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribDivisor(uint index, uint divisor);
        internal static VertexAttribDivisor glVertexAttribDivisor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribFormat(uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, bool normalized, uint relativeoffset);
        internal static VertexAttribFormat glVertexAttribFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribIFormat(uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, uint relativeoffset);
        internal static VertexAttribIFormat glVertexAttribIFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribLFormat(uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, uint relativeoffset);
        internal static VertexAttribLFormat glVertexAttribLFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayAttribFormat(uint vaobj, uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, bool normalized, uint relativeoffset);
        internal static VertexArrayAttribFormat glVertexArrayAttribFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayAttribIFormat(uint vaobj, uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, uint relativeoffset);
        internal static VertexArrayAttribIFormat glVertexArrayAttribIFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayAttribLFormat(uint vaobj, uint attribindex, int size, OpenGLInterop.VertexAttribFormat type, uint relativeoffset);
        internal static VertexArrayAttribLFormat glVertexArrayAttribLFormat;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer);
        internal static VertexAttribPointer glVertexAttribPointer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribIPointer(uint index, int size, VertexAttribPointerType type, int stride, IntPtr pointer);
        internal static VertexAttribIPointer glVertexAttribIPointer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexAttribLPointer(uint index, int size, VertexAttribPointerType type, int stride, IntPtr pointer);
        internal static VertexAttribLPointer glVertexAttribLPointer;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexBindingDivisor(uint bindingindex, uint divisor);
        internal static VertexBindingDivisor glVertexBindingDivisor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void VertexArrayBindingDivisor(uint vaobj, uint bindingindex, uint divisor);
        internal static VertexArrayBindingDivisor glVertexArrayBindingDivisor;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Viewport(int x, int y, int width, int height);
        internal static Viewport glViewport;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ViewportArrayv(uint first, int count, float[] v);
        internal static ViewportArrayv glViewportArrayv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ViewportIndexedf(uint index, float x, float y, float w, float h);
        internal static ViewportIndexedf glViewportIndexedf;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ViewportIndexedfv(uint index, float[] v);
        internal static ViewportIndexedfv glViewportIndexedfv;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void WaitSync(IntPtr sync, uint flags, ulong timeout);
        internal static WaitSync glWaitSync;

        public delegate void DebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void DebugMessageCallback(IntPtr callback, IntPtr userParam);
        internal static DebugMessageCallback glDebugMessageCallback;
        internal static DebugMessageCallback glDebugMessageCallbackKHR;

        // Extensions

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void SpecializeShader(uint shader, string entryPoint, uint numSpecializationConstants, IntPtr constantIndex, IntPtr constantValue);
        internal static SpecializeShader glSpecializeShader;
    }
}