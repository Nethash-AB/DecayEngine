using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.TypingsGenerator.Model;
using DecayEngine.TypingsGenerator.Model.Flags;
using DecayEngine.TypingsGenerator.Model.Object;
using DecayEngine.TypingsGenerator.Model.Value;
using Enum = DecayEngine.TypingsGenerator.Model.Object.Enum;

namespace DecayEngine.TypingsGenerator
{
    public class TypeReflector
    {
        private static readonly Dictionary<Type, string> TypeMap;

        static TypeReflector()
        {
            TypeMap = new Dictionary<Type, string>
            {
                [typeof(int)] = "number",
                [typeof(uint)] = "number",
                [typeof(long)] = "number",
                [typeof(ulong)] = "number",
                [typeof(ushort)] = "number",
                [typeof(decimal)] = "number",
                [typeof(float)] = "number",
                [typeof(double)] = "number",
                [typeof(byte)] = "number",
                [typeof(sbyte)] = "number",
                [typeof(string)] = "string",
                [typeof(char)] = "string",
                [typeof(bool)] = "boolean",
                [typeof(void)] = "void",
                [typeof(Delegate)] = "Function"
            };
        }

        private readonly List<(Type Type, string Name)> _knownTypes;
        private readonly List<(Type Type, Namespace NameSpace)> _knownNamespaces;
        private readonly List<DocumentableObject> _rootLevelObjects;
        private readonly Dictionary<Type, DocumentableObject> _reflectedObjects;

        public TypeReflector()
        {
            _knownTypes = new List<(Type Type, string Name)>();
            _knownNamespaces = new List<(Type Type, Namespace NameSpace)>();
            _rootLevelObjects = new List<DocumentableObject>();
            _reflectedObjects = new Dictionary<Type, DocumentableObject>();
        }

