using System.Reflection;

namespace Core.Utils
{
    public class ReflectionUtil
    {
        /// <summary>
        /// Get property type of property of T class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Type GetType<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName).PropertyType;
        }

        /// <summary>
        /// Get public const field info of Type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<FieldInfo> GetConstantsFieldInfo<T>()
        {
            Type type = typeof(T);
            List<FieldInfo> list = GetConstantsFieldInfo(type);
            return list;
        }

        /// <summary>
        /// Get public const field info of input Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetConstantsFieldInfo(Type type)
        {
            List<FieldInfo> list = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                                        .ToList();
            return list;
        }
    }
}
