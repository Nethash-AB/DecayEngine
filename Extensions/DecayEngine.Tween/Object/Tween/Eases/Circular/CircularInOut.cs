using System;

namespace DecayEngine.Tween.Object.Tween.Eases.Circular
{
    public class CircularInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if ((time *= 2) < 1) return 0.5f * ((float) Math.Sqrt(1 - time * time) - 1);
            return 0.5f * ((float) Math.Sqrt(1 - (time -= 2) * 2) + 1);
        }
    }
}