namespace DecayEngine.Tween.Object.Tween.Eases.Back
{
    public class BackInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if ((time *= 2) < 1) return 0.5f * (time * time * (3.5949095f * time - 2.5949095f));
            return 0.5f * ((time -= 2) * time * (3.5949095f * time + 2.5949095f) + 2);
        }
    }
}