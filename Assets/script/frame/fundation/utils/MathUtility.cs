using UnityEngine;

namespace foundation
{
    public class MathUtility
    {
        public float MIN = 0.01f;
        /// <summary>
        /// 计算范围
        /// </summary>
        /// <param name="TopLeft"></param>
        /// <param name="BottomRight"></param>
        /// <returns></returns>
        public static Bounds caculateBounds(Vector2 TopLeft, Vector2 BottomRight)
        {
            Vector3[] points = new Vector3[5];
            points[0] = new Vector2(TopLeft.x, TopLeft.y);
            points[1] = new Vector2(BottomRight.x, TopLeft.y);
            points[2] = new Vector2(BottomRight.x, BottomRight.y);
            points[3] = new Vector2(TopLeft.x, BottomRight.y);
            points[4] = new Vector2(TopLeft.x, TopLeft.y);
            float width = Vector2.Distance(points[0], points[1]);
            float height = Vector2.Distance(points[1], points[2]);
            return new Bounds(new Vector2((float)System.Math.Round((points[0].x + points[1].x) / 2, 2), (float)System.Math.Round((points[0].y + points[2].y) / 2, 2)), new Vector2((float)System.Math.Round(width, 2), (float)System.Math.Round(height, 2)));
        }
    }
}
