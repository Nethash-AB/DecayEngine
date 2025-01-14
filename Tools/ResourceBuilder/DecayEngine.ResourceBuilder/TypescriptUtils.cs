using System;
using System.Diagnostics;
using System.Text;

namespace DecayEngine.ResourceBuilder
{
    public static class TypescriptUtils
    {
        public static void InstallNpmModules(string projectPath)
        {
            Process proc = Process.Start(new ProcessStartInfo
            {
                FileName = "npm.cmd",
                Arguments = $"install",
                UseShellExecute = true,
                CreateNoWindow = true,
                WorkingDirectory = projectPath
            });

            if (proc == null)
            {
                throw new Exception($"Failed to start NPM. Is NPM installed and in PATH?");
            }

            proc.WaitForExit();
            if (proc.ExitCode > 0)
            {
                throw new Exception("Error executing NPM tasks.");
            }
        }

        public static string GetPackage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"name\": \"new-decay-engine-game\",");
            sb.AppendLine("  \"version\": \"1.0.0\",");
            sb.AppendLine("  \"description\": \"\",");
            sb.AppendLine("  \"author\": \"YourCompany\",");
            sb.AppendLine("  \"license\": \"ISC\",");
            sb.AppendLine("  \"dependencies\": {");
            sb.AppendLine("  },");
            sb.AppendLine("  \"devDependencies\": {");
            sb.AppendLine("    \"tslint\": \"^5.17.0\",");
            sb.AppendLine("    \"tslint-eslint-rules\": \"^5.4.0\",");
            sb.AppendLine("    \"typescript\": \"^3.5.1\"");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GetTsConfig()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"compileOnSave\": true,");
            sb.AppendLine("  \"compilerOptions\": {");
            sb.AppendLine("    \"target\": \"es6\",");
            sb.AppendLine("    \"module\": \"es6\",");
            sb.AppendLine("    \"sourceMap\": true,");
            sb.AppendLine("    \"inlineSourceMap\": true");
            sb.AppendLine("  },");
            sb.AppendLine("  \"include\": [");
            sb.AppendLine("    \"**/*.ts\"");
            sb.AppendLine("  ],");
            sb.AppendLine("  \"exclude\": [");
            sb.AppendLine("    \"node_modules\"");
            sb.AppendLine("  ]");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GetTsLint()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("    \"defaultSeverity\": \"error\",");
            sb.AppendLine("    \"extends\": [");
            sb.AppendLine("        \"tslint:recommended\",");
            sb.AppendLine("        \"tslint-eslint-rules\"");
            sb.AppendLine("    ],");
            sb.AppendLine("    \"linterOptions\": {");
            sb.AppendLine("      \"exclude\": [\"src/**/*.js\", \"src/**/*.d.ts\"]");
            sb.AppendLine("    },");
            sb.AppendLine("    \"jsRules\": {},");
            sb.AppendLine("    \"rules\": {");
            sb.AppendLine("      \"no-console\": [false],");
            sb.AppendLine("      \"prefer-const\": false,");
            sb.AppendLine("      \"max-line-length\": [true, 220],");
            sb.AppendLine("      \"no-namespace\": false,");
            sb.AppendLine("      \"variable-name\": [true, \"check-format\", \"allow-leading-underscore\", \"ban-keywords\"],");
            sb.AppendLine("      \"no-bitwise\": false");
            sb.AppendLine("    },");
            sb.AppendLine("    \"rulesDirectory\": []");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GetGitIgnore()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".idea/");
            sb.AppendLine("node_modules/");
            sb.AppendLine("**/*.js");
            sb.AppendLine("**/*.spv");
            sb.AppendLine("global.d.ts");

            return sb.ToString();
        }
    }
}