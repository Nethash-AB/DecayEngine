using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Exponential
{
    public class ExponentialOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return time.IsApproximately(1f) ? 1f : 1f - (float) Math.Pow(2f, -10f * time);
        }
    }
}