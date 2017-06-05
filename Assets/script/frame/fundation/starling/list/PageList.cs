using System.Collections;
using System.Collections.Generic;
using foundation;
using UnityEngine;

namespace starling
{
    public class PageList : DisplayObjectContainerX, IScrollable,IDataProvider
    {
        public int cacheSize = 10;
        public bool allowRepeat = false;

        /**
		 * 间隙 
		 */
        protected int _itemGapH = 0;
        protected int _itemGapV = 0;

        protected RectOffset _border = new RectOffset();

        protected int _firstIndex = 0;

        /**
		 * 当前显示列表;
		 */
        protected List<IListItemRender> _childrenList;

        /**
		 * 元素构造器
		 */
        protected IFactory _itemFacotry;
        protected Stack<IListItemRender> _caches;

        protected int _itemWidth;
        protected int _itemHeight;

        protected int _itemBoundWidth;
        protected int _itemBoundHeight;

        protected IList _dataProvider;

        protected IListItemRender _selectedItem;
        protected object _selectedData;

        protected bool _vertical;
        protected Scroller _scroller;
        protected int _maxDirect;
        protected int _fullWidth;
        protected int _fullHeight;

        //protected var renderTick:RenderTick;

        protected bool isCenter;
        protected int centerWidth;
        protected int itemLen;
        /**
		 * 
		 * @param itemFacotry 项创建工厂
		 * @param itemWidth 项宽
		 * @param itemHeight 项高
		 * @param vertical 是否垂直排布
		 * @param maxDirect 是否限制最大方向排列数
		 *
		 */

        public PageList(IFactory itemFacotry, int itemWidth, int itemHeight, bool vertical = true, int maxDirect = -1)
        {
            this._itemFacotry = itemFacotry;
            this._itemWidth = itemWidth;
            this._itemHeight = itemHeight;

            this._vertical = vertical;
            this._maxDirect = maxDirect;
            this._itemBoundWidth = _itemWidth + _itemGapH;
            this._itemBoundHeight = _itemHeight + _itemGapV;
            this.isCenter = false;
            _caches = new Stack<IListItemRender>();
            _childrenList = new List<IListItemRender>();
            //renderTick = new RenderTick();
        }

        public int itemWidth
        {
            get { return _itemWidth; }
        }

        public void setCenterWidth(int _centerWidth)
        {
            this.isCenter = true;
            this.centerWidth = _centerWidth;
        }

        public void resetCenterWidht()
        {
            this.isCenter = false;
        }

        public int itemGap
        {
            set
            {
                if (_vertical)
                {
                    itemGapV = value;
                }
                else
                {
                    itemGapH = value;
                }
            }

        }

        public IFactory itemFacotry
        {
            get { return _itemFacotry; }
        }

        public int itemGapH
        {
            set
            {
                _itemGapH = value;
                this._itemBoundWidth = _itemWidth + _itemGapH;
            }

        }

        public int itemGapV
        {
            set
            {
                _itemGapV = value;
                this._itemBoundHeight = _itemHeight + _itemGapV;
            }
        }

        public void setBorder(int left = 0, int top = 0, int right = 0,
            int bottom = 0)
        {
            _border.left = left;
            _border.top = top;
            _border.right = right;
            _border.bottom = bottom;
        }


        public int firstIndex
        {

            set { _firstIndex = value; }
        }

        public IList dataProvider
        {
            get { return _dataProvider; }
            set
            {
                _dataProvider = value;
                //清空旧的选择项;
                selectedItem = null;
                //开始渲染新数据;
                renderList(_dataProvider);

                if (hasEventListener(EventX.RESIZE))
                {
                    calculatorBound();
                }
            }
        }

        protected void calculatorBound()
        {
            int oldFullW = _fullWidth;
            int oldFullH = _fullHeight;

            _fullWidth = calculatorWidth();
            _fullHeight = calculatorHeight();

            if (oldFullW != _fullWidth || oldFullH != _fullHeight)
            {
                this.dispatchEvent(new EventX(EventX.RESIZE));
            }
        }

