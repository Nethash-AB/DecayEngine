using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Sinusoidal
{
    public class SinusoidalIn : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time.IsZero()) return 0f;
            if (time.IsApproximately(1f)) return 1f;
            return 1f - (float) Math.Cos(time * Math.PI / 2f);
        }
    }
}