using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC;
using DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderCSpvc;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Shader
{
    public class ShaderCompiler : IResourceCompiler<ShaderResource>
    {
        private static readonly string[] BrokenNonOpaqueTypes = {
            "bool",
            "int",
            "uint",
            "float",
            "double",
            "vec2",
            "vec3",
            "vec4",
            "bvec2",
            "bvec3",
            "bvec4",
            "ivec2",
            "ivec3",
            "ivec4",
            "uvec2",
            "uvec3",
            "uvec4",
            "dvec2",
            "dvec3",
            "dvec4",
            "mat2",
            "mat2x2",
            "mat2x3",
            "mat2x4",
            "mat3",
            "mat3x2",
            "mat3x3",
            "mat3x4",
            "mat4",
            "mat4x2",
            "mat4x3",
            "mat4x4"
        };

        public Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            if (!(resource is ShaderResource specificResource)) return null;
            return Compile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            if (!(resource is ShaderResource specificResource)) return null;
            return Decompile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Compile(ShaderResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream,
            out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            if (!resource.Compile) return sourceStream;

            string source;
            using (TextReader reader = new StreamReader(sourceStream, Encoding.UTF8))
            {
                source = reader.ReadToEnd();
                sourceStream.Position = 0;
            }

            if (string.IsNullOrEmpty(source))
            {
                return sourceStream;
            }

            MemoryStream ms = CompileSpirv(resource.Type, resource.Language, source, dataPointer().SourcePath);
            GenerateFallbacks(resource, ms);

            extraPointers.AddRange(resource.Fallbacks.Select(f => f.SourceByReference));

            ms.Position = 0;
            return ms;
        }

        public Stream Decompile(ShaderResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            if (!resource.Compile) return sourceStream;

            foreach (ShaderFallback fallback in resource.Fallbacks)
            {
                string tempFile = Path.GetTempFileName();
                DataPointer fallbackPointer = new DataPointer
                {
                    SourcePath = Path.Combine(Path.GetDirectoryName(resource.MetaFilePath), fallback.Source.SourcePath),
                    FullSourcePath = tempFile
                };

                using MemoryStream ms = fallback.Source.GetData();
                using FileStream fs = File.OpenWrite(tempFile);
                ms.CopyTo(fs);

                extraPointers.Add(fallbackPointer);
            }

            return DecompileSpirv(sourceStream, resource.Language, resource.Language);
        }

        private static void GenerateFallbacks(ShaderResource resource, Stream spirvStream)
        {
            resource.Fallbacks = new List<ShaderFallback>();
            List<ShaderLanguage> targets = new List<ShaderLanguage> {resource.Language};
            targets.AddRange(resource.FallbackTargets);

            string sourceName = Path.GetFileNameWithoutExtension(resource.Source.SourcePath);
            string sourceExtension = Path.GetExtension(resource.Source.SourcePath);
            foreach (ShaderLanguage fallbackLanguage in targets.Distinct())
            {
                string tempFile = Path.GetTempFileName();
                ShaderFallback shaderFallback = new ShaderFallback
                {
                    Language = fallbackLanguage,
                    Source = new DataPointer
                    {
                        SourcePath = $"./fallbacks/{sourceName}/{fallbackLanguage.ToString().ToLower()}{sourceExtension}",
                        FullSourcePath = tempFile
                    }
                };

                spirvStream.Position = 0;
                using MemoryStream ms = DecompileSpirv(spirvStream, resource.Language, fallbackLanguage);
                using FileStream fs = File.OpenWrite(shaderFallback.Source.FullSourcePath);
                ms.CopyTo(fs);

                resource.Fallbacks.Add(shaderFallback);
            }
        }

        private static MemoryStream CompileSpirv(ShaderType type, ShaderLanguage sourceLanguage, string source, string fileName)
        {
            IntPtr compiler = ShaderC.shaderc_compiler_initialize();

            ShaderCShaderKind kind = type switch
            {
                ShaderType.Vertex => ShaderCShaderKind.VertexShader,
                ShaderType.Geometry => ShaderCShaderKind.GeometryShader,
                ShaderType.Fragment => ShaderCShaderKind.FragmentShader,
                _ => throw new Exception($"Unknown shader type: {type}")
            };

            IntPtr options = ShaderC.shaderc_compile_options_initialize();
            switch (sourceLanguage)
            {
                case ShaderLanguage.Glsl:
                    ShaderC.shaderc_compile_options_set_source_language(options, ShaderCSourceLanguage.Glsl);
                    ShaderC.shaderc_compile_options_set_target_env(options, ShaderCTargetEnv.Opengl, ShaderCEnvVersion.Opengl45);
                    break;
                case ShaderLanguage.Hlsl:
                    ShaderC.shaderc_compile_options_set_source_language(options, ShaderCSourceLanguage.Hlsl);
                    ShaderC.shaderc_compile_options_set_target_env(options, ShaderCTargetEnv.Default, ShaderCEnvVersion.Vulkan11);
                    break;
                default:
                    ShaderC.shaderc_compile_options_set_source_language(options, ShaderCSourceLanguage.Glsl);
                    ShaderC.shaderc_compile_options_set_target_env(options, ShaderCTargetEnv.Default, ShaderCEnvVersion.Vulkan11);
                    break;
            }
            ShaderC.shaderc_compile_options_set_auto_bind_uniforms(options, true);

            IntPtr result = ShaderC.shaderc_compile_into_spv(compiler, source, source.Length, kind, Path.GetFileName(fileName),
                "main", options);

            ShaderCCompilationStatus status = ShaderC.shaderc_result_get_compilation_status(result);
            if (status != ShaderCCompilationStatus.Success)
            {
                int errorCount = ShaderC.shaderc_result_get_num_errors(result);
                IntPtr error = ShaderC.shaderc_result_get_error_message(result);
                string errorMessages = Marshal.PtrToStringAnsi(error);

                ShaderC.shaderc_compile_options_release(options);
                ShaderC.shaderc_result_release(result);
                ShaderC.shaderc_compiler_release(compiler);

                throw new Exception($"Shader compilation errors found ({errorCount}):\n{errorMessages}");
            }

            int warningCount = ShaderC.shaderc_result_get_num_warnings(result);
            Console.WriteLine($"Successfully compiled shader to SPIR-V. Warnings: {warningCount}");

            int outputLength = ShaderC.shaderc_result_get_length(result);
            byte[] output = new byte[outputLength];
            IntPtr outputPtr = ShaderC.shaderc_result_get_bytes(result);

            Marshal.Copy(outputPtr, output, 0, outputLength);

            MemoryStream ms = new MemoryStream();
            ms.Write(output, 0, output.Length);
            ms.Position = 0;

            ShaderC.shaderc_compile_options_release(options);
            ShaderC.shaderc_result_release(result);
            ShaderC.shaderc_compiler_release(compiler);

            return ms;
        }

        private static MemoryStream DecompileSpirv(Stream sourceStream, ShaderLanguage sourceLanguage, ShaderLanguage targetLanguage)
        {
            MemoryStream compiledStream = new MemoryStream();
            sourceStream.CopyTo(compiledStream);
            byte[] encodedCompiledShader = compiledStream.ToArray();
            uint[] compiledShader = new uint[encodedCompiledShader.Length / 4]; // 4 = sizeof(uint)

            long j = 0;
            for (long i = 0; i < encodedCompiledShader.Length; i += 4) // 4 = sizeof(uint)
            {
                byte[] numberBytes =
                {
                    encodedCompiledShader[i],
                    encodedCompiledShader[i + 1],
                    encodedCompiledShader[i + 2],
                    encodedCompiledShader[i + 3]
                };

                uint number = BitConverter.ToUInt32(numberBytes, 0);
                compiledShader[j] = number;
                j++;
            }

            IntPtr compiler = ShaderCSpvc.shaderc_spvc_compiler_initialize();

            IntPtr options = ShaderCSpvc.shaderc_spvc_compile_options_initialize();
            ShaderCSpvc.shaderc_spvc_compile_options_set_entry_point(options, "main");
            ShaderCSpvc.shaderc_spvc_compile_options_set_remove_unused_variables(options, false);
            ShaderCSpvc.shaderc_spvc_compile_options_set_vulkan_semantics(options, false);

            switch (sourceLanguage)
            {
                case ShaderLanguage.Glsl:
                case ShaderLanguage.GlslEs:
                    ShaderCSpvc.shaderc_spvc_compile_options_set_source_env(options, ShaderCTargetEnv.Opengl, ShaderCEnvVersion.Opengl45);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_target_env(options, ShaderCTargetEnv.Opengl, ShaderCEnvVersion.Opengl45);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_separate_shader_objects(options, false);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_flatten_ubo(options, false);
                    break;
                case ShaderLanguage.Hlsl:
                    ShaderCSpvc.shaderc_spvc_compile_options_set_source_env(options, ShaderCTargetEnv.Vulkan, ShaderCEnvVersion.Vulkan11);
                    break;
                default:
                    ShaderC.shaderc_compile_options_set_source_language(options, ShaderCSourceLanguage.Glsl);
                    break;
            }

            switch (targetLanguage)
            {
                case ShaderLanguage.Glsl:
                    ShaderCSpvc.shaderc_spvc_compile_options_set_glsl_language_version(options, 440);
                    break;
                case ShaderLanguage.GlslEs:
                    ShaderCSpvc.shaderc_spvc_compile_options_set_glsl_language_version(options, 310);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_es(options, true);
                    break;
                case ShaderLanguage.Hlsl:
                    ShaderCSpvc.shaderc_spvc_compile_options_set_hlsl_point_size_compat(options, true);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_hlsl_point_coord_compat(options, true);
                    ShaderCSpvc.shaderc_spvc_compile_options_set_hlsl_shader_model(options, 64);
                    break;
            }

            IntPtr result = targetLanguage == ShaderLanguage.Hlsl
                ? ShaderCSpvc.shaderc_spvc_compile_into_hlsl(compiler, compiledShader, compiledShader.Length, options)
                : ShaderCSpvc.shaderc_spvc_compile_into_glsl(compiler, compiledShader, compiledShader.Length, options);

            ShaderCCompilationStatus status = ShaderCSpvc.shaderc_spvc_result_get_status(result);
            if (status != ShaderCCompilationStatus.Success)
            {
                IntPtr error = ShaderCSpvc.shaderc_spvc_result_get_messages(result);
                string errorMessages = Marshal.PtrToStringAnsi(error);

                ShaderCSpvc.shaderc_spvc_compile_options_release(options);
                ShaderCSpvc.shaderc_spvc_result_release(result);
                ShaderCSpvc.shaderc_spvc_compiler_release(compiler);

                throw new Exception($"Shader decompilation errors found:\n{errorMessages}");
            }

            Console.WriteLine($"Successfully decompiled shader to {targetLanguage}.");

            IntPtr output = ShaderCSpvc.shaderc_spvc_result_get_output(result);
            string outputText = Marshal.PtrToStringAnsi(output);

            // GOOGLE PLEASE WHY U DO THIS, MAT4 IS NOT AN OPAQUE TYPE YOU HYPERCHROMOSOMIC BRAINLETS!!!!!!!
            outputText = BrokenNonOpaqueTypes.Aggregate(outputText, RemoveBindingFromNonOpaqueType);

            MemoryStream ms = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8, 1024, true))
            {
                writer.Write(outputText);
            }
            ms.Position = 0;

            ShaderCSpvc.shaderc_spvc_compile_options_release(options);
            ShaderCSpvc.shaderc_spvc_result_release(result);
            ShaderCSpvc.shaderc_spvc_compiler_release(compiler);

            return ms;
        }

        private static string RemoveBindingFromNonOpaqueType(string text, string opaqueType)
        {
            string cleanText = text;

            Match matchLast = Regex.Match(cleanText, $@"(,\s*binding\s*=\s*[0-9]*).*uniform\s*{opaqueType}");
            while (matchLast.Success)
            {
                cleanText = cleanText.Replace(
                    matchLast.Groups[0].Value,
                    matchLast.Groups[0].Value.Replace(matchLast.Groups[1].Value, "")
                );
                matchLast = matchLast.NextMatch();
            }

            Match matchFirst = Regex.Match(cleanText, $@"(,\s*binding\s*=\s*[0-9]*).*uniform\s*{opaqueType}");
            while (matchFirst.Success)
            {
                cleanText = cleanText.Replace(
                    matchFirst.Groups[0].Value,
                    matchFirst.Groups[0].Value.Replace(matchFirst.Groups[1].Value, "")
                );
                matchFirst = matchFirst.NextMatch();
            }

            Match matchSingle = Regex.Match(cleanText, $@"(,\s*binding\s*=\s*[0-9]*).*uniform\s*{opaqueType}");
            while (matchSingle.Success)
            {
                cleanText = cleanText.Replace(
                    matchSingle.Groups[0].Value,
                    matchSingle.Groups[0].Value.Replace(matchSingle.Groups[1].Value, "")
                );
                matchSingle = matchSingle.NextMatch();
            }

            return cleanText;
        }
    }
}