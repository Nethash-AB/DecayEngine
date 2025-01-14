using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Sinusoidal
{
    public class SinusoidalInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time.IsZero()) return 0f;
            if (time.IsApproximately(1f)) return 1f;
            return 0.5f * (1f - (float) Math.Cos(Math.PI * time));
        }
    }
}