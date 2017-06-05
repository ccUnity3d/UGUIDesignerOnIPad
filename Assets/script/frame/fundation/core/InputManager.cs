using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class InputManager:EventDispatcher
    {
        private Dictionary<string, Action<KeyCode>> eventDownDictonary=new Dictionary<string, Action<KeyCode>>();
        private Dictionary<string, List<KeyCode>> keyCodeMapping=new Dictionary<string, List<KeyCode>>();

        private Dictionary<KeyCode, List<Action<KeyCode>>> eventUpDictonary = new Dictionary<KeyCode, List<Action<KeyCode>>>();

        private static KeyCode specialEnterKey = KeyCode.KeypadEnter;

        private GameObject _collidingObject;
        private GameObject _previousCollidingObject = null;
        private Vector2X mp=new Vector2X();
        public void update()
        {
            if ((Input.anyKey || currentDownKeys.Count>0)&&(eventDownDictonary.Count>0 || eventUpDictonary.Count>0))
            {
                //Debug.Log(Input.anyKey+":"+Input.anyKeyDown);
                updateKey();
                return;
            }


            bool isNew = false;
            MouseEventX mouseEvent;
            Event currentEvent = Event.current;
            if (currentEvent != null && currentEvent.button == 0)
            {
                Vector2 t = currentEvent.mousePosition;
                mp.setTo(t.x, t.y);

                _collidingObject = checkPicker(t);
                isNew = (_previousCollidingObject != _collidingObject);

                if (currentEvent.type == EventType.MouseDown)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_DOWN, mp);

                    if (_previousCollidingObject != null && isNew)
                    {
                        _previousCollidingObject.BroadcastMessage("dispatchEvent",
                            new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }

                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", mouseEvent);
                    }

                    this.dispatchEvent(mouseEvent);
                }
                else if (currentEvent.type == EventType.MouseUp)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_UP, mp);
                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", mouseEvent);
                    }
                    this.dispatchEvent(mouseEvent);
                }
                else if (currentEvent.type == EventType.mouseMove)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_MOVE, mp);

                    if (_collidingObject != null && isNew == false)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", mouseEvent);
                    }

                    this.dispatchEvent(mouseEvent);
                }

                if (_collidingObject != _previousCollidingObject)
                {
                    if (_previousCollidingObject != null)
                    {
                        _previousCollidingObject.BroadcastMessage("dispatchEvent",
                            new MouseEventX(MouseEventX.MOUSE_OUT, mp));
                    }
                    if (_collidingObject != null)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_OVER, mp));
                    }
                }
                else if (_collidingObject != null)
                {
                    _collidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_MOVE, mp));
                }

                _previousCollidingObject = _collidingObject;

                return;
            }

            int touchCount = Input.touchCount;
            if (touchCount < 1)
            {
                if (_previousCollidingObject != null)
                {
                    _previousCollidingObject.BroadcastMessage("dispatchEvent", MouseEventX.MOUSE_UP);
                    _previousCollidingObject = null;
                }
                return;
            }

            Touch touch = Input.touches[0];
            mp.setTo(touch.position.x, touch.position.y);
            _collidingObject = checkPicker(Input.touches[0].position);
            isNew = (_previousCollidingObject != _collidingObject);

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if (_previousCollidingObject != null && isNew)
                    {
                        _previousCollidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }

                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_DOWN, mp));
                    }
                    break;


                case TouchPhase.Canceled:
                case TouchPhase.Ended:

                    if (_previousCollidingObject != null)
                    {
                        _previousCollidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }
                    break;

                case TouchPhase.Moved:

                    if (_collidingObject != null && isNew == false)
                    {
                        _collidingObject.BroadcastMessage("dispatchEvent", new MouseEventX(MouseEventX.MOUSE_MOVE, mp));
                    }

                    break;
            }

            _previousCollidingObject = _collidingObject;
        }


        private GameObject checkPicker(Vector2 pointer)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            return null;
        }

        private HashSet<KeyCode> currentDownKeys=new HashSet<KeyCode>();
        private HashSet<KeyCode> currentUpKeys=new HashSet<KeyCode>(); 
        private List<KeyCode> tempRemove=new List<KeyCode>(); 
       
        private void updateKey()
        {
            tempRemove.Clear();

            foreach (KeyCode code in currentDownKeys)
            {
                if (Input.GetKey(code)) continue;

                tempRemove.Add(code);
                List<Action<KeyCode>> list = null;
                if (eventUpDictonary.TryGetValue(code, out list) == false) continue;

                for (int i = 0; i < list.Count; i++)
                {
                    list[i](code);
                }
            }
            foreach (KeyCode code in tempRemove)
            {
                currentDownKeys.Remove(code);
            }

            bool hasNew = false;
            foreach (string keyString in eventDownDictonary.Keys)
            {
                List<KeyCode> keys = keyCodeMapping[keyString];
                bool success = true;
                int len = keys.Count;
                bool isEnter=keys[len - 1]== specialEnterKey;

                //是否持续触发回调;
                if (isEnter)
                {
                    len = len - 1;
                }

                for (int i = 0; i < len; i++)
                {
                    KeyCode key = keys[i];
                    if (Input.GetKey(key))
                    {
                        if (currentDownKeys.Contains(key) == false)
                        {
                            hasNew = true;
                        }
                        currentDownKeys.Add(key);
                    }
        

                    success &= currentDownKeys.Contains(key);
                }
                //Debug.Log(success+":"+hasNew);
                if (success)
                {
                    if (isEnter || hasNew)
                    {
                        eventDownDictonary[keyString](keys[0]);
                    }
                }
            }

        }


        public bool isKeyDown(KeyCode key)
        {
            return currentDownKeys.Contains(key);
        }


        public void registKeyDown(KeyCode key, Action<KeyCode> func, bool enter=false,bool shift = false, bool ctrl = false, bool alt = false)
        {
            List<KeyCode> list = new List<KeyCode>();
            list.Add(key);

            string keyValue = key.ToString();
            if (shift)
            {
                list.Add(KeyCode.LeftShift);
                keyValue += "#";
            }
            if (alt)
            {
                list.Add(KeyCode.LeftAlt);
                keyValue += "$";
            }
            if (ctrl)
            {
                list.Add(KeyCode.LeftControl);
                keyValue += "&";
            }

            if (enter)
            {
                list.Add(specialEnterKey);
                keyValue += "@";
            }


            List<KeyCode> keyList;
            if (keyCodeMapping.TryGetValue(keyValue, out keyList))
            {
                return;
            }

            keyCodeMapping[keyValue] = list;
            eventDownDictonary[keyValue] = func;
        }

        public void registKeyUp(KeyCode key, Action<KeyCode> func)
        {
            List<Action<KeyCode>> list;
            if (eventUpDictonary.TryGetValue(key,out list)==false)
            {
                list=new List<Action<KeyCode>>();
                eventUpDictonary.Add(key, list);
            }

            if (list.IndexOf(func) == -1)
            {
                list.Add(func);
            }
        }


        public void unregistKeyDown(KeyCode key, Action<KeyCode> func, bool enter = false, bool shift = false,
            bool ctrl = false, bool alt = false)
        {
            string keyValue = key.ToString();
            if (shift)
            {
                keyValue += "#";
            }
            if (alt)
            {
                keyValue += "$";
            }
            if (ctrl)
            {
                keyValue += "&";
            }

            if (enter)
            {
                keyValue += "@";
            }

            List<KeyCode> keyList;
            if (keyCodeMapping.TryGetValue(keyValue, out keyList))
            {
                return;
            }
            keyCodeMapping.Remove(keyValue);
            eventDownDictonary.Remove(keyValue);
        }

        public void unregistKeyUp(KeyCode key, Action<KeyCode> func)
        {
            List<Action<KeyCode>> actions;
            if (eventUpDictonary.TryGetValue(key, out actions))
            {
                if (actions.Contains(func))
                {
                    actions.Remove(func);
                }
            }
        }

        private static InputManager instance;
        public static InputManager getInstance()
        {
            if (instance == null)
            {
                instance=new InputManager();
            }
            return instance;
        }
    }
}
