using System;
using UnityEngine;

namespace foundation
{
    
    public class MathExtendUtils
    {
        public const float TWO_PI = (float)Math.PI * 2;
        /** Moves a radian angle into the range [-PI, +PI], while keeping the direction intact. */
        public static float normalizeAngle(float angle)
        {
            // move to equivalent value in range [0 deg, 360 deg] without a loop
            angle = angle % 360f;

            // move to [-180 deg, +180 deg]
            if (angle < -180f) angle += 360f;
            if (angle > 180f) angle -= 360f;

            return angle;
        }


        public static float random
        {
            get
            {
                return UnityEngine.Random.Range(0, 10)/10.0f;
            }
        }


        /// <summary>
        /// 截取保留小数点后 n 位.
        /// </summary>
        /// <param name='num'>
        /// 原数据 num.
        /// </param>
        /// <param name='n'>
        /// 保留小数点后 n 位.
        /// </param>
        public static float Round(float num, int n)
        {
            float temp = num * Mathf.Pow(10, n);
            return Mathf.Round(temp) / Mathf.Pow(10, n);
        }

        /// <summary>
        /// 判断两个 Vector3 是否相等
        /// </summary>
        /// <param name="pos">Vector3</param>
        /// <param name="n">对比到小数后多少位</param>
        /// <returns></returns>
        public static bool campareVector3(Vector3 pos1, Vector3 pos2, int n = 2)
        {
            if (MathExtendUtils.Round(pos1.x, n) == MathExtendUtils.Round(pos2.x, n)
                && MathExtendUtils.Round(pos1.y, n) == MathExtendUtils.Round(pos2.y, n)
                && MathExtendUtils.Round(pos1.z, n) == MathExtendUtils.Round(pos2.z, n)
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得 2D 空间两点坐标之间的弧度.
        /// </summary>
        public static float Get2DRadianOfTwoPos(Vector2 startPos, Vector2 targetPos)
        {
            return Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        }

        /// <summary>
        /// 获得 2D 空间两点坐标之间的夹角.
        /// </summary>
        public static float Get2DAngleOfTwoPos(Vector2 startPos, Vector2 targetPos)
        {
            //	Mathf.Rad2Deg = 360 / (PI * 2) = 180 / PI;
            return Mathf.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x) * Mathf.Rad2Deg;
        }

    }


}
