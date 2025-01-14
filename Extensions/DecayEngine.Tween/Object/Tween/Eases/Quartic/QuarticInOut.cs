namespace DecayEngine.Tween.Object.Tween.Eases.Quartic
{
    public class QuarticInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if ((time *= 2) < 1) return 0.5f * time * time * time * time;
            return -0.5f * ((time -= 2) * time * time * time - 2);
        }
    }
}