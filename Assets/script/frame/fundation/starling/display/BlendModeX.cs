namespace starling
{

    public class BlendModeX
    {
        /** Inherits the blend mode from this display object's parent. */
        public const string AUTO = "auto";

        /** Deactivates blending, i.e. disabling any transparency. */
        public const string NONE = "none";

        /** The display object appears in front of the background. */
        public const string NORMAL = "normal";

        /** Adds the values of the colors of the display object to the colors of its background. */
        public const string ADD = "add";

        /** Multiplies the values of the display object colors with the the background color. */
        public const string MULTIPLY = "multiply";

        /** Multiplies the complement (inverse) of the display object color with the complement of 
		 * the background color, resulting in a bleaching effect. */
        public const string SCREEN = "screen";

        /** Erases the background when drawn on a RenderTexture. */
        public const string ERASE = "erase";

        /** Draws under/below existing objects; useful especially on RenderTextures. */
        public const string BELOW = "below";

    }
}
