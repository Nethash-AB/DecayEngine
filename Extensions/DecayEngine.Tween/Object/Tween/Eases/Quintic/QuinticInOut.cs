namespace DecayEngine.Tween.Object.Tween.Eases.Quintic
{
    public class QuinticInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if ((time *= 2) < 1) return 0.5f * time * time * time * time * time;
            return 0.5f * ((time -= 2) * time * time * time * time + 2);
        }
    }
}