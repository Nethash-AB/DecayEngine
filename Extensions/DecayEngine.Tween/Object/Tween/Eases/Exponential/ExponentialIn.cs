using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Exponential
{
    public class ExponentialIn : TweenEase
    {
        public override float Calculate(float time)
        {
            return time.IsZero() ? 0f : (float) Math.Pow(1024, time - 1);
        }
    }
}