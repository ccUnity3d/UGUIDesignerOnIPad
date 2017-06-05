namespace starling
{
    public class BitmapTextField:Image
    {
        public BitmapTextField() : base(null)
        {
            
        }

        public string text { get; internal set; }
        public TextFormat textFormat { get; internal set; }
    }
}
