using System.Collections;
using System.Collections.Generic;

namespace foundation
{
    internal class AMF3ObjectVectorWriter : IAMFWriter
    {
        #region IAMFWriter Members

        public bool IsPrimitive
        {
            get { return false; }
        }

        public void WriteData(AMFWriter writer, object data)
        {
            writer.WriteByte(AMF3TypeCode.ObjectVector);
            writer.WriteAMF3ObjectVector(data as IList);
        }

        #endregion
    }
}
