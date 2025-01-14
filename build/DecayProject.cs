using System.Diagnostics;

[DebuggerDisplay("Path = {" + nameof(Path) + "}, Type = {" + nameof(_type) + "}, BuildTarget = {" + nameof(_buildTarget) + "}")]
public class DecayProject
{
    private readonly DecayProjectType _type;
    private readonly DecayProjectBuildTarget _buildTarget;

    public string Path { get; }

    public bool IsDependency => _type == DecayProjectType.Dependency;
    public bool IsCoreRt => _type == DecayProjectType.CoreRt;
    public bool IsModule => _type == DecayProjectType.Module;
    public bool IsManaged => _type == DecayProjectType.Managed;
    public bool IsAndroidApp => _type == DecayProjectType.AndroidApp;
    public bool IsAndroidModule => _type == DecayProjectType.AndroidModule;

    public bool IsDemo => !IsTool && IsCoreRt || IsAndroidApp;

    public bool IsDesktop => _buildTarget.HasFlag(DecayProjectBuildTarget.Desktop);
    public bool IsAndroid => _buildTarget.HasFlag(DecayProjectBuildTarget.Android);
    public bool IsTool => _buildTarget.HasFlag(DecayProjectBuildTarget.Tool);

    public DecayProject(string path, DecayProjectBuildTarget buildTarget, DecayProjectType projectType)
    {
        Path = path;
        _buildTarget = buildTarget;
        _type = projectType;
    }
}