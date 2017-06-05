using System;
using System.Xml;
using System.ComponentModel;
using System.Collections;
using System.Reflection;

namespace foundation
{
    public sealed class TypeHelper
	{
		static TypeHelper()
		{
        }

        static public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        static public Type Locate(string typeName)
		{
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            Assembly[] assemblies = GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				Type type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
			}
			return null;
		}


        public static object CreateInstance(Type type)
        {
            //Is this a generic type definition?
            if (ReflectionUtils.IsGenericType(type))
            {
                Type genericTypeDefinition = ReflectionUtils.GetGenericTypeDefinition(type);
                // Get the generic type parameters or type arguments.
                Type[] typeParameters = ReflectionUtils.GetGenericArguments(type);

                Type constructed = ReflectionUtils.MakeGenericType(genericTypeDefinition, typeParameters);
                object obj = Activator.CreateInstance(constructed);
                if (obj == null)
                {
                    throw new Exception("CreateInstance" + type.ToString());
                }
                return obj;
            }
            return Activator.CreateInstance(type);
        }

        static public object ChangeType(object value, Type targetType)
        {
            return ConvertChangeType(value, targetType, ReflectionUtils.IsNullable(targetType));
        }

        private static object ConvertChangeType(object value, Type targetType, bool isNullable)
        {
            if (targetType.IsArray)
            {
                if (null == value)
                {
                    return null;
                }
                Type srcType = value.GetType();

                if (srcType == targetType)
                {
                    return value;
                }

                if (srcType.IsArray)
                {
                    Type srcElementType = srcType.GetElementType();
                    Type dstElementType = targetType.GetElementType();

                    if (srcElementType.IsArray != dstElementType.IsArray
                        || (srcElementType.IsArray &&
                            srcElementType.GetArrayRank() != dstElementType.GetArrayRank()))
                    {
                        throw new InvalidCastException(string.Format("Can not convert array of type '{0}' to array of '{1}'.", srcType.FullName, targetType.FullName));
                    }

                    Array srcArray = (Array)value;
                    Array dstArray;

                    int rank = srcArray.Rank;

                    if (rank == 1 && 0 == srcArray.GetLowerBound(0))
                    {
                        int arrayLength = srcArray.Length;

                        dstArray = Array.CreateInstance(dstElementType, arrayLength);

                        // Int32 is assignable from UInt32, SByte from Byte and so on.
                        //
                        if (dstElementType.IsAssignableFrom(srcElementType))
                            Array.Copy(srcArray, dstArray, arrayLength);
                        else
                            for (int i = 0; i < arrayLength; ++i)
                                dstArray.SetValue(ConvertChangeType(srcArray.GetValue(i), dstElementType, isNullable), i);
                    }
                    else
                    {
                        int arrayLength = 1;
                        int[] dimensions = new int[rank];
                        int[] indices = new int[rank];
                        int[] lbounds = new int[rank];

                        for (int i = 0; i < rank; ++i)
                        {
                            arrayLength *= (dimensions[i] = srcArray.GetLength(i));
                            lbounds[i] = srcArray.GetLowerBound(i);
                        }

                        dstArray = Array.CreateInstance(dstElementType, dimensions, lbounds);
                        for (int i = 0; i < arrayLength; ++i)
                        {
                            int index = i;
                            for (int j = rank - 1; j >= 0; --j)
                            {
                                indices[j] = index % dimensions[j] + lbounds[j];
                                index /= dimensions[j];
                            }

                            dstArray.SetValue(ConvertChangeType(srcArray.GetValue(indices), dstElementType, isNullable), indices);
                        }
                    }
                    return dstArray;
                }
                return null;
            }

            if (targetType.IsEnum)
            {
                 return Enum.Parse(targetType, value.ToString(), true);
            }

            if (value == null)
            {
                return null;
            }
            if (targetType.IsAssignableFrom(value.GetType()))
            {
                return value;//Skip further adapting;
            }


            if (isNullable)
            {
                switch (Type.GetTypeCode(GetUnderlyingType(targetType)))
                {
                    case TypeCode.Boolean: return (Boolean)(value);
                    case TypeCode.Byte: return (Byte)(value);
                    case TypeCode.DateTime: return (DateTime)(value);
                  
                    case TypeCode.Double: return (double)(value);
                    case TypeCode.Int16: return (Int16)(value);
                    case TypeCode.Int32: return (Int32)(value);
                    case TypeCode.Int64: return (Int64)(value);
                    case TypeCode.SByte: return (SByte)(value);
                    case TypeCode.Single: return (Single)(value);
                    case TypeCode.UInt16: return (UInt16)(value);
                    case TypeCode.UInt32: return (UInt32)(value);
                    case TypeCode.UInt64: return (UInt64)(value);
                }
            }
            TypeCode code = Type.GetTypeCode(targetType);
            switch (code)
            {
                case TypeCode.Boolean: return (Boolean)(value);
                case TypeCode.Byte: return (Byte)(value);
                case TypeCode.Char: return (Char)(value);
                case TypeCode.DateTime:
                    return (DateTime)(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);
                case TypeCode.Double: 
                    return Convert.ToDouble(value);
                case TypeCode.Int16: return Convert.ToInt16(value);
                case TypeCode.Int32: return Convert.ToInt32(value);
                case TypeCode.Int64: return Convert.ToInt64(value);
                case TypeCode.SByte: return Convert.ToSByte(value);
                case TypeCode.Single: return Convert.ToSingle(value);
                case TypeCode.String: return value.ToString();
                case TypeCode.UInt16: return Convert.ToUInt16(value);
                case TypeCode.UInt32: return Convert.ToUInt32(value);
                case TypeCode.UInt64: return Convert.ToUInt64(value);
            }

            if (typeof(XmlDocument) == targetType) return (XmlDocument)(value);
            if (typeof(byte[]) == targetType) return (byte[])(value);
            if (typeof(char[]) == targetType) return (char[])(value);

            if (value == null)
                return null;
            //Check whether the target Type can be assigned from the value's Type
            if (targetType.IsAssignableFrom(value.GetType()))
            {
                return value; //Skip further adapting
            }

            TypeConverter typeConverter = ReflectionUtils.GetTypeConverter(targetType);// TypeDescriptor.GetConverter(targetType);
            if (typeConverter != null && typeConverter.CanConvertFrom(value.GetType()))
            {
                return typeConverter.ConvertFrom(value);
            }

            if (targetType.IsInterface)
            {
                MethodInfo castMethod = typeof(TypeHelper).GetMethod("Cast", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(targetType);
                object castedObject = castMethod.Invoke(null, new object[] { value });
                if (castedObject != null)
                {
                    return castedObject;
                }
            }

            //Collections
            if (ReflectionUtils.ImplementsInterface(targetType, "System.Collections.Generic.ICollection`1") && value is IList)
            {
                object obj = null;
                if (CollectionUtils.IsListType(targetType))
                    obj = CollectionUtils.CreateList(targetType);
                if (obj == null)
                    obj = CreateInstance(targetType);
                if (obj != null)
                {
                    //For generic interfaces, the name parameter is the mangled name, ending with a grave accent (`) and the number of type parameters
                    Type[] typeParameters = ReflectionUtils.GetGenericArguments(targetType);
                    if (typeParameters != null && typeParameters.Length == 1)
                    {
                        //For generic interfaces, the name parameter is the mangled name, ending with a grave accent (`) and the number of type parameters
                        Type typeGenericICollection = targetType.GetInterface("System.Collections.Generic.ICollection`1", true);
                        MethodInfo miAddCollection = typeGenericICollection.GetMethod("Add");
                        IList source = value as IList;
                        for (int i = 0; i < (value as IList).Count; i++)
                            miAddCollection.Invoke(obj, new object[] { ChangeType(source[i], typeParameters[0]) });
                    }
                    return obj;
                }
            }
            if (ReflectionUtils.ImplementsInterface(targetType, "System.Collections.IList") && value is IList)
            {
                object obj = CreateInstance(targetType);
                if (obj != null)
                {
                    IList source = value as IList;
                    IList destination = obj as IList;
                    for (int i = 0; i < source.Count; i++)
                        destination.Add(source[i]);
                    return obj;
                }
            }
            if (ReflectionUtils.ImplementsInterface(targetType, "System.Collections.Generic.IDictionary`2") && value is IDictionary)
            {
                object obj = CreateInstance(targetType);
                if (obj != null)
                {
                    IDictionary source = value as IDictionary;
                    Type[] typeParameters = ReflectionUtils.GetGenericArguments(targetType);
                    if (typeParameters != null && typeParameters.Length == 2)
                    {
                        //For generic interfaces, the name parameter is the mangled name, ending with a grave accent (`) and the number of type parameters
                        Type typeGenericIDictionary = targetType.GetInterface("System.Collections.Generic.IDictionary`2", true);
                        MethodInfo miAddCollection = typeGenericIDictionary.GetMethod("Add");
                        IDictionary dictionary = value as IDictionary;
                        foreach (DictionaryEntry entry in dictionary)
                        {
                            miAddCollection.Invoke(obj, new object[] {
                                ChangeType(entry.Key, typeParameters[0]),
                                ChangeType(entry.Value, typeParameters[1])
                            });
                        }
                    }
                    return obj;
                }
            }

            if (ReflectionUtils.ImplementsInterface(targetType, "System.Collections.IDictionary") && value is IDictionary)
            {
                object obj = CreateInstance(targetType);
                if (obj != null)
                {
                    IDictionary source = value as IDictionary;
                    IDictionary destination = obj as IDictionary;
                    foreach (DictionaryEntry entry in source)
                        destination.Add(entry.Key, entry.Value);
                    return obj;
                }
            }

            return System.Convert.ChangeType(value, targetType, null);
        }

         public static Type GetUnderlyingType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (ReflectionUtils.IsNullable(type))
				type = type.GetGenericArguments()[0];
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            return type;
        }

    }
}
