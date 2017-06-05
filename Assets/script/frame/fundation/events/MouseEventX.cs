using foundation;
using UnityEngine;


public class MouseEventX : EventX
    {
        public const string CLICK = "item_click";
        public const string PRESS = "item_press";

        public const string MOUSE_DOWN = "mouse down";
        public const string MOUSE_UP = "mouse up";
        public const string MOUSE_OUT = "mouse out";
        public const string MOUSE_OVER = "mouse over";
        public const string MOUSE_LEAVE = "mouse_leave";

        public Vector2 stagePosition;
  
        public const string MOUSE_MOVE = "mouse move";

        public Touch touch { get; internal set; }

        public MouseEventX(string type) : base(type)
        {
        }

        public MouseEventX(string type,Vector2 v) : base(type)
        {
            stagePosition = v;
        }

        public MouseEventX(string type, Vector2X v) : base(type)
        {
            stagePosition = new Vector2(v.x,v.y);
        }
    }

