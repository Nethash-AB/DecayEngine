using System;

namespace DecayEngine.Tween.Object.Tween.Eases.Circular
{
    public class CircularIn : TweenEase
    {
        public override float Calculate(float time)
        {
            return 1 - (float) Math.Sqrt(1 - time * time);
        }
    }
}