namespace foundation
{
    public class AnimationEventX:EventX
    {
        new public const string CHANGE = "ani_change";
        new public const string COMPLETE = "ani_complete";
        public AnimationEventX(string type, object data) : base(type, data)
        {
            
        }
    }
}
