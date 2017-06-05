using foundation;

namespace starling
{
    public interface IScrollable:IEventDispatcher
    {
        /// <summary>
        /// 完整宽度; 
        /// </summary>
        int fullWidth { get; }

        /// <summary>
        /// 完整高度; 
        /// </summary>
        int fullHeight { get; }

        Scroller scroller { set; }

        float scrollX
        {
            get; set;
        }

        float scrollY
        {
            get; set;
        }

        DisplayObjectX getScrollView();
    }
}
