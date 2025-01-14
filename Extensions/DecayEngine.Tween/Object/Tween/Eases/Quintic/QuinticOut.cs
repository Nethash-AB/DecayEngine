namespace DecayEngine.Tween.Object.Tween.Eases.Quintic
{
    public class QuinticOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return --time * time * time * time * time + 1;
        }
    }
}