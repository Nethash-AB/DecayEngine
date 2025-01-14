using System;
using System.Runtime.InteropServices;

namespace DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC
{
    public class ShaderC
    {
        private const string DllName = "shaderc_shared";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compiler_initialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compiler_release(IntPtr compiler);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compile_options_initialize();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compile_options_clone(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_release(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_add_macro_definition(IntPtr options, string name, int nameLength, string value, int valueLength);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_source_language(IntPtr options, ShaderCSourceLanguage lang);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_generate_debug_info(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_optimization_level(IntPtr options, ShaderCOptimizationLevel level);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_forced_version_profile(IntPtr options, int version, ShaderCProfile profile);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_include_callbacks(IntPtr options, IntPtr resolver, IntPtr resultReleaser, IntPtr userData);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_suppress_warnings(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_target_env(IntPtr options, ShaderCTargetEnv target, ShaderCEnvVersion version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_target_spirv(IntPtr options, ShaderCSpirvVersion version);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_warnings_as_errors(IntPtr options);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_limit(IntPtr options, ShaderCLimit limit, int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_auto_bind_uniforms(IntPtr options, bool autoBind);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_hlsl_io_mapping(IntPtr options, bool hlslIoMap);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_hlsl_offsets(IntPtr options, bool hlslOffsets);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_binding_base(IntPtr options, ShaderCUniformKind kind, uint @base);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_binding_base_for_stage(IntPtr options, ShaderCShaderKind shaderKind, ShaderCUniformKind kind, uint @base);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_auto_map_locations(IntPtr options, bool autoMap);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(IntPtr options, ShaderCShaderKind shaderKind, string reg, string set, string binding);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_hlsl_register_set_and_binding(IntPtr options, string reg, string set, string binding);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_hlsl_functionality1(IntPtr options, bool enable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_invert_y(IntPtr options, bool enable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_compile_options_set_nan_clamp(IntPtr options, bool enable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compile_into_spv(IntPtr compiler, string sourceText, int sourceTextSize, ShaderCShaderKind shaderKind, string inputFileName, string entryPointName, IntPtr additionalOptions);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compile_into_spv_assembly(IntPtr compiler, string sourceText, int sourceTextSize, ShaderCShaderKind shaderKind, string inputFileName, string entryPointName, IntPtr additionalOptions);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_compile_into_preprocessed_text(IntPtr compiler, string sourceText, int sourceTextSize, ShaderCShaderKind shaderKind, string inputFileName, string entryPointName, IntPtr additionalOptions);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_assemble_into_spv(IntPtr compiler, string sourceAssembly, int sourceAssemblySize, IntPtr additionalOptions);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_result_release(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int shaderc_result_get_length(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int shaderc_result_get_num_warnings(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int shaderc_result_get_num_errors(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ShaderCCompilationStatus shaderc_result_get_compilation_status(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_result_get_bytes(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr shaderc_result_get_error_message(IntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void shaderc_get_spv_version(uint version, uint revision);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool shaderc_parse_version_profile(string str, int version, ShaderCProfile profile);
    }
}