/*
 * Transform2D is a 2d matrix contribute for rotation/translation/scaling
 * component structure:
 *  | a c tx | x  
 *  | b d ty | y
 *  | u v w  | 1
 */

using UnityEngine;

namespace foundation
{

    public class Transform2X
    {
        public Transform2X()
        {

        }

        public Transform2X(float a, float b, float c, float d, float tx, float ty, float u, float v, float w)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
            this.u = u;
            this.v = v;
            this.w = w;
        }

        public Transform2X(float a, float b, float c, float d, float tx, float ty)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
        }

        public float a = 1.0f;

        public float b = 0.0f;

        public float c = 0.0f;

        public float d = 1.0f;

        public float tx = 0.0f;

        public float ty = 0.0f;

        public float u = 0.0f;

        public float v = 0.0f;

        public float w = 1.0f;


        public void identity()
        {
            a = 1.0f;
            b = 0.0f;
            c = 0.0f;
            d = 1.0f;
            tx = 0.0f;
            ty = 0.0f;
            u = 0.0f;
            v = 0.0f;
            w = 1.0f;
        }


        public override string ToString()
        {

            return string.Format(a + "," + c + "," + tx + "," + b + "," + d + "," + ty + "," + u + "," + c + "," + w);
        }

        public void scale(float x, float y)
        {
            a = a*x;
            b = b*x;
            c = c*y;
            d = d*y;
        }

        public void translate(float x, float y = 0.0f)
        {
            tx += x;
            ty += y;
        }

        public void copyFrom(Transform2X transform)
        {
            this.a = transform.a;
            this.b = transform.b;
            this.c = transform.c;
            this.d = transform.d;

            this.tx = transform.tx;
            this.ty = transform.ty;
        }

        public void rotate(float rad)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float ra = a*cos + c*sin;
            float rc = -a*sin + c*cos;
            float rb = b*cos + d*sin;
            float rd = -b*sin + d*cos;

            a = ra;
            b = rb;
            c = rc;
            d = rd;
        }




        public Transform2X clone()
        {
            return (new Transform2X(a, b, c, d, tx, ty, u, v, w));
        }

        public void invert()
        {
            float A = d;
            float B = -b;
            //float C = 0;
            float D = -c;
            float E = a;
            //float F = 0;
            float G = c*ty - tx*d;
            float H = tx*b - a*ty;
            float K = a*d - c*b;
            float det = 1/(a*A + c*B);
            a = det*A;
            b = det*B;
            c = det*D;
            d = det*E;
            tx = det*G;
            ty = det*H;
            u = 0; //det * C;
            v = 0; //det * F;
            w = det*K;
            /*
		 * | A D G |
		 * | B E H |
		 * | C F K |
		 * 
		 */
        }

        public Transform2X getInverted()
        {
            Transform2X result = new Transform2X(a, b, c, d, tx, ty);
            result.invert();
            return (result);
        }

        public Rect getBoundRect(Vector2 minPos, Vector2 maxPos)
        {
            Rect result = new Rect();
            if (maxPos.x - minPos.x < 0 || maxPos.y - minPos.y < 0)
            {
                return (result);
            }
            Vector2X p1 = transformVector(new Vector2X(minPos.x, minPos.y));
            Vector2X p2 = transformVector(new Vector2X(maxPos.x, minPos.y));
            Vector2X p3 = transformVector(new Vector2X(maxPos.x, maxPos.y));
            Vector2X p4 = transformVector(new Vector2X(minPos.x, maxPos.y));

            Vector2X minPos2 = new Vector2X();
            Vector2X maxPos2 = new Vector2X();
            minPos2.x = p1.x;
            if (minPos2.x > p2.x)
            {
                minPos2.x = p2.x;
            }
            if (minPos2.x > p3.x)
            {
                minPos2.x = p3.x;
            }
            if (minPos2.x > p4.x)
            {
                minPos2.x = p4.x;
            }
            minPos2.y = p1.y;
            if (minPos2.y > p2.y)
            {
                minPos2.y = p2.y;
            }
            if (minPos2.y > p3.y)
            {
                minPos2.y = p3.y;
            }
            if (minPos2.y > p4.y)
            {
                minPos2.y = p4.y;
            }

            maxPos2.x = p1.x;
            if (maxPos2.x < p2.x)
            {
                maxPos2.x = p2.x;
            }
            if (maxPos2.x < p3.x)
            {
                maxPos2.x = p3.x;
            }
            if (maxPos2.x < p4.x)
            {
                maxPos2.x = p4.x;
            }
            maxPos2.y = p1.y;
            if (maxPos2.y < p2.y)
            {
                maxPos2.y = p2.y;
            }
            if (maxPos2.y < p3.y)
            {
                maxPos2.y = p3.y;
            }
            if (maxPos2.y < p4.y)
            {
                maxPos2.y = p4.y;
            }

            result.x = minPos2.x;
            result.y = minPos2.y;
            result.width = (maxPos2.x - minPos2.x);
            result.height = (maxPos2.y - minPos2.y);
            return (result);
        }

        public RectangleX getBoundRect(RectangleX rect)
        {
            RectangleX result = new RectangleX();
            if (rect.width < 0 || rect.height < 0)
            {
                return result;
            }
            Vector2X p1 = transformVector(new Vector2X(rect.x, rect.y));
            Vector2X p2 = transformVector(new Vector2X(rect.x + rect.width, rect.y));
            Vector2X p3 = transformVector(new Vector2X(rect.x + rect.width, rect.y + rect.height));
            Vector2X p4 = transformVector(new Vector2X(rect.x, rect.y + rect.height));

            Vector2X minPos2 = new Vector2X();
            Vector2X maxPos2 = new Vector2X();
            minPos2.x = p1.x;
            if (minPos2.x > p2.x)
            {
                minPos2.x = p2.x;
            }
            if (minPos2.x > p3.x)
            {
                minPos2.x = p3.x;
            }
            if (minPos2.x > p4.x)
            {
                minPos2.x = p4.x;
            }
            minPos2.y = p1.y;
            if (minPos2.y > p2.y)
            {
                minPos2.y = p2.y;
            }
            if (minPos2.y > p3.y)
            {
                minPos2.y = p3.y;
            }
            if (minPos2.y > p4.y)
            {
                minPos2.y = p4.y;
            }

            maxPos2.x = p1.x;
            if (maxPos2.x < p2.x)
            {
                maxPos2.x = p2.x;
            }
            if (maxPos2.x < p3.x)
            {
                maxPos2.x = p3.x;
            }
            if (maxPos2.x < p4.x)
            {
                maxPos2.x = p4.x;
            }
            maxPos2.y = p1.y;
            if (maxPos2.y < p2.y)
            {
                maxPos2.y = p2.y;
            }
            if (maxPos2.y < p3.y)
            {
                maxPos2.y = p3.y;
            }
            if (maxPos2.y < p4.y)
            {
                maxPos2.y = p4.y;
            }

            result.x = minPos2.x;
            result.y = minPos2.y;
            result.width = (maxPos2.x - minPos2.x);
            result.height = (maxPos2.y - minPos2.y);
            return result;
        }

        public Vector2X transformVector(Vector2X vec,Vector2X result=null)
        {
            if (result == null)
            {
                result=new Vector2X();
            }
            result.x = a*vec.x + c*vec.y + tx;
            result.y = b* vec.x + d*vec.y + ty;
            return result;
        }

        public Vector3X transformVector(Vector3X vec, Vector3X result=null)
        {
            if (result == null)
            {
                result = new Vector3X();
            }
            result.x = a*vec.x + c*vec.y + tx;
            result.y = b*vec.x + d*vec.y + ty;
            result.z = vec.z;
            return result;
        }

        public static Transform2X multiply(Transform2X mtx1, Transform2X mtx2, Transform2X result = null)
        {
            if (result == null)
            { 
                result = new Transform2X();
            }

            result.a = mtx1.a*mtx2.a + mtx1.c*mtx2.b;
            result.c = mtx1.a*mtx2.c + mtx1.c*mtx2.d;
            result.tx = mtx1.a*mtx2.tx + mtx1.c*mtx2.ty + mtx1.tx;

            result.b = mtx1.b*mtx2.a + mtx1.d*mtx2.b;
            result.d = mtx1.b*mtx2.c + mtx1.d*mtx2.d;
            result.ty = mtx1.b*mtx2.tx + mtx1.d*mtx2.ty + mtx1.ty;

            result.u = 0;
            result.v = 0;
            result.w = 1;

            return result;
        }


    }
}