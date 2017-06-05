using UnityEngine;

namespace starling
{
    public class Bar : Quad
    {
        public static int barSize = 5;
        public static Color clr = new Color(0.5f, 0.5f, 0.5f);

        private int _size;
        private Direction direct;

        public Bar(Direction dir)
        {
            this.direct = dir;

            if (direct == Direction.HORIZONTAL)
            {

            }

            this.init(barSize, barSize);
        }

        public int size
        {
            get { return _size; }
            set { _size = value; }
        }

        private int _pos = int.MaxValue;

        public int position
        {
            set
            {
                if (_pos == value)
                {
                    return;
                }
                _pos = value;

                int v = _size;
                if (direct == Direction.HORIZONTAL)
                {
                }
                else
                {

                }
            }
        }

        public int max { get; internal set; }
    }
}