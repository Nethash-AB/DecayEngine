namespace DecayEngine.TypingsGenerator.Model.Value
{
    public class Return : ITypedObject
    {
        public string Description { get; }
        public string Type { get; set; }

        public Return(string description, string type)
        {
            Description = description;
            Type = type;
        }

        public override string ToString()
        {
            return $": {Type}";
        }
    }
}