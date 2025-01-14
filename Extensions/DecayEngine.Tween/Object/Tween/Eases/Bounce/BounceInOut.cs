namespace DecayEngine.Tween.Object.Tween.Eases.Bounce
{
    public class BounceInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if (time < 0.5f)
            {
                time *= 2;
                time = 1 - time;
                if (time < 1 / 2.75f)
                {
                    return 7.5625f * time * time * 0.5f;
                }

                if (time < 2 / 2.75f)
                {
                    return 7.5625f * (time -= 1.5f / 2.75f) * time + 0.75f * 0.5f;
                }

                if (time < 2.5f / 2.75f)
                {
                    return 7.5625f * (time -= 2.25f / 2.75f) * time + 0.9375f * 0.5f;
                }

                return 7.5625f * (time -= 2.625f / 2.75f) * time + 0.984375f * 0.5f;
            }

            time = time * 2 - 1;
            if (time < 1 / 2.75f)
            {
                return 7.5625f * time * time * 0.5f + 0.5f;
            }

            if (time < 2 / 2.75f)
            {
                return 7.5625f * (time -= 1.5f / 2.75f) * time + 0.75f * 0.5f + 0.5f;
            }

            if (time < 2.5f / 2.75f)
            {
                return 7.5625f * (time -= 2.25f / 2.75f) * time + 0.9375f * 0.5f + 0.5f;
            }

            return 7.5625f * (time -= 2.625f / 2.75f) * time + 0.984375f * 0.5f + 0.5f;
        }
    }
}