        protected void renderList(IList source)
        {
            itemLen = source.Count;
            IListItemRender item;
            int len = source.Count;
            //取得旧子项;
            List<IListItemRender> oldChildrenList = _childrenList.GetRange(0, _childrenList.Count);
            _childrenList.Clear();

            //renderTick.begin();
            DisplayObjectX view;

            for (int i = 0; i < len; i++)
            {
                if (oldChildrenList.Count > 0)
                {
                    item = oldChildrenList[0];
                    oldChildrenList.RemoveAt(0);
                }
                else if (_caches.Count > 0)
                {
                    item = _caches.Pop();
                }
                else
                {
                    item = (IListItemRender) _itemFacotry.newInstance();
                }

                item.chooseState = ChooseState.NORMAL;

                item.index = _firstIndex + i;
                view = item as DisplayObjectX;

                if (view != null)
                {
                    this.addChild(view);
                }
                bindItemData(item, source[i]);

                bindItemEvent(item, true);
                if (view != null)
                {
                    layout(view, i);
                }

                _childrenList.Add(item);
            }

            innerClear(oldChildrenList);
        }

        protected void bindItemData(IListItemRender item, object data)
        {
            item.data = data;
        }

        /**
		 * 清理children列表,默认清理childrenList并会清空childrenList的长度,如有传入值,也将会被清空; 
		 * @param willCleanChildren 替换清理子节点;
		 * 
		 */

        public void clear()
        {
            innerClear(_childrenList);
        }

        protected void innerClear(List<IListItemRender> willCleanChildren)
        {
            IListItemRender item;
            int length = willCleanChildren.Count;
            DisplayObjectX view;
            for (int i = length - 1; i >= 0; i--)
            {
                item = willCleanChildren[i];
                view = item as DisplayObjectX;

                bindItemEvent(item, false);
                if (view != null)
                {
                    removeChild(view);
                }

                if (_caches.Count == cacheSize)
                {
                    item.dispose();
                    continue;
                }
                item.sleep();
                _caches.Push(item);
            }
            willCleanChildren.Clear();
        }


        protected void layout(DisplayObjectX _displayItem, int i)
        {
            int oriXY = 0;
            if (_vertical)
            {
                if (_maxDirect == -1)
                {
                    if (isCenter)
                    {
//居中
                        oriXY = (centerWidth - itemLen*_itemBoundHeight)/2;
                    }
                    _displayItem.y = oriXY + i*_itemBoundHeight + _border.top;
                    _displayItem.x = _border.left;
                    return;
                }
                _displayItem.x = (int) (i/_maxDirect)*_itemBoundWidth + _border.left;
                _displayItem.y = (i%_maxDirect)*_itemBoundHeight + _border.top;
            }
            else
            {
                if (_maxDirect == -1)
                {
                    if (isCenter)
                    {
//居中
                        oriXY = (centerWidth - itemLen*_itemBoundWidth)/2;
                    }
                    _displayItem.x = oriXY + i*_itemBoundWidth + _border.left;
                    _displayItem.y = _border.top;
                    return;
                }
                _displayItem.y = (int) (i/_maxDirect)*_itemBoundHeight + _border.top;
                _displayItem.x = (i%_maxDirect)*_itemBoundWidth + _border.left;
            }
        }

        /**
		 * 取得一个索引项的位置; 
		 * @param i
		 * 
		 */

        protected Vector2X getPosition(int i, bool hasBorder = true)
        {
            Vector2X point = new Vector2X();
            if (_vertical)
            {
                if (_maxDirect == -1)
                {
                    point.x = 0;
                    point.y = _itemBoundHeight*i;
                }
                else
                {
                    point.x = (i/_maxDirect)*_itemBoundWidth;
                    point.y = (i%_maxDirect)*_itemBoundHeight;
                }
            }
            else
            {
                if (_maxDirect == -1)
                {
                    point.x = _itemBoundWidth*i;
                    point.y = 0;
                }
                else
                {
                    point.y = (i/_maxDirect)*_itemBoundHeight;
                    point.x = (i%_maxDirect)*_itemBoundWidth;
                }
            }

            if (hasBorder)
            {
                point.x += _border.left;
                point.y += _border.top;
            }

            return point;
        }

        protected int getPositionY(int i, bool hasBorder = true)
        {
            float pos;
            if (_vertical)
            {
                pos = i/_maxDirect*_itemBoundHeight;
                if (hasBorder)
                {
                    pos += _border.top;
                }
            }
            else
            {
                pos = i%_maxDirect*_itemBoundHeight;
                if (hasBorder)
                {
                    pos += _border.left;
                }
            }

            return (int) pos;
        }


        protected void bindItemEvent(IListItemRender item, bool isBind)
        {
            if (isBind)
            {
                item.addEventListener(MouseEventX.CLICK, clickHandle);
            }
            else
            {
                item.removeEventListener(MouseEventX.CLICK, clickHandle);
            }
        }

