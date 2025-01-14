using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Elastic
{
    public class ElasticOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time.IsZero()) return 0f;
            if (time.IsApproximately(1f)) return 1f;
            return (float) (Math.Pow(2, -10 * time) * Math.Sin((time - 0.4f / 4) * (2 * Math.PI) / 0.4f) + 1);
        }
    }
}