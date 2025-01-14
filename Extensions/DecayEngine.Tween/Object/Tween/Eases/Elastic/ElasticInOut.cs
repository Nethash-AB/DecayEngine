using System;
using DecayEngine.DecPakLib.Math;

namespace DecayEngine.Tween.Object.Tween.Eases.Elastic
{
    public class ElasticInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time.IsZero()) return 0f;
            if (time.IsApproximately(1f)) return 1f;
            if ((time *= 2) < 1) return -0.5f * (float) (Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time - 0.4f / 4) * (2 * Math.PI) / 0.4f));
            return (float) (Math.Pow(2, -10 * (time -= 1)) * Math.Sin((time - 0.4f / 4) * (2 * Math.PI) / 0.4f) * 0.5f + 1);
        }
    }
}