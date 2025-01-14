namespace DecayEngine.Tween.Object.Tween.Eases.Cubic
{
    public class CubicIn : TweenEase
    {
        public override float Calculate(float time)
        {
            return time * time * time;
        }
    }
}