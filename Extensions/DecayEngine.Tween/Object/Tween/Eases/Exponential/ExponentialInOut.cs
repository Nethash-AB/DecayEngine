using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Exponential
{
    public class ExponentialInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time.IsZero()) return 0f;
            if (time.IsApproximately(1f)) return 1f;
            if ((time *= 2) < 1) return 0.5f * (float) Math.Pow(1024, time - 1);
            return 0.5f * ((float) -Math.Pow(2, -10 * (time - 1)) + 2);
        }
    }
}