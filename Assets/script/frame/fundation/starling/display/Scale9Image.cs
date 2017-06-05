using foundation;

namespace starling
{
    /// <summary>
    /// 九宫格
    /// </summary>
    public class Scale9Image : DisplayObjectContainerX
    {
        private Image _tl;
        private Image _tc;
        private Image _tr;
        private Image _cl;
        private Image _cc;
        private Image _cr;
        private Image _bl;
        private Image _bc;
        private Image _br;

        private RectangleX _grid;
        private float _tW;
        private float _tH;

        private float _height;
        private float _width;

        private bool _scaleIfSmaller = true;

        public bool scaleIfSmaller
        {
            get { return _scaleIfSmaller; }
            set
            {
                if (_scaleIfSmaller == value) return;

                _scaleIfSmaller = value;
            }
        }

        public override float width
        {
            get { return _width; }
            set
            {
                if (_width == value) return;

                _width = value;
                apply9Scale(_width, _height);
            }
        }

        public override float height
        {
            get { return _height; }
            set
            {
                if (_height == value) return;

                _height = value;
                apply9Scale(_width, _height);
            }
        }

        public Scale9Image(TextureX texture, RectangleX centerRect)
        {
            _tW = texture.width;
            _tH = texture.height;
            _grid = centerRect;

            _width = _tW;
            _height = _tH;

            _tl = new Image(TextureX.fromTexture(texture, new RectangleX(0, 0, _grid.x, _grid.y)));

            _tc = new Image(TextureX.fromTexture(texture, new RectangleX(_grid.x, 0, _grid.width, _grid.y)));

            _tr = new Image(TextureX.fromTexture(texture,
                new RectangleX(_grid.x + _grid.width, 0, texture.width - (_grid.x + _grid.width), _grid.y)));

            _cl = new Image(TextureX.fromTexture(texture, new RectangleX(0, _grid.y, _grid.x, _grid.height)));

            _cc = new Image(TextureX.fromTexture(texture, new RectangleX(_grid.x, _grid.y, _grid.width, _grid.height)));

            _cr = new Image(TextureX.fromTexture(texture,
                new RectangleX(_grid.x + _grid.width, _grid.y, texture.width - (_grid.x + _grid.width), _grid.height)));

            _bl = new Image(TextureX.fromTexture(texture,
                new RectangleX(0, _grid.y + _grid.height, _grid.x, texture.height - (_grid.y + _grid.height))));

            _bc = new Image(TextureX.fromTexture(texture,
                new RectangleX(_grid.x, _grid.y + _grid.height, _grid.width,
                    texture.height - (_grid.y + _grid.height))));

            _br = new Image(TextureX.fromTexture(texture,
                new RectangleX(_grid.x + _grid.width, _grid.y + _grid.height,
                    texture.width - (_grid.x + _grid.width), texture.height - (_grid.y + _grid.height))));

            _tc.x = _cc.x = _bc.x = _grid.x;
            _cl.y = _cc.y = _cr.y = _grid.y;

            addChild(_tl);
            addChild(_tc);
            addChild(_tr);

            addChild(_cl);
            addChild(_cc);
            addChild(_cr);

            addChild(_bl);
            addChild(_bc);
            addChild(_br);

            apply9Scale(_tW, _tH);
        }

        private void apply9Scale(float x, float y)
        {
            float width = x/scaleX;
            float height = y/scaleY;
            float lw;
            float rw;
            float rx;
            float pct;
            float cw;
            float tw;
            float bh;
            float bx;
            if (width < _tW - _grid.width)
            {
                _tc.visible = false;
                _bc.visible = false;

                if (!_scaleIfSmaller)
                {
                    lw = _grid.x;

                    _tl.width = lw;
                    _cl.width = lw;
                    _bl.width = lw;

                    _tr.x = lw;
                    _cr.x = lw;
                    _br.x = lw;

                    rw = (_tW - _grid.x - _grid.width);
                    _tr.width = rw;
                    _cr.width = rw;
                    _br.width = rw;
                }
                else
                {
                    pct = width/(_tW - _grid.width);
                    lw = _grid.x*pct;
                    _tl.width = lw;
                    _cl.width = lw;
                    _bl.width = lw;

                    rw = (_tW - _grid.x - _grid.width)*pct;
                    _tr.width = rw;
                    _cr.width = rw;
                    _br.width = rw;

                    rx = width - rw;
                    _tr.x = rx;
                    _cr.x = rx;
                    _br.x = rx;

                }
            }
            else
            {
                _tc.visible = true;
                _bc.visible = true;

                lw = _grid.x;
                _tl.width = lw;
                _cl.width = lw;
                _bl.width = lw;

                rw = (_tW - _grid.x - _grid.width);
                _tr.width = rw;
                _cr.width = rw;
                _br.width = rw;

                rx = width - rw;
                _tr.x = rx;
                _cr.x = rx;
                _br.x = rx;

                cw = rx - _grid.x;
                _tc.width = cw;
                _cc.width = cw;
                _bc.width = cw;
            }

            if (height < _tH - _grid.height)
            {
                _cl.visible = false;
                _cr.visible = false;

                if (!_scaleIfSmaller)
                {
                    tw = _grid.y;

                    _tl.height = tw;
                    _tc.height = tw;
                    _tr.height = tw;

                    _br.y = tw;
                    _bc.y = tw;
                    _bl.y = tw;

                    bh = (_tH - _grid.y - _grid.height);
                    _br.height = bh;
                    _bc.height = bh;
                    _bl.height = bh;
                }
                else
                {
                    pct = height/(_tH - _grid.height);

                    tw = _grid.y*pct;
                    _tl.height = tw;
                    _tc.height = tw;
                    _tr.height = tw;

                    bh = (_tH - _grid.y - _grid.height)*pct;
                    _br.height = bh;
                    _bc.height = bh;
                    _bl.height = bh;

                    bx = height - bh;
                    _br.y = bx;
                    _bc.y = bx;
                    _bl.y = bx;

                }
            }
            else
            {
                _cl.visible = true;
                _cr.visible = true;

                _tl.height = _grid.y;
                _tc.height = _grid.y;
                _tr.height = _grid.y;

                bh = (_tH - _grid.y - _grid.height);

                _bl.height = bh;
                _bc.height = bh;
                _br.height = bh;

                float by = height - bh;
                _bl.y = by;
                _bc.y = by;
                _br.y = by;

                float ch = by - _grid.y;
                _cl.height = ch;
                _cc.height = ch;
                _cr.height = ch;
            }

            _cc.visible = _cl.visible && _tc.visible;
        }
    }
}
