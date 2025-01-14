using System.Text;

namespace DecayEngine.TypingsGenerator.Model.Value
{
    public class Parameter : ITypedObject
    {
        public string Name { get; }
        public bool IsParams { get; }
        public string Description { get; }
        public string Type { get; set; }

        public Parameter(string name, string description, string type, bool isParams)
        {
            Name = name;
            Description = description;
            Type = type;
            IsParams = isParams;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (IsParams)
            {
                sb.Append("...");
            }

            sb.Append($"{Name}: {Type}");

            return sb.ToString();
        }
    }
}