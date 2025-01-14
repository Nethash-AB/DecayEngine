using System;

namespace DecayEngine.Tween.Object.Tween.Eases.Circular
{
    public class CircularOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return (float) Math.Sqrt(1 - (--time * time));
        }
    }
}