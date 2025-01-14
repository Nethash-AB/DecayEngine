using System;
using System.Reflection;

namespace DecayEngine.Tween.Object.Tween.Data
{
    public class TweenPropertyData : TweenData
    {
        private readonly PropertyInfo _property;

        public TweenPropertyData(PropertyInfo property, object targetObject, float targetValue) : base(targetObject, targetValue)
        {
            _property = property;
        }

        public override void SetValue(float value)
        {
            _property.SetValue(TargetObject, Convert.ChangeType(GetScaled(value), _property.PropertyType));
        }

        public override void Reset()
        {
            InitialValue = (float) Convert.ChangeType(_property.GetValue(TargetObject), typeof(float));
        }
    }
}