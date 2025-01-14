using System;
using System.Collections.Generic;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Resource;

namespace DecayEngine.MayaPlugin.Scene
{
    public interface IMayaNode
    {
        MayaScene Scene { get; set; }
        MObject MayaObject { get; }
        List<IMayaNode> Children { get; }
        string DisplayId { get; }
        bool IsValid { get; }

        IEnumerable<IResource> ToDecayResource(Uri outputDirUri);
    }
}