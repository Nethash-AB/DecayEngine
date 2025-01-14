using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;

namespace Parser
{
    public static class DecayProjectParser
    {
        public static DecayProject ParseProject(string projectFile)
        {
            MSBuildSettings settings = new MSBuildSettings()
                .DisableLogOutput()
                .SetProjectFile(projectFile)
                .SetVerbosity(MSBuildVerbosity.Diagnostic)
                .SetTargets(Guid.NewGuid().ToString());

            IProcess process = ProcessTasks.StartProcess(settings);
            process.AssertWaitForExit();

            string[] array = process.Output.Select(x => x.Text).ToArray();

            (DecayProjectBuildTarget buildTarget, DecayProjectType projectType) = ParseProperties(array);
            return buildTarget == DecayProjectBuildTarget.Invalid
                ? null
                : new DecayProject(projectFile, buildTarget, projectType);
        }

        private static (DecayProjectBuildTarget, DecayProjectType) ParseProperties(IEnumerable<string> lines)
        {
            List<string> propertyLines = lines
                .SkipWhile(x => x != "Initial Properties:").Skip(1)
                .TakeWhile(x => x != "Initial Items:").ToList();

            DecayProjectBuildTarget buildTarget = DecayProjectBuildTarget.Invalid;
            DecayProjectType projectType = DecayProjectType.Dependency;

            for (int i = 0; i < propertyLines.Count - 1; i++)
            {
                string line = propertyLines[i];

                // Matches "(PropertyName) = (PropertyValue),"
                Match match = Regex.Match(line, @"(\S*)(?:\s*=\s*(.*))(?<![,])");
                if (!match.Success) continue;

                string property = match.Groups[1].Value.Trim();
                string value = "";
                if (match.Groups.Count > 2)
                {
                    value = match.Groups[2].Value.Trim();
                }

                switch (property)
                {
                    case "DecayDesktop" when bool.Parse(value):
                        buildTarget |= DecayProjectBuildTarget.Desktop;
                        break;
                    case "DecayAndroid" when bool.Parse(value):
                        buildTarget |= DecayProjectBuildTarget.Android;
                        break;
                    case "DecayTool" when bool.Parse(value):
                        buildTarget |= DecayProjectBuildTarget.Tool;
                        break;
                    case "DecayProjectType" when value == "CoreRT":
                        projectType = DecayProjectType.CoreRt;
                        break;
                    case "DecayProjectType" when value == "Module":
                        projectType = DecayProjectType.Module;
                        break;
                    case "DecayProjectType" when value == "Managed":
                        projectType = DecayProjectType.Managed;
                        break;
                }
            }

            return (buildTarget, projectType);
        }
    }
}