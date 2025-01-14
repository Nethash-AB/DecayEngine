using System;
using System.Reflection;

namespace DecayEngine.Tween.Object.Tween.Data
{
    public class TweenFieldData : TweenData
    {
        private readonly FieldInfo _field;

        public TweenFieldData(FieldInfo field, object targetObject, float targetValue) : base(targetObject, targetValue)
        {
            _field = field;
        }

        public override void SetValue(float value)
        {
            _field.SetValue(TargetObject, Convert.ChangeType(GetScaled(value), _field.FieldType));
        }

        public override void Reset()
        {
            InitialValue = (float) Convert.ChangeType(_field.GetValue(TargetObject), typeof(float));
        }
    }
}