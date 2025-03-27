using Newtonsoft.Json.Linq;

namespace Core.Api.Model
{
    /// <summary>
    /// Model for api error response
    /// </summary>
    public class Api_Error
    {
        public long Code { get; set; }
        public string SubCode { get; set; } = string.Empty;
        public int Http_status { get; set; }
        
        public string System { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public Dictionary<string, Api_Locale_Error_Message> Locale { get; set; } = new Dictionary<string, Api_Locale_Error_Message>();

        public Validate_Errors? Validate { get; set; }

        // Locale Key
        public const string LOCALE_KEY_TH = "th-TH";
        public const string LOCALE_KEY_EN = "en-US";

        public Api_Error()
        {

        }

        public Api_Error(JObject rawData)
        {
            if (rawData == null) return;

            try { this.Code = (long)rawData["code"]; } catch (Exception ex) { }
            try { this.Http_status = (int)rawData["http_status"]; } catch (Exception ex) { }

            try { this.System = (string)rawData["system"]; } catch (Exception ex) { }
            try { this.Message = (string)rawData["message"]; } catch (Exception ex) { }

            // Locale
            this.Locale = new Dictionary<string, Api_Locale_Error_Message>();
            try
            {
                JObject locales = (JObject)rawData["locale"];
                foreach (var locale in locales)
                {
                    try
                    {                        
                        String localeKey = locale.Key;
                        
                        String title = (string)locale.Value["title"];
                        String message = (string)locale.Value["message"];

                        Api_Locale_Error_Message locale_Msg = new Api_Locale_Error_Message
                        {
                            Title = title,
                            Message = message
                        };

                        this.Locale[localeKey] = locale_Msg;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public Api_Error(Api_ErrorCode api_ErrorCode) 
            : this(api_ErrorCode, "")
        {
        }

        public Api_Error (Api_ErrorCode api_ErrorCode, string message)
        {
            Code = api_ErrorCode.ErrorId;
            Http_status = api_ErrorCode.Default_HTTP_Status;

            System = api_ErrorCode.System_Label;
            Message = message;

            // Locale
            Locale = new Dictionary<string, Api_Locale_Error_Message>();

            // Locale: th-TH
            Api_Locale_Error_Message errorLangTh = new Api_Locale_Error_Message();
            errorLangTh.Title = api_ErrorCode.Error_Title_Th;
            errorLangTh.Message = api_ErrorCode.Error_Message_Th;

            Locale.Add(LOCALE_KEY_TH, errorLangTh);

            // Locale: en-EN
            Api_Locale_Error_Message errorLangEn = new Api_Locale_Error_Message();
            errorLangEn.Title = api_ErrorCode.Error_Title_En;
            errorLangEn.Message = api_ErrorCode.Error_Message_En;

            Locale.Add(LOCALE_KEY_EN, errorLangEn); 
        }

        public Api_Error(Api_ErrorCode api_ErrorCode, string subCode, string message) : this(api_ErrorCode, message)
        {
            this.SubCode = subCode;
        }
    }

    
}
