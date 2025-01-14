namespace DecayEngine.Tween.Object.Tween.Eases.Quartic
{
    public class QuarticOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return 1 - (--time * time * time * time);
        }
    }
}