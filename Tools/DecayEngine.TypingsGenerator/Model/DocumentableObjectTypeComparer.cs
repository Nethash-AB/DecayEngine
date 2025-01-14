using System;
using System.Collections.Generic;
using DecayEngine.TypingsGenerator.Model.Object;
using Enum = DecayEngine.TypingsGenerator.Model.Object.Enum;

namespace DecayEngine.TypingsGenerator.Model
{
    public class DocumentableObjectTypeComparer : Comparer<DocumentableObject>
    {
        private static readonly List<Type> TypeOrder = new List<Type>
        {
            typeof(Namespace),
            typeof(Interface),
            typeof(Class),
            typeof(Enum),
            typeof(Property),
            typeof(Field),
            typeof(Constructor),
            typeof(Method)
        };

        public override int Compare(DocumentableObject x, DocumentableObject y)
        {
            switch (x)
            {
                case null when y == null:
                    return 0;
                case null:
                    return -1;
            }

            if (y == null)
            {
                return 1;
            }

            Type xType = x.GetType();
            Type yType = y.GetType();
            if (xType == yType) return 0;

            int xTypeIndex = TypeOrder.IndexOf(xType);
            int yTypeIndex = TypeOrder.IndexOf(yType);

            if (xTypeIndex > yTypeIndex) return -1;
            if (xTypeIndex < yTypeIndex) return 1;

            return 0;
        }
    }
}