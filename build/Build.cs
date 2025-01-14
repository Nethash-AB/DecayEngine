using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using Parameter;
using Parser;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.ControlFlow;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[VerbosityMapping(typeof(MSBuildVerbosity),
    Quiet = nameof(MSBuildVerbosity.Quiet),
    Minimal = nameof(MSBuildVerbosity.Minimal),
    Normal = nameof(MSBuildVerbosity.Normal),
    Verbose = nameof(MSBuildVerbosity.Detailed))]
public class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main()
    {
        if (Is64Bit) return Execute<Build>(x => x.Compile);

        Error("Decay Engine can only be built under a 64 bit host.");
        return -1;
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Target platform to build - Default is 'Desktop'")]
    public readonly TargetPlatforms TargetPlatforms = TargetPlatforms.All;

    [Solution]
    public readonly Solution Solution;

    private MSBuildToolsVersion _currentMsBuildVersion;

    private List<DecayProject> _projects = new List<DecayProject>();

    protected override void OnBuildInitialized()
    {
        if (_currentMsBuildVersion != null) return;

        _currentMsBuildVersion = new MSBuildToolsVersion();
        typeof(Enumeration)
            .GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic)?
            .SetValue(_currentMsBuildVersion, "Current");
    }

    private Target PreparseProjects => _ => _
        .Requires(() => TargetPlatforms != null && TargetPlatforms != TargetPlatforms.None)
        .Executes(() =>
        {
            Solution.AllProjects
                .AsParallel()
                .WithDegreeOfParallelism(50)
                .Select(p => DecayProjectParser.ParseProject(p.Path))
                .Where(p => p != null)
                .ForEach(_projects.Add);

            Success($"Successfully preparsed {_projects.Count} Decay Projects.");
        });

    private Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Solution.AllProjects.Where(p => p.Name != "_build").ForEach(p => p.Directory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory));
        });

    private Target Restore => _ => _
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Restore"));
        });

    private Target Compile => _ => _
        .Requires(() => TargetPlatforms != null && TargetPlatforms != TargetPlatforms.None)
        .DependsOn(Restore)
        .DependsOn(PreparseProjects)
        .Triggers(CompileModules)
        .Triggers(CompileTools)
        .Triggers(CompileDemos);

    private Target CompileModules => _ => _
        .Requires(() => TargetPlatforms != null && TargetPlatforms != TargetPlatforms.None)
        .DependsOn(Restore)
        .DependsOn(PreparseProjects)
        .Executes(() =>
        {
            List<DecayProject> builtProjects = new List<DecayProject>();

            if (TargetPlatforms.BuildDesktop)
            {
                foreach (DecayProject project in _projects.Where(p => p.IsDesktop && p.IsModule))
                {
                    if (builtProjects.Contains(project))
                    {
                        Info($"Project already built, skipping: {project.Path}.");
                        continue;
                    }

                    IReadOnlyCollection<Output> output =
                        MSBuild(s => s
                            .SetToolsVersion(_currentMsBuildVersion)
                            .SetTargetPath(project.Path)
                            .SetTargets("Build")
                            .SetConfiguration(Configuration)
                            .SetTargetPlatform(MSBuildTargetPlatform.x64)
                            .SetMaxCpuCount(Environment.ProcessorCount)
                            .SetNodeReuse(IsLocalBuild));

                    if (output.Any(o => o.Type == OutputType.Err))
                    {
                        Fail($"Failed to build Decay Module for Desktop, aborting: {project.Path}.");
                        return;
                    }

                    builtProjects.Add(project);
                }
            }

            if (TargetPlatforms.BuildAndroid)
            {
                foreach (DecayProject project in _projects.Where(p => p.IsAndroid && p.IsAndroidModule || p.IsModule))
                {
                    if (builtProjects.Contains(project))
                    {
                        Info($"Project already built, skipping: {project.Path}.");
                        continue;
                    }

                    IReadOnlyCollection<Output> output =
                        MSBuild(s => s
                            .SetToolsVersion(_currentMsBuildVersion)
                            .SetTargetPath(project.Path)
                            .SetTargets("Build")
                            .SetConfiguration(Configuration)
                            .SetTargetPlatform(MSBuildTargetPlatform.MSIL)
                            .SetMaxCpuCount(Environment.ProcessorCount)
                            .SetNodeReuse(IsLocalBuild));

                    if (output.Any(o => o.Type == OutputType.Err))
                    {
                        Fail($"Failed to build Decay Module for Android, aborting: {project.Path}.");
                        return;
                    }

                    builtProjects.Add(project);
                }
            }

            Success($"Successfully built {builtProjects.Count} Decay Modules.");
        });

    private Target CompileTools => _ => _
        .DependsOn(Restore)
        .DependsOn(CompileModules)
        .DependsOn(PreparseProjects)
        .Executes(() =>
        {
            List<DecayProject> builtProjects = new List<DecayProject>();

            foreach (DecayProject project in _projects.Where(p => p.IsTool))
            {
                if (builtProjects.Contains(project))
                {
                    Info($"Project already built, skipping: {project.Path}.");
                    continue;
                }

                IReadOnlyCollection<Output> output =
                    MSBuild(s => s
                        .SetToolsVersion(_currentMsBuildVersion)
                        .SetTargetPath(project.Path)
                        .SetTargets("Build")
                        .SetConfiguration(Configuration)
                        .SetTargetPlatform(MSBuildTargetPlatform.x64)
                        .SetMaxCpuCount(Environment.ProcessorCount)
                        .SetNodeReuse(IsLocalBuild));

                if (output.Any(o => o.Type == OutputType.Err))
                {
                    Fail($"Failed to build Decay Tool, aborting: {project.Path}.");
                    return;
                }

                builtProjects.Add(project);
            }

            Success($"Successfully built {builtProjects.Count} Decay Tools.");
        });

    private Target CompileDemos => _ => _
        .Requires(() => TargetPlatforms != null && TargetPlatforms != TargetPlatforms.None)
        .DependsOn(Restore)
        .DependsOn(CompileModules)
        .DependsOn(PreparseProjects)
        .Executes(() =>
        {
            List<DecayProject> builtProjects = new List<DecayProject>();

            if (TargetPlatforms.BuildDesktop)
            {
                foreach (DecayProject project in _projects.Where(p => p.IsDesktop && p.IsDemo))
                {
                    if (builtProjects.Contains(project))
                    {
                        Info($"Project already built, skipping: {project.Path}.");
                        continue;
                    }

                    IReadOnlyCollection<Output> output =
                        MSBuild(s => s
                            .SetToolsVersion(_currentMsBuildVersion)
                            .SetTargetPath(project.Path)
                            .SetTargets("Build")
                            .SetConfiguration(Configuration)
                            .SetTargetPlatform(MSBuildTargetPlatform.x64)
                            .SetMaxCpuCount(Environment.ProcessorCount)
                            .SetNodeReuse(IsLocalBuild));

                    if (output.Any(o => o.Type == OutputType.Err))
                    {
                        Fail($"Failed to build Decay Demo for Desktop, aborting: {project.Path}.");
                        return;
                    }

                    builtProjects.Add(project);
                }
            }

            if (TargetPlatforms.BuildAndroid)
            {
                foreach (DecayProject project in _projects.Where(p => p.IsAndroid && p.IsDemo))
                {
                    if (builtProjects.Contains(project))
                    {
                        Info($"Project already built, skipping: {project.Path}.");
                        continue;
                    }

                    IReadOnlyCollection<Output> output =
                        MSBuild(s => s
                            .SetToolsVersion(_currentMsBuildVersion)
                            .SetTargetPath(project.Path)
                            .SetTargets("Build")
                            .SetConfiguration(Configuration)
                            .SetTargetPlatform(MSBuildTargetPlatform.MSIL)
                            .SetMaxCpuCount(Environment.ProcessorCount)
                            .SetNodeReuse(IsLocalBuild));

                    if (output.Any(o => o.Type == OutputType.Err))
                    {
                        Fail($"Failed to build Decay Demo for Android, aborting: {project.Path}.");
                        return;
                    }

                    builtProjects.Add(project);
                }
            }

            Success($"Successfully built {builtProjects.Count} Decay Demos.");
        });
}
