using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.Coroutine;
using DecayEngine.Tween.Object.Tween;
using DecayEngine.Tween.Object.Tween.Data;
using DecayEngine.Tween.Object.Tween.Eases.Back;
using DecayEngine.Tween.Object.Tween.Eases.Bounce;
using DecayEngine.Tween.Object.Tween.Eases.Circular;
using DecayEngine.Tween.Object.Tween.Eases.Cubic;
using DecayEngine.Tween.Object.Tween.Eases.Elastic;
using DecayEngine.Tween.Object.Tween.Eases.Exponential;
using DecayEngine.Tween.Object.Tween.Eases.Linear;
using DecayEngine.Tween.Object.Tween.Eases.Quadratic;
using DecayEngine.Tween.Object.Tween.Eases.Quartic;
using DecayEngine.Tween.Object.Tween.Eases.Quintic;
using DecayEngine.Tween.Object.Tween.Eases.Sinusoidal;

namespace DecayEngine.Tween.Exports.Tween
{
    [ScriptExportClass("Tween", "Represents a Tween Component that can update references to fields and/or properties over time.")]
    public class TweenExport : ExportableManagedObject<Object.Tween.Tween>
    {
        [ScriptExportProperty("An `EventHandler` that fires when the tweening operation is finished.")]
        public EventExport<Action> OnEnd { get; }

        public override int Type => (int) ManagedExportType.Tween;

        [ScriptExportConstructor]
        public TweenExport(
            [ScriptExportParameter("The ease type the `Tween` will use to interpolate.", typeof(TweenEaseType))] int easeType,
            [ScriptExportParameter("The target time in milliseconds the `Tween` will take to complete.")] float targetTime,
            [ScriptExportParameter("The object to perform the tweening operation on. It must not be a value type.")] object sourceObject,
            [ScriptExportParameter("An object containing the members of the `sourceObject` that the `Tween` will update.")] object values
        )
        {
            object targetObject = GameEngine.ScriptEngine.MarshalFrom<object>(sourceObject);
            Type targetObjectType = targetObject.GetType();
            if (targetObjectType.IsValueType) throw new Exception("Cannot tween source value types.");

            List<TweenData> tweenData = new List<TweenData>();
            Dictionary<string, object> objectProperties = GameEngine.ScriptEngine.MarshalFrom<Dictionary<string, object>>(values);
            foreach (KeyValuePair<string, object> pair in objectProperties)
            {
                if (!IsNumericObject(pair.Value)) continue;
                float targetValue = Convert.ToSingle(pair.Value);

                MemberInfo memberInfo = targetObjectType
                    .GetMember(pair.Key, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault();
                if (memberInfo == null) continue;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
                        if (!propertyInfo.CanRead || !propertyInfo.CanWrite || !IsNumericType(propertyInfo.PropertyType)) continue;
                        tweenData.Add(new TweenPropertyData(
                            propertyInfo,
                            targetObject,
                            targetValue)
                        );
                        break;
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = (FieldInfo) memberInfo;
                        if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral || !IsNumericType(fieldInfo.FieldType)) continue;
                        tweenData.Add(new TweenFieldData(
                            fieldInfo,
                            targetObject,
                            targetValue)
                        );
                        break;
                    default:
                        continue;
                }

                TweenEase tweenEase = GetEase((TweenEaseType) easeType);
                if (tweenEase == null) throw new Exception("Invalid tween ease type.");

                Object.Tween.Tween tween = new Object.Tween.Tween(tweenEase, tweenData, targetTime);

                OnEnd = new EventExport<Action>();
                tween.OnEnd += () => OnEnd.Fire();

                Reference = tween;
            }
        }

        [ScriptExportMethod("Runs the `Tween`.")]
        public void Run(
            [ScriptExportParameter("The context the tween will run on.", typeof(CoroutineContextExport))] int context
        )
        {
            Reference.Run((CoroutineContext) context);
        }

        internal TweenExport(ByReference<Object.Tween.Tween> referencePointer) : base(referencePointer)
        {
        }

        private static TweenEase GetEase(TweenEaseType type)
        {
            return type switch
            {
                TweenEaseType.Linear => (TweenEase) new None(),
                TweenEaseType.QuadraticIn => new QuadraticIn(),
                TweenEaseType.QuadraticOut => new QuadraticOut(),
                TweenEaseType.QuadraticInOut => new QuadraticInOut(),
                TweenEaseType.CubicIn => new CubicIn(),
                TweenEaseType.CubicOut => new CubicOut(),
                TweenEaseType.CubicInOut => new CubicInOut(),
                TweenEaseType.QuarticIn => new QuarticIn(),
                TweenEaseType.QuarticOut => new QuarticOut(),
                TweenEaseType.QuarticInOut => new QuarticInOut(),
                TweenEaseType.QuinticIn => new QuinticIn(),
                TweenEaseType.QuinticOut => new QuinticOut(),
                TweenEaseType.QuinticInOut => new QuinticInOut(),
                TweenEaseType.SinusoidalIn => new SinusoidalIn(),
                TweenEaseType.SinusoidalOut => new SinusoidalOut(),
                TweenEaseType.SinusoidalInOut => new SinusoidalInOut(),
                TweenEaseType.ExponentialIn => new ExponentialIn(),
                TweenEaseType.ExponentialOut => new ExponentialOut(),
                TweenEaseType.ExponentialInOut => new ExponentialInOut(),
                TweenEaseType.CircularIn => new CircularIn(),
                TweenEaseType.CircularOut => new CircularOut(),
                TweenEaseType.CircularInOut => new CircularInOut(),
                TweenEaseType.ElasticIn => new ElasticIn(),
                TweenEaseType.ElasticOut => new ElasticOut(),
                TweenEaseType.ElasticInOut => new ElasticInOut(),
                TweenEaseType.BackIn => new BackIn(),
                TweenEaseType.BackOut => new BackOut(),
                TweenEaseType.BackInOut => new BackInOut(),
                TweenEaseType.BounceIn => new BounceIn(),
                TweenEaseType.BounceOut => new BounceOut(),
                TweenEaseType.BounceInOut => new BounceInOut(),
                _ => new None()
            };
        }

        private static bool IsNumericObject(object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }

        private static bool IsNumericType(Type value)
        {
            return typeof(sbyte).IsAssignableFrom(value)
                   || typeof(byte).IsAssignableFrom(value)
                   || typeof(short).IsAssignableFrom(value)
                   || typeof(ushort).IsAssignableFrom(value)
                   || typeof(int).IsAssignableFrom(value)
                   || typeof(uint).IsAssignableFrom(value)
                   || typeof(long).IsAssignableFrom(value)
                   || typeof(ulong).IsAssignableFrom(value)
                   || typeof(float).IsAssignableFrom(value)
                   || typeof(double).IsAssignableFrom(value)
                   || typeof(decimal).IsAssignableFrom(value);
        }
    }
}