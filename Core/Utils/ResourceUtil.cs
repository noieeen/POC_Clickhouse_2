using System.Reflection;

namespace Core.Utils
{
    public class ResourceUtil
    {
        public static List<Type> ListRegisteredTypes = new List<Type>();

        /// <summary>
        /// Store key-value of const field. ie.<br/>
        /// - public const string varName = "varValue" <br/>
        /// key = varName <br/>
        /// value = varValue
        /// </summary>
        private static Dictionary<string, string> DictResValues = new Dictionary<string, string>();

        private static bool IsInit = false;


        public static void RegisteredType(Type type)
        {
            ListRegisteredTypes.Add(type);
        }

        public static void Init()
        {
            DictResValues.Clear();

            foreach (var type in ListRegisteredTypes)
            {
                List<FieldInfo> list = ReflectionUtil.GetConstantsFieldInfo(type);

                list.ForEach(field =>
                {
                    string value = field.GetRawConstantValue()!.ToString() ?? "";
                    DictResValues.Add(field.Name, value);
                });
            }
        }

        public static void LazyInit()
        {
            if (!IsInit) Init();
        }

        public static string GetMessage(string resourceKey)
        {
            LazyInit();
            string key = resourceKey;
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        public static int GetErrorCode(string resourceKey, int defaultValue = 1_000_422)
        {
            LazyInit();
            string key = $"{resourceKey}_Code";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            int resultInt;
            if (!int.TryParse(result, out resultInt))
                resultInt = defaultValue;

            return resultInt;
        }

        public static string GetErrorCodeStr(string resourceKey)
        {
            LazyInit();
            string key = $"{resourceKey}_Code";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        public static string GetHttpStatusCode(string resourceKey)
        {
            LazyInit();
            string key = $"{resourceKey}_Http";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        public static string GetTitle(string resourceKey)
        {
            LazyInit();
            string key = $"{resourceKey}_Title_En";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="lang">2 chars language code</param>
        /// <returns></returns>
        public static string GetTitle(string resourceKey, string lang)
        {
            LazyInit();
            string langClean = char.ToUpper(lang[0]) + lang.Substring(1);

            string key = $"{resourceKey}_Title_{langClean}";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        public static string GetMessageDetail(string resourceKey)
        {
            LazyInit();
            string key = $"{resourceKey}_Msg_En";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="lang">2 chars language code</param>
        /// <returns></returns>
        public static string GetMessageDetail(string resourceKey, string lang)
        {
            LazyInit();
            string langClean = char.ToUpper(lang[0]) + lang.Substring(1);

            string key = $"{resourceKey}_Msg_{langClean}";
            string result = "";
            if (DictResValues.ContainsKey(key))
                result = DictResValues[key];

            return result;
        }

    }
}
