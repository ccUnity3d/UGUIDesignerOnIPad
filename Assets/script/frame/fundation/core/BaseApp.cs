using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace foundation
{
    public class BaseApp:MonoBehaviour,IResizeable
    {
        protected bool debugUIEnabled = false;

        protected static List<IClearable> clearableList=new List<IClearable>();
        public static BaseApp instance;
     
        protected StateMachine sceneMachine;

        private List<string> logs=new List<string>();

        public BaseApp()
        {
        }

        public void OnEnable()
        {
            Application.logMessageReceived += handleLog;
        }

        public void OnDisable()
        {
            Application.logMessageReceived -= handleLog;
        }


        public static void AddClearable(IClearable value)
        {
            if (clearableList.Contains(value) == false)
            {
                clearableList.Add(value);
            }
        }

        public static void RemoveClearable(IClearable value)
        {
            if (clearableList.Contains(value))
            {
                clearableList.Remove(value);
            }
        }

        protected virtual void handleLog(string condition, string stackTrace, LogType type)
        {
            string msg = string.Format(@"[{0} {1}] {2}", DateTime.Now.ToString(), type, condition /*+":"+ stackTrace*/);
            if (logs.Count > 50)
            {
                logs.RemoveRange(0,10);
            }

            if (type == LogType.Error || type == LogType.Exception)
            {
                msg += "\n"+stackTrace;

                if (Application.isWebPlayer)
                {
                    Application.ExternalCall("tip",msg);
                }
            }

            logs.Add(msg);
        }

        protected virtual void Start()
        {

            instance = this;

            sceneMachine = new StateMachine();

            ResizeMananger.Add(this);

            InputManager.getInstance().registKeyDown(KeyCode.UpArrow,keyDebugHandle,false,true);
        }

        public virtual void onResize(float width, float height)
        {
            
        }

        protected void keyDebugHandle(KeyCode code)
        {
            debugUIEnabled = !debugUIEnabled;
        }

        public void onGUI(Action action, bool v=true)
        {
            bool has = guiActions.Contains(action);
            if (v)
            {
                if (has == false)
                {
                    guiActions.Add(action);
                }
            }
            else
            {
                if (has)
                {
                    guiActions.Remove(action);
                }
            }
        }

        private  List<Action> guiActions=new List<Action>();
        private Vector2 scrollPosition=new Vector2(0,int.MaxValue);
        protected virtual void OnGUI()
        {
            foreach (Action action in guiActions)
            {
                action();
            }

            if (debugUIEnabled)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,GUILayout.MinWidth(600),GUILayout.MaxHeight(300));

                StringBuilder sb=new StringBuilder();
                foreach (string log in logs)
                {
                    sb.AppendLine(log);
                }
                GUILayout.TextArea(sb.ToString(),GUILayout.MaxHeight(300));
                GUILayout.EndScrollView();
            }
        }

        protected virtual void Update()
        {
            ResizeMananger.getInstance().resize(Screen.width,Screen.height);
            TickManager.getInstance().tick();
        }

        protected virtual void FixedUpdate()
        {
            InputManager.getInstance().update();
            TickManager.getInstance().fixedTick();
        }

        protected virtual void OnDrawGizmos()
        {
            if(Application.isPlaying)
            TickManager.getInstance().gizmos();
        }

        public static void Clear()
        {
            foreach (IClearable item in clearableList)
            {
                item.clear();
            }
        }
        public static void FindLinkData()
        {
            foreach (IClearable item in clearableList)
            {
                item.findeLinkData();
            }
        }
    }
}
