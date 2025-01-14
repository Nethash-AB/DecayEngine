namespace DecayEngine.Tween.Object.Tween.Data
{
    public abstract class TweenData
    {
        protected float InitialValue { get; set; }
        protected float TargetValue { get; }
        protected object TargetObject { get; }

        protected TweenData(object targetObject, float targetValue)
        {
            TargetObject = targetObject;
            TargetValue = targetValue;
        }

        protected float GetScaled(float value)
        {
            if (value > 1) value = 1;
            else if (value < 0) value = 0;

            return value * (TargetValue - InitialValue) + InitialValue;
        }

        public abstract void SetValue(float value);
        public abstract void Reset();
    }
}