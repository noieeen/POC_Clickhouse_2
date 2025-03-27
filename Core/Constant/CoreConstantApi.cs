namespace Core.Constant
{
    public static partial class CoreConstant
    {
        public partial class Api
        {
            public static class Result_Status
            {
                public const string Success = "success";
                public const string Fail = "fail";
                public const string Error = "error";

                public static List<string> GetAllValues()
                {
                    return new List<string>() { Success, Fail, Error };
                }

                public static string ToStringAllValues()
                {
                    return $"{Success}, {Fail}, {Error}";
                }
            }

            public static class Header
            {
                // Api RequestId
                public const string X_Request_Id = "X-Request-Id";
                public const string Authorization = "Authorization";

                // BCRM App
                public const string BCRM_Brand_Ref = "bcrm-brand-ref";
                public const string BCRM_App_Id = "bcrm-app-id";
                public const string BCRM_App_Secret = "bcrm-app-secret";

                public const string Accept_Language = "Accept-Language";
            }

            public static class RouteData
            {
                public static class Key
                {
                    public const string CDP_Identity_Context = "BCRM_Identity_Context";

                    public const string Api_Header_RequestId = "Api_Header_RequestId";

                    public const string Api_JWT_Token_Data = "Api_JWT_Token_Data";

                    public const string Api_Token_BrandId = "Api_Req_BrandId";
                    public const string Api_Token_Brand_Ref = "Api_Req_Brand_Ref";

                    public const string Api_Token_AppId = "Api_Req_AppId";
                    public const string Api_Token_App_Id = "Api_Req_App_Id";

                    public const string Api_Token_IdentityId = "Api_Req_IdentityId";
                    public const string Api_Token_Identity_SRef = "Api_Req_Identity_SRef";
                }
            }

            public static class Filter
            {
                [Flags]
                public enum CC_HttpMethods
                {
                    Get = 1,
                    Post = 2,
                    Put = 4,
                    Delete = 8,
                    Patch = 16,
                    Options = 32
                }
            }
        }

        // TODO: Delete code after 2022-06-30
        // This class has moved to CDP_Core_Const.Database.QueryTimeout
        //public static class Service
        //{
        //    public static class Query
        //    {
        //        public const int TimeOut = 300;
        //    }
        //}
    }
}
