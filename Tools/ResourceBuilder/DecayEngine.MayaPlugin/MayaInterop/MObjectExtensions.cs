using Autodesk.Maya.OpenMaya;

namespace DecayEngine.MayaPlugin.MayaInterop
{
    public static class MObjectExtensions
    {
        public static bool IsVisible(this MObject mayaObject)
        {
            MFnDagNode dagNodeFn = new MFnDagNode(mayaObject);

            MPlug visibilityPlug = dagNodeFn.findPlug("visibility");
            if (visibilityPlug.isNull)
            {
                MGlobal.displayError($"Error querying visibility for node: {dagNodeFn.name}.\n");
                return false;
            }

            bool isVisible = false;
            visibilityPlug.getValue(ref isVisible);

            return isVisible;
        }
    }
}