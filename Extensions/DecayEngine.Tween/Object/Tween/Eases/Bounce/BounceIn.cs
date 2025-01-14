namespace DecayEngine.Tween.Object.Tween.Eases.Bounce
{
    public class BounceIn : TweenEase
    {
        public override float Calculate(float time)
        {
            time = 1 - time;

            if (time < 1 / 2.75f)
            {
                return 7.5625f * time * time;
            }

            if (time < 2 / 2.75f)
            {
                return 7.5625f * (time -= 1.5f / 2.75f) * time + 0.75f;
            }

            if (time < 2.5f / 2.75f)
            {
                return 7.5625f * (time -= 2.25f / 2.75f) * time + 0.9375f;
            }

            return 7.5625f * (time -= 2.625f / 2.75f) * time + 0.984375f;
        }
    }
}