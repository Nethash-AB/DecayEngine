namespace DecayEngine.Tween.Object.Tween.Eases.Quadratic
{
    public class QuadraticInOut : TweenEase
    {
        public override float Calculate(float time)
        {
            if ((time *= 2) < 1) return 0.5f * time * time;
            return -0.5f * (--time * (time - 2) - 1);
        }
    }
}