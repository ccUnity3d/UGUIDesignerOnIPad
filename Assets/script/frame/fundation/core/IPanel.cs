using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public interface IPanel:ISkinable,IPoolable,IDisposable,IDataRenderer,IEventDispatcher,IResizeable
    {
        /// <summary>
        /// 展示出来
        /// </summary>
		void show(GameObject go=null);
		
		/**
		 * 是否是展示状态 
		 * @return 
		 * 
		 */
        bool isShow
        {
            get;
        }

        /// <summary>
        /// 面板的清理操作
        /// </summary>
         void Clear();


		/**
		 * 弹到最顶层; 
		 * 
		 */		
		void bringTop();

        /// <summary>
        /// panel可以存在的场景状态
        /// </summary>
        /// <returns></returns>
        ASList<string> getSceneState();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneState"></param>
        void changeState(string sceneState);


        /**
		 * 隐藏 
		 * @param event
		 * 
		 */
        void hide(EventX e=null);
    }
}
