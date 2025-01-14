namespace DecayEngine.Tween.Object.Tween.Eases.Quartic
{
    public class QuarticIn : TweenEase
    {
        public override float Calculate(float time)
        {
            return time * time * time * time;
        }
    }
}