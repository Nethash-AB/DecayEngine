namespace DecayEngine.Tween.Object.Tween.Eases.Linear
{
    public class None : TweenEase
    {
        public override float Calculate(float time)
        {
            return time;
        }
    }
}