        private void FindTemplateTypes()
        {
            Console.WriteLine("\n(Stage 2/5) Finding reflectable types.");
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes().Where(type => !type.IsNested))
                {
                    ScriptExportNamespaceAttribute nameSpaceAttr = type.GetCustomAttribute<ScriptExportNamespaceAttribute>();
                    if (nameSpaceAttr != null)
                    {
                        Console.WriteLine($"Found reflectable type: {type.Name} => {nameSpaceAttr.Name}");
                        _knownTypes.Add((type, nameSpaceAttr.Name));
                    }

                    ScriptExportClassAttribute classAttr = type.GetCustomAttribute<ScriptExportClassAttribute>();
                    if (classAttr != null)
                    {
                        Console.WriteLine($"Found reflectable type: {type.Name} => {classAttr.Name}");
                        _knownTypes.Add((type, classAttr.Name));
                        continue;
                    }

                    ScriptExportInterfaceAttribute ifaceAttr = type.GetCustomAttribute<ScriptExportInterfaceAttribute>();
                    if (ifaceAttr != null)
                    {
                        Console.WriteLine($"Found reflectable type: {type.Name} => {ifaceAttr.Name}");
                        _knownTypes.Add((type, ifaceAttr.Name));
                        continue;
                    }

                    ScriptExportEnumAttribute enumAttr = type.GetCustomAttribute<ScriptExportEnumAttribute>();
                    if (enumAttr != null)
                    {
                        Console.WriteLine($"Found reflectable type: {type.Name} => {enumAttr.Name}");
                        _knownTypes.Add((type, enumAttr.Name));
                    }

                    ScriptExportStructAttribute structAttr = type.GetCustomAttribute<ScriptExportStructAttribute>();
                    if (structAttr != null)
                    {
                        Console.WriteLine($"Found reflectable type: {type.Name} => {structAttr.Name}");
                        _knownTypes.Add((type, structAttr.Name));
                    }
                }
            }
        }

        public IEnumerable<DocumentableObject> ReflectExports()
        {
            FindTemplateTypes();

            Console.WriteLine("\n(Stage 3/5) Reflecting types.");
            foreach ((Type type, string _) in _knownTypes)
            {
                if (type.IsClass)
                {
                    if (type.GetCustomAttribute<ScriptExportNamespaceAttribute>() != null)
                    {
                        ReflectNamespace(type);
                    }
                    else
                    {
                        ReflectClass(type, false, null);
                    }
                }
                else if (type.IsInterface)
                {
                    ReflectInterface(type, null);
                }
                else if (type.IsValueType)
                {
                    if (type.IsEnum)
                    {
                        ReflectEnum(type, null);
                    }
                    else
                    {
                        ReflectClass(type, true, null); // TS doesn't have structs, class is the closest to .NET structs.
                    }
                }
            }

            Console.WriteLine("\n(Stage 4/5) Unwinding references.");

            foreach (DocumentableObject rootObj in _rootLevelObjects)
            {
                UnwindReferences(rootObj);
            }

            return _rootLevelObjects;
        }

        private void UnwindReferences(DocumentableObject obj)
        {
            switch (obj)
            {
                case Class classObj:
                    UnwindClass(classObj);
                    break;
                case Constructor constructor:
                    constructor.Parameters.ForEach(UnwindTypedValue);
                    break;
                case Field field:
                    UnwindTypedValue(field);
                    break;
                case Method method:
                    method.Parameters.ForEach(UnwindTypedValue);
                    UnwindTypedValue(method.Return);
                    break;
                case Property property:
                    UnwindTypedValue(property);
                    break;
            }

            foreach (DocumentableObject child in obj.Children)
            {
                UnwindReferences(child);
            }
        }

        private void UnwindTypedValue(ITypedObject typedObject)
        {
            if (typedObject?.Type == null || typedObject.Type.Split('.').Length >= 2) return;

            string type = typedObject.Type;
            if (type.StartsWith("Array<"))
            {
                string unwoundType = UnwindArray(type);
                if (unwoundType == null) return;

                type = unwoundType;
            }
            else if (type.Contains(" | "))
            {
                string unwoundType = UnwindUnion(type);
                if (unwoundType == null) return;

                type = unwoundType;
            }
            else
            {
                DocumentableObject typeRealObj = FindObject(type);
                if (typeRealObj == null) return;

                type = typeRealObj.FullyQualifiedName;
            }

            Console.WriteLine($"Unwound reference {typedObject.Type} => {type}.");
            typedObject.Type = type;
        }

        private string UnwindArray(string type)
        {
            string t = type.Remove(0, "Array<".Length).TrimEnd('>');

            DocumentableObject typeRealObj = FindObject(t);
            return typeRealObj == null ? null : $"Array<{typeRealObj.FullyQualifiedName}>";
        }

        private string UnwindUnion(string type)
        {
            List<string> unwoundTypes = new List<string>();

            string[] types = type.Split(" | ");
            foreach (string t in types)
            {
                DocumentableObject typeRealObj = FindObject(t);
                if (typeRealObj == null) return null;

                unwoundTypes.Add(typeRealObj.FullyQualifiedName);
            }

            return string.Join(" | ", unwoundTypes);
        }

        private void UnwindClass(Class classObj)
        {
            if (classObj.BaseType != null && classObj.BaseType.Split('.').Length < 2)
            {
                DocumentableObject baseTypeRealObj = FindObject(classObj.BaseType);
                if (baseTypeRealObj != null)
                {
                    string fullName = baseTypeRealObj.FullyQualifiedName;
                    Console.WriteLine($"Unwound reference {classObj.BaseType} => {fullName}.");
                    classObj.BaseType = fullName;
                }
            }

            List<string> unwoundInterfaces = new List<string>();
            foreach (string implementedInterface in classObj.ImplementedInterfaces)
            {
                if (implementedInterface != null && implementedInterface.Split('.').Length < 2)
                {
                    DocumentableObject realInterfaceObj = FindObject(implementedInterface);
                    if (realInterfaceObj != null)
                    {
                        string fullName = realInterfaceObj.FullyQualifiedName;
                        Console.WriteLine($"Unwound reference {implementedInterface} => {fullName}.");
                        unwoundInterfaces.Add(fullName);
                    }
                    else
                    {
                        unwoundInterfaces.Add(implementedInterface);
                    }
                }
                else
                {
                    unwoundInterfaces.Add(implementedInterface);
                }
            }

            classObj.ImplementedInterfaces = unwoundInterfaces;
        }

        private DocumentableObject FindObject(string name)
        {
            foreach (DocumentableObject obj in _rootLevelObjects)
            {
                DocumentableObject foundObj = FindObject(obj, name);
                if (foundObj != null) return foundObj;
            }

            return null;
        }

        private static DocumentableObject FindObject(DocumentableObject obj, string name)
        {
            if (obj.Name == name)
            {
                return obj;
            }

            foreach (DocumentableObject child in obj.Children)
            {
                DocumentableObject foundObj = FindObject(child, name);
                if (foundObj != null) return foundObj;
            }

            return null;
        }

        private Namespace ReflectNamespace(Type type, DocumentableObject parent = null)
        {
            Namespace nameSpace = _knownNamespaces.Where(pair => pair.Type == type).Select(pair => pair.NameSpace).FirstOrDefault();
            ScriptExportNamespaceAttribute nameSpaceAttr = type.GetCustomAttribute<ScriptExportNamespaceAttribute>();
            if (nameSpaceAttr == null || !type.IsClass || !(type.IsAbstract && type.IsSealed)) return null;

            if (nameSpace == null)
            {
                nameSpace = new Namespace(nameSpaceAttr.Name, nameSpaceAttr.Description,
                    nameSpaceAttr.NameSpace == null && parent == null, nameSpaceAttr.OverrideExisting);
                _knownNamespaces.Add((type, nameSpace));

                foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    ReflectMethod(methodInfo, nameSpace);
                }

                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    ReflectField(fieldInfo, nameSpace);
                }

                foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    ReflectProperty(propertyInfo, nameSpace);
                }

                foreach (DocumentableObject child in nameSpace.Children)
                {
                    child.Parent = nameSpace;
                }
            }

            if (parent != null)
            {
                if (!parent.Children.Contains(nameSpace))
                {
                    parent.Children.Add(nameSpace);
                }
            }
            else
            {
                if (nameSpaceAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(nameSpaceAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(nameSpace))
                    {
                        parentNameSpace.Children.Add(nameSpace);
                        nameSpace.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(nameSpace))
                    {
                        _rootLevelObjects.Add(nameSpace);
                    }
                }
            }

            return nameSpace;
        }

        private Enum ReflectEnum(Type type, DocumentableObject parent)
        {
            if (_reflectedObjects.ContainsKey(type)) return _reflectedObjects[type] as Enum;

            ScriptExportEnumAttribute enumAttr = type.GetCustomAttribute<ScriptExportEnumAttribute>();
            if (enumAttr == null) return null;

            Type underlyingType = type.GetEnumUnderlyingType();
            Enum enumObj = new Enum(enumAttr.Name, enumAttr.Description, enumAttr.NameSpace == null && parent == null,
                ReflectType(enumAttr.TypeOverride ?? underlyingType));

            foreach (FieldInfo fieldInfo in type.GetTypeInfo().DeclaredFields)
            {
                EnumValue enumValue = ReflectEnumValue(fieldInfo, underlyingType);
                if (enumValue != null)
                {
                    Console.WriteLine($"Reflected enum value: {fieldInfo} => {enumValue.Name}");
                    enumObj.Values.Add(enumValue);
                }
            }

            if (parent != null)
            {
                if (!parent.Children.Contains(enumObj))
                {
                    parent.Children.Add(enumObj);
                }
            }
            else
            {
                if (enumAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(enumAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(enumObj))
                    {
                        parentNameSpace.Children.Add(enumObj);
                        enumObj.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(enumObj))
                    {
                        _rootLevelObjects.Add(enumObj);
                    }
                }
            }

            _reflectedObjects[type] = enumObj;

            Console.Write($"Reflected enum: {type} => {enumObj.Name}");
            if (enumAttr.TypeOverride != null)
            {
                Console.Write($" | Type Override: {enumAttr.TypeOverride.Namespace}.{enumAttr.TypeOverride.Name}");
            }
            Console.Write("\n");

            return enumObj;
        }

        private EnumValue ReflectEnumValue(FieldInfo fieldInfo, Type underlyingType)
        {
            ScriptExportFieldAttribute fieldAttr = fieldInfo.GetCustomAttribute<ScriptExportFieldAttribute>();
            return fieldAttr == null
                ? null
                : new EnumValue(fieldInfo.Name, fieldAttr.Description,
                    ReflectType(fieldAttr.TypeOverride ?? underlyingType), Convert.ChangeType(fieldInfo.GetValue(null), underlyingType).ToString());
        }

        private Interface ReflectInterface(Type type, DocumentableObject parent)
        {
            if (_reflectedObjects.ContainsKey(type)) return _reflectedObjects[type] as Interface;

            ScriptExportInterfaceAttribute ifaceAttr = type.GetCustomAttribute<ScriptExportInterfaceAttribute>();
            if (ifaceAttr == null) return null;

            Interface iface = new Interface(ifaceAttr.Name, ifaceAttr.Description, ifaceAttr.NameSpace == null && parent == null);

            Type[] allInterfaces = type.GetInterfaces();
            foreach (Type subIfaceType in allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())))
            {
                string subIfaceName = _knownTypes
                   .Where(x => x.Type == (subIfaceType.IsGenericType ? subIfaceType.GetGenericTypeDefinition() : subIfaceType))
                   .Select(x => x.Name)
                   .FirstOrDefault();
                if (subIfaceName == null) continue;

                iface.ImplementedInterfaces.Add(subIfaceName);
            }

            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectMethod(methodInfo, iface);
            }

            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectField(fieldInfo, iface);
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectProperty(propertyInfo, iface);
            }

            if (parent != null)
            {
                if (!parent.Children.Contains(iface))
                {
                    parent.Children.Add(iface);
                }
            }
            else
            {
                if (ifaceAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(ifaceAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(iface))
                    {
                        parentNameSpace.Children.Add(iface);
                        iface.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(iface))
                    {
                        _rootLevelObjects.Add(iface);
                    }
                }
            }

            _reflectedObjects[type] = iface;

            Console.WriteLine($"Reflected interface: {type} => {iface.Name}\n");

            return iface;
        }

        private Class ReflectClass(Type type, bool isStruct, DocumentableObject parent)
        {
            if (_reflectedObjects.ContainsKey(type)) return _reflectedObjects[type] as Class;

            bool isStatic = type.IsAbstract && type.IsSealed; // Static classes are abstract AND sealed at IL level.
            Class classObj;
            Type nameSpace;

            if (isStruct)
            {
                isStatic = false;

                ScriptExportStructAttribute structAttr = type.GetCustomAttribute<ScriptExportStructAttribute>();
                if (structAttr == null) return null;

                nameSpace = structAttr.NameSpace;
                classObj = new Class(structAttr.Name, structAttr.Description, nameSpace == null && parent == null, ClassFlags.Struct, null);
            }
            else
            {
                ScriptExportClassAttribute classAttr = type.GetCustomAttribute<ScriptExportClassAttribute>();
                if (classAttr == null) return null;

                string baseClass = _knownTypes
                    .Where(x => x.Type == (type.BaseType.IsGenericType ? type.BaseType.GetGenericTypeDefinition() : type.BaseType))
                    .Select(x => x.Name)
                    .FirstOrDefault();

                ClassFlags flags = ClassFlags.None;
                if (isStatic)
                {
                    flags |= ClassFlags.Static;
                }
                else
                {
                    if (type.IsAbstract) flags |= ClassFlags.Abstract;
                }

                nameSpace = classAttr.NameSpace;

                string name = classAttr.Name;
                if (type.IsGenericType)
                {
                    Type eventDelegateType = type.GetGenericArguments()[0];
                    if (eventDelegateType != null
                        && typeof(Delegate).IsAssignableFrom(eventDelegateType)
                        && typeof(EventExport<>).MakeGenericType(type.GetGenericArguments()[0]).IsAssignableFrom(type)
                    )
                    {
                        name = $"{name}<TDelegate extends Function>";
                    }
                }

                classObj = new Class(name, classAttr.Description, nameSpace == null && parent == null, flags, baseClass);
            }

            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectMethod(methodInfo, classObj);
            }

            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectField(fieldInfo, classObj);
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                ReflectProperty(propertyInfo, classObj);
            }

            if (!isStatic)
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (constructors.Length > 0)
                {
                    foreach (ConstructorInfo constructorInfo in constructors)
                    {
                        Constructor constructor = ReflectConstructor(constructorInfo);
                        if (constructor != null)
                        {
                            Console.WriteLine($"Reflected constructor: {constructorInfo} => {constructor.Name}");
                            classObj.Children.Add(constructor);
                        }
                    }
                }
                else
                {
                    if (!classObj.Flags.HasFlag(ClassFlags.Abstract))
                    {
                        classObj.Flags |= ClassFlags.Abstract;
                    }
                }

                if (!isStruct)
                {
                    Type[] allInterfaces = type.GetInterfaces();
                    List<Type> directInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).ToList();

                    foreach (Type ifaceType in type.GetInterfaces())
                    {
                        bool isExportedInterface = _knownTypes
                            .Where(x => x.Type == (ifaceType.IsGenericType ? ifaceType.GetGenericTypeDefinition() : ifaceType))
                            .Select(x => x.Name)
                            .FirstOrDefault() != null;
                        if (!isExportedInterface) continue;

                        Interface iface = ReflectInterface(ifaceType, null);
                        foreach (DocumentableObject child in iface.Children)
                        {
                            DocumentableObject existingChild = classObj.Children.FirstOrDefault(childObj => childObj.Name == child.Name);
                            if (existingChild != null)
                            {
                                switch (existingChild)
                                {
                                    case Field field1 when child is Field field2 && field1.Type == field2.Type:
                                    case Property property1 when child is Property property2 && property1.Type == property2.Type:
                                    case Function func1 when child is Function func2 && func1.Parameters.SequenceEqual(func2.Parameters):
                                        continue;
                                }
                            }

                            DocumentableObject childClone = (DocumentableObject) child.Clone();
                            childClone.RequiresAccessModifier = true;
                            classObj.Children.Add(childClone);
                        }

                        if (directInterfaces.Contains(ifaceType) && !type.BaseType.GetInterfaces().Contains(ifaceType))
                        {
                            classObj.ImplementedInterfaces.Add(iface.Name);
                        }
                    }
                }
            }

            foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (nestedType.IsClass)
                {
                    Class childClass = ReflectClass(nestedType, false, classObj);
                    if (childClass != null)
                    {
                        childClass.Parent = classObj;
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsEnum)
                    {
                        Enum childEnum = ReflectEnum(type, classObj);
                        if (childEnum != null)
                        {
                            childEnum.Parent = classObj;
                        }
                    }
                    else
                    {
                        Class childStruct = ReflectClass(type, true, classObj); // TS doesn't have structs, class is the closest to .NET structs.
                        if (childStruct != null)
                        {
                            childStruct.Parent = classObj;
                        }
                    }
                }
            }

            if (parent != null)
            {
                if (!parent.Children.Contains(classObj))
                {
                    parent.Children.Add(classObj);
                }
            }
            else
            {
                if (nameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(nameSpace);
                    if (!parentNameSpace.Children.Contains(classObj))
                    {
                        parentNameSpace.Children.Add(classObj);
                        classObj.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(classObj))
                    {
                        _rootLevelObjects.Add(classObj);
                    }
                }
            }

            _reflectedObjects[type] = classObj;

            Console.WriteLine(isStruct ? $"Reflected struct: {type} => {classObj.Name}\n" : $"Reflected class: {type} => {classObj.Name}\n");

            return classObj;
        }

        private Constructor ReflectConstructor(MethodBase constructorInfo)
        {
            ScriptExportConstructor constructorAttr = constructorInfo.GetCustomAttribute<ScriptExportConstructor>();
            if (constructorAttr == null) return null;

            Constructor constructor = new Constructor(constructorAttr.Description);

            foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
            {
                Parameter parameter = ReflectParameter(parameterInfo);
                if (parameter == null)
                {
                    constructor = null;
                    break;
                }
                constructor.Parameters.Add(parameter);
            }

            return constructor;
        }

        private void ReflectMethod(MethodInfo methodInfo, DocumentableObject parent)
        {
            ScriptExportMethodAttribute methodAttr = methodInfo.GetCustomAttribute<ScriptExportMethodAttribute>();
            if (methodAttr == null) return;

            Return ret = ReflectReturn(methodInfo.ReturnParameter);
            if (ret == null) return;

            Method method = new Method(methodInfo.Name, methodAttr.Description, methodAttr.NameSpace == null && parent == null,
                parent != null && !(parent is Interface), methodInfo.IsStatic, ret);

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                Parameter parameter = ReflectParameter(parameterInfo);
                if (parameter == null)
                {
                    return;
                }
                method.Parameters.Add(parameter);
            }

            if (parent != null)
            {
                if (!parent.Children.Contains(method))
                {
                    parent.Children.Add(method);
                }
            }
            else
            {
                if (methodAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(methodAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(method))
                    {
                        parentNameSpace.Children.Add(method);
                        method.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(method))
                    {
                        _rootLevelObjects.Add(method);
                    }
                }
            }

            Console.WriteLine($"Reflected method: {methodInfo} => {method.Name}");
        }

        private Parameter ReflectParameter(ParameterInfo parameterInfo)
        {
            ScriptExportParameterAttribute parameterAttr = parameterInfo.GetCustomAttribute<ScriptExportParameterAttribute>();
            Type parameterType = parameterInfo.ParameterType;

            bool isParams = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
            if (isParams)
            {
                parameterType = GetEnumerableOf(parameterType);
            }

            if (parameterAttr == null) return null;

            string resolvedType;
            if (parameterAttr.TypeUnionOverride != null && parameterAttr.TypeUnionOverride.Length > 0)
            {
                resolvedType = ReflectUnion(parameterAttr.TypeUnionOverride);
            }
            else if (parameterAttr.TypeOverride != null)
            {
                resolvedType = ReflectType(parameterAttr.TypeOverride);
            }
            else
            {
                resolvedType = ReflectType(parameterType);
            }

            return new Parameter(parameterInfo.Name.ToCamelCase(), parameterAttr.Description, resolvedType, isParams);
        }

        private Return ReflectReturn(ParameterInfo returnInfo)
        {
            if (returnInfo.ParameterType == typeof(void))
            {
                return new Return(null, "void");
            }

            ScriptExportReturnAttribute returnAttr = returnInfo.GetCustomAttribute<ScriptExportReturnAttribute>();

            if (returnAttr == null) return null;

            string resolvedType;
            if (returnAttr.TypeUnionOverride != null && returnAttr.TypeUnionOverride.Length > 0)
            {
                resolvedType = ReflectUnion(returnAttr.TypeUnionOverride);
            }
            else if (returnAttr.TypeOverride != null)
            {
                resolvedType = ReflectType(returnAttr.TypeOverride);
            }
            else
            {
                resolvedType = ReflectType(returnInfo.ParameterType);
            }

            return new Return(returnAttr.Description, resolvedType);
        }

        private void ReflectField(FieldInfo fieldInfo, DocumentableObject parent)
        {
            ScriptExportFieldAttribute fieldAttr = fieldInfo.GetCustomAttribute<ScriptExportFieldAttribute>();
            if (fieldAttr == null) return;

            string resolvedType;
            if (fieldAttr.TypeUnionOverride != null && fieldAttr.TypeUnionOverride.Length > 0)
            {
                resolvedType = ReflectUnion(fieldAttr.TypeUnionOverride);
            }
            else if (fieldAttr.TypeOverride != null)
            {
                resolvedType = ReflectType(fieldAttr.TypeOverride);
            }
            else
            {
                resolvedType = ReflectType(fieldInfo.FieldType);
            }

            Field field = new Field(fieldInfo.Name, fieldAttr.Description, fieldAttr.NameSpace == null && parent == null,
                parent != null && !(parent is Interface), resolvedType, fieldInfo.IsStatic);

            if (parent != null)
            {
                if (!parent.Children.Contains(field))
                {
                    parent.Children.Add(field);
                }
            }
            else
            {
                if (fieldAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(fieldAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(field))
                    {
                        parentNameSpace.Children.Add(field);
                        field.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(field))
                    {
                        _rootLevelObjects.Add(field);
                    }
                }
            }

            Console.Write($"Reflected field: {fieldInfo} => {field.Name}");
            if (fieldAttr.TypeOverride != null)
            {
                Console.Write($" | Type Override: {fieldAttr.TypeOverride.Namespace}.{fieldAttr.TypeOverride.Name}");
            }
            else if (fieldAttr.TypeUnionOverride != null)
            {
                Console.Write(" | Type Union Override: " +
                              $"{string.Join(" | ", fieldAttr.TypeUnionOverride.Select(t => $"{t.Namespace}.{t.Name}"))}");
            }
            Console.Write("\n");
        }

        private void ReflectProperty(PropertyInfo propertyInfo, DocumentableObject parent)
        {
            ScriptExportPropertyAttribute propertyAttr = propertyInfo.GetCustomAttribute<ScriptExportPropertyAttribute>();
            bool isStatic = propertyInfo.CanRead ? propertyInfo.GetGetMethod().IsStatic : propertyInfo.CanWrite && propertyInfo.GetSetMethod().IsStatic;
            if(propertyAttr == null) return;

            string resolvedType;
            if (propertyAttr.TypeUnionOverride != null && propertyAttr.TypeUnionOverride.Length > 0)
            {
                resolvedType = ReflectUnion(propertyAttr.TypeUnionOverride);
            }
            else if (propertyAttr.TypeOverride != null)
            {
                resolvedType = ReflectType(propertyAttr.TypeOverride);
            }
            else
            {
                resolvedType = ReflectType(propertyInfo.PropertyType);
            }

            Property property = new Property(propertyInfo.Name, propertyAttr.Description, propertyAttr.NameSpace == null && parent == null,
                parent != null && !(parent is Interface),
                resolvedType, isStatic,
                propertyInfo.CanRead, propertyInfo.CanWrite);

            if (parent != null)
            {
                if (!parent.Children.Contains(property))
                {
                    parent.Children.Add(property);
                }
            }
            else
            {
                if (propertyAttr.NameSpace != null)
                {
                    Namespace parentNameSpace = ReflectNamespace(propertyAttr.NameSpace);
                    if (!parentNameSpace.Children.Contains(property))
                    {
                        parentNameSpace.Children.Add(property);
                        property.Parent = parentNameSpace;
                    }
                }
                else
                {
                    if (!_rootLevelObjects.Contains(property))
                    {
                        _rootLevelObjects.Add(property);
                    }
                }
            }

            Console.Write($"Reflected property: {propertyInfo} => {property.Name}");
            if (propertyAttr.TypeOverride != null)
            {
                Console.Write($" | Type Override: {propertyAttr.TypeOverride.Namespace}.{propertyAttr.TypeOverride.Name}");
            }
            else if (propertyAttr.TypeUnionOverride != null)
            {
                Console.Write(" | Type Union Override: " +
                              $"{string.Join(" | ", propertyAttr.TypeUnionOverride.Select(t => $"{t.Namespace}.{t.Name}"))}");
            }
            Console.Write("\n");
        }

        private string ReflectUnion(IEnumerable<Type> typeUnion)
        {
            List<string> resolvedTypes = new List<string>();
            foreach (Type type in typeUnion)
            {
                string resolvedType = ReflectType(type);
                if (resolvedType == "any")
                {
                    resolvedTypes.Clear();
                    resolvedTypes.Add("any");
                    break;
                }

                resolvedTypes.Add(resolvedType);
            }

            return string.Join(" | ", resolvedTypes);
        }

        private string ReflectType(Type type)
        {
            if (type.IsGenericType)
            {
                Type eventDelegateType = type.GetGenericArguments()[0];
                if (eventDelegateType != null
                    && typeof(Delegate).IsAssignableFrom(eventDelegateType)
                    && typeof(EventExport<>).MakeGenericType(type.GetGenericArguments()[0]).IsAssignableFrom(type)
                )
                {
                    MethodInfo methodInfo = eventDelegateType.GetMethod(nameof(MethodInfo.Invoke));
                    List<Tuple<string, string>> eventSignature = methodInfo
                        .GetParameters()
                        .Select(parameterInfo => new Tuple<string, string>(parameterInfo.Name, ReflectType(parameterInfo.ParameterType)))
                        .ToList();

                    StringBuilder sb = new StringBuilder();
                    sb.Append("(");
                    int i = 0;
                    foreach ((string eventArgumentName, string eventArgumentType) in eventSignature)
                    {
                        sb.Append($"{eventArgumentName}: {eventArgumentType}");

                        if (i < eventSignature.Count - 1)
                        {
                            sb.Append(", ");
                        }
                        i++;
                    }
                    sb.Append(") => void");

                    string knownGenericType = _knownTypes
                        .Where(x => x.Type == (type.IsGenericType ? type.GetGenericTypeDefinition() : type))
                        .Select(x => x.Name)
                        .FirstOrDefault();
                    if (!string.IsNullOrEmpty(knownGenericType))
                    {
                        return $"{knownGenericType}<{sb}>";
                    }
                }
            }

            if ((typeof(Delegate).IsAssignableFrom(type) || typeof(Delegate).IsAssignableFrom(type.BaseType)))
            {
                if (type.DeclaringType != null && type.DeclaringType.IsGenericType &&
                    typeof(Delegate).IsAssignableFrom(type.DeclaringType.GetGenericArguments()[0]))
                {
                    return "TDelegate";
                }

                return "Function";
            }

            if (TypeMap.ContainsKey(type))
            {
                return TypeMap[type];
            }

            if (type.BaseType != null && TypeMap.ContainsKey(type.BaseType))
            {
                return TypeMap[type.BaseType];
            }

            (Type key, Type value) = GetDictionaryOf(type);
            if (key != null && value != null)
            {
                return $"{{[key: {ReflectType(key)}]: {ReflectType(value)}}}";
            }

            Type enumerableOf = GetEnumerableOf(type);
            if (enumerableOf != null)
            {
                return $"Array<{ReflectType(enumerableOf)}>";
            }

            string knownType = _knownTypes
                .Where(x => x.Type == (type.IsGenericType ? type.GetGenericTypeDefinition() : type))
                .Select(x => x.Name)
                .FirstOrDefault();
            return !string.IsNullOrEmpty(knownType) ? knownType : "any";
        }

        private static Type GetEnumerableOf(Type type)
        {
            Type iface;
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                iface = type;
            }
            else
            {
                iface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }
            return iface == null ? null : iface.GetGenericArguments()[0];
        }

        private static (Type Key, Type Value) GetDictionaryOf(Type type)
        {
            Type iface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (iface == null) return (null, null);

            Type[] genericArguments = iface.GetGenericArguments();
            return (genericArguments[0], genericArguments[1]);
        }
    }
}