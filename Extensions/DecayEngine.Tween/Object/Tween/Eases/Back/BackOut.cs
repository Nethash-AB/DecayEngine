namespace DecayEngine.Tween.Object.Tween.Eases.Back
{
    public class BackOut : TweenEase
    {
        public override float Calculate(float time)
        {
            return --time * time * (2.70158f * time + 1.70158f) + 1;
        }
    }
}