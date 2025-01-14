namespace DecayEngine.Tween.Object.Tween.Eases.Quadratic
{
    public class QuadraticIn : TweenEase
    {
        public override float Calculate(float time)
        {
            return time * time;
        }
    }
}