namespace DecayEngine.Tween.Object.Tween.Eases.Quadratic
{
    public class QuadraticOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return time * (2 - time);
        }
    }
}