        protected void clickHandle(EventX e)
        {
            /*if(TouchSprite.isDragging || DragManager.isDragging){
				return;
			}*/
            IListItemRender item = e.currentTarget as IListItemRender;
            selectedItem = item;

            if (hasEventListener(EventX.ITEM_CLICK))
            {
                EventX ev = new EventX(EventX.ITEM_CLICK, item);
                dispatchEvent(ev);
            }
        }

        public IListItemRender selectedItem
        {
            set
            {
                if (_selectedItem == value && allowRepeat == false)
                {
                    return;
                }
                if (_selectedItem != null)
                {
                    _selectedItem.chooseState = ChooseState.NORMAL;
                }

                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedItem.chooseState = ChooseState.SELECT;
                    _selectedData = _selectedItem.data;
                }
                else
                {
                    _selectedData = null;
                }

                if (hasEventListener(EventX.CHANGE))
                {
                    this.dispatchEvent(new EventX(EventX.SELECT, _selectedItem));
                }
            }
            get { return _selectedItem; }
        }



        public int selectedIndex
        {
            get
            {
                if (_selectedItem == null) return -1;

                return _childrenList.IndexOf(_selectedItem);
            }
            set
            {
                if (_childrenList.Count > value)
                {
                    selectedItem = _childrenList[value];
                    return;
                }
                selectedItem = null;
            }
        }

        /**
		 * 设置选中数据 
		 * @param data
		 * 
		 */

        public object selectedData
        {
            set
            {

                bool has = false;
                foreach (IListItemRender item in _childrenList)
                {
                    if (item.data == value)
                    {
                        selectedItem = item;
                        has = true;
                        break;
                    }
                }

                if (!has)
                {
                    selectedIndex = 0;
                }
            }
            get { return _selectedData; }
        }


        /**
		 *   做一些唤醒操作;
		 */

        public void awaken()
        {
            foreach (IListItemRender item in _childrenList)
            {
                item.awaken();
            }
        }

        /**
		 * 休眠操作;
		 */

        public void sleep()
        {
            foreach (IListItemRender item in _childrenList)
            {
                item.sleep();
            }
        }

        public DisplayObjectX getScrollView()
        {
            return this;
        }

        public List<IListItemRender> items
        {
            get { return _childrenList; }
        }

        public Scroller scroller
        {
            set { _scroller = value; }
        }

        public float scrollX
        {
            set { this.x = value; }
            get { return x; }
        }

        public float scrollY
        {
            set { this.y = value; }
            get { return y; }
        }


        /**
		 * 整体的宽度; 
		 * @return 
		 * 
		 */

        public int fullWidth
        {
            get { return _fullWidth; }
        }

        /**
		 * 整体的高度; 
		 * @return 
		 * 
		 */

        public int fullHeight
        {
            get { return _fullHeight; }
        }

        protected int calculatorWidth()
        {
            if (null == _dataProvider) return 0;

            if (_maxDirect == -1)
            {
                if (_vertical)
                {
                    return _itemWidth + (int) (_border.left + _border.right);
                }
                return _dataProvider.Count*_itemBoundWidth - _itemGapH + (int) (_border.left + _border.right);
            }

            if (_vertical)
            {
                return (int) Mathf.Ceil(_dataProvider.Count/_maxDirect)*_itemBoundWidth - _itemGapH +
                       (int) (_border.left + _border.right);
            }
            int len = _dataProvider.Count;
            if (len < _maxDirect)
            {
                return len*_itemBoundWidth - _itemGapH + (int) (_border.left + _border.right);
            }
            return _maxDirect*_itemBoundWidth - _itemGapH + (int) (_border.left + _border.right);
        }

        protected int calculatorHeight()
        {
            if (null == _dataProvider) return 0;

            if (_maxDirect == -1)
            {
                if (_vertical)
                {
                    return _dataProvider.Count*_itemBoundHeight - _itemGapV + (int) (_border.top + _border.bottom);
                }
                return _itemHeight + (int) (_border.top + _border.bottom);
            }

            if (!_vertical)
            {
                return (int) Mathf.Ceil(_dataProvider.Count/_maxDirect)*_itemBoundHeight - _itemGapV +
                       (int) (_border.top + _border.bottom);
            }
            int len = _dataProvider.Count;
            if (len < _maxDirect)
            {
                return len*_itemBoundHeight - _itemGapV + (int) (_border.top + _border.bottom);
            }
            return _maxDirect*_itemBoundHeight - _itemGapV + (int) (_border.top + _border.bottom);
        }

        public int itemHeight
        {
            get { return _itemHeight; }
        }
    }
}
