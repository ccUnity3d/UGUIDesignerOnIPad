using System;
using UnityEngine;
using foundation;
using System.Collections.Generic;

namespace foundation
{

    /// <summary>
    ///  场景变化管理类
    /// </summary>
    public class ResizeMananger
    {
        protected List<Action<float, float>> resizeList;
        protected float width = 1;
        protected float height = 1;

        public static int oldScreenWidth;
        public static int oldScreenHight;
        public static float uiDiySize = 640f;

        public static float getUIScale()
        {
            return Screen.width / uiDiySize;
        }

        private static ResizeMananger instance;

        public ResizeMananger()
        {
            resizeList = new List<Action<float, float>>();

            instance = this;
        }

        public static ResizeMananger getInstance()
        {
            if (instance == null)
            {
                instance = new ResizeMananger();
            }
            return instance;
        }

        public static void Add(IResizeable item)
        {
            if (item == null)
            {
                return;
            }
            getInstance().add(item.onResize);
        }

        public static void Remove(IResizeable item)
        {
            if (item == null)
            {
                return;
            }
            getInstance().remove(item.onResize);
        }


        public static void AddAction(Action<float, float> item)
        {
            if (item == null)
            {
                return;
            }
            getInstance().add(item);
        }

        public static void RemoveAction(Action<float, float> item)
        {
            if (item == null)
            {
                return;
            }
            getInstance().remove(item);
        }


        public static void diy(float w, float h)
        {
            getInstance().resize(w, h);
        }

        public void resize(float w, float h)
        {
            if (w == width && h == height)
            {
                return;
            }

            width = w;
            height = h;

            foreach (Action<float, float> item in resizeList)
            {
                item(width, height);
            }

            //dispatchEvent(e);
        }

        public static void Resize(int width, int height)
        {
            getInstance().resize(width, height);
        }



        /// <summary>
        /// 添加;
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool add(Action<float, float> item)
        {
            addingResize(item);
            if (resizeList.IndexOf(item) != -1)
            {
                return false;
            }
            resizeList.Add(item);
            return true;
        }

        protected void addingResize(Action<float, float> item)
        {
            item(width, height);
        }

        /// <summary>
        /// 刷新当前的坐标;
        /// </summary>
        /// <param name="value"></param>
        public void refreash(IResizeable value)
        {
            value.onResize(width, height);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int remove(Action<float, float> item)
        {
            int index = resizeList.IndexOf(item);
            if (index == -1) return -1;

            resizeList.RemoveAt(index);
            return index;
        }

    }

}