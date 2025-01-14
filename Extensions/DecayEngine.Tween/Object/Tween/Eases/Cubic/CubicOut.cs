namespace DecayEngine.Tween.Object.Tween.Eases.Cubic
{
    public class CubicOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return --time * time * time + 1;
        }
    }
}