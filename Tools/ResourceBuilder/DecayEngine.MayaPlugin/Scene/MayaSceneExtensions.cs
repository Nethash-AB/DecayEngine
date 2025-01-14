using System.Text;

namespace DecayEngine.MayaPlugin.Scene
{
    public static class MayaSceneExtensions
    {
        public static void ToHierarchyString(this IMayaNode node, StringBuilder sb, int depth)
        {
            string indentation = "";
            for (int i = 0; i < depth; i++)
            {
                indentation += "  ";
            }

            sb.AppendLine($"{indentation}{node.DisplayId}");

            sb.AppendLine($"{indentation}{{");
            foreach (IMayaNode mayaSceneNode in node.Children)
            {
                mayaSceneNode.ToHierarchyString(sb, depth + 1);
            }
            sb.AppendLine($"{indentation}}}");
        }
    }
}