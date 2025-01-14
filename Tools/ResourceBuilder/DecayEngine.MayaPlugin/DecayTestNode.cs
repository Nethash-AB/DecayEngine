//using Autodesk.Maya.OpenMaya;
//using Autodesk.Maya.OpenMayaUI;
//
//[assembly: MPxNodeClass(typeof(DecayEngine.MayaPlugin.DecayTestNode), "decayTestNode", 0x00000001, NodeType = MPxNode.NodeType.kLocatorNode)]
//
//namespace DecayEngine.MayaPlugin
//{
//    public class DecayTestNode : MPxLocatorNode
//    {
//        [MPxNodeNumeric("in", "input", MFnNumericData.Type.kFloat, Storable = true)]
//        public static MObject Input;
//        [MPxNodeNumeric("out", "output", MFnNumericData.Type.kFloat, Storable = false, Writable = false)]
//        public static MObject Output;
//
//        public override bool compute(MPlug plug, MDataBlock dataBlock)
//        {
//            bool res = plug.attribute.equalEqual(Output);
//
//            if (!res)
//            {
//                MGlobal.displayInfo("NODE OUTPUT PLUG IS INVALID\n");
//                return false;
//            }
//
//            MDataHandle inputData = dataBlock.inputValue(Input);
//
//            MDataHandle outputHandle = dataBlock.outputValue(Output);
//            outputHandle.asFloat = 2 * inputData.asFloat;
//            dataBlock.setClean(plug);
//            return true;
//        }
//    }
//}