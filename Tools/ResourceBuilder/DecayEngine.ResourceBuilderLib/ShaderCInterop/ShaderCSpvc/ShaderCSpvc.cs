using System;
using System.Runtime.InteropServices;
using DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC;

namespace DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderCSpvc
{
    public class ShaderCSpvc
    {
        private const string DllName = "shaderc_spvc_shared";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compiler_initialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compiler_release(IntPtr compiler);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compile_options_initialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compile_options_clone(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_release(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_entry_point(IntPtr options, string entryPoint);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_remove_unused_variables(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_source_env(IntPtr options, ShaderCTargetEnv env, ShaderCEnvVersion version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_target_env(IntPtr options, ShaderCTargetEnv env, ShaderCEnvVersion version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_vulkan_semantics(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_separate_shader_objects(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_flatten_ubo(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_glsl_language_version(IntPtr options, uint version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_flatten_multidimensional_arrays(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_es(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_glsl_emit_push_constant_as_ubo(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_language_version(IntPtr options, uint version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_swizzle_texture_samples(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_platform(IntPtr options, ShaderCSpvcMslPlatform platform);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_pad_fragment_output(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_capture(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_domain_lower_left(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_argument_buffers(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_msl_discrete_descriptor_sets(IntPtr options, uint[] descriptors, int numDescriptors);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_hlsl_shader_model(IntPtr options, uint model);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_hlsl_point_size_compat(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_hlsl_point_coord_compat(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_fixup_clipspace(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_flip_vert_y(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_validate(IntPtr options, bool b);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_compile_options_set_for_fuzzing(IntPtr options, ushort[] data, int size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compile_into_glsl(IntPtr compiler, uint[] source, int sourceLen, IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compile_into_hlsl(IntPtr compiler, uint[] source, int sourceLen, IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_compile_into_msl(IntPtr compiler, uint[] source, int sourceLen, IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_spvc_result_release(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ShaderCCompilationStatus shaderc_spvc_result_get_status(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_result_get_messages(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_spvc_result_get_output(IntPtr result);
    }
}