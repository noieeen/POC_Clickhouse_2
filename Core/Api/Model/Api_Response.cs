using Newtonsoft.Json.Linq;

namespace Core.Api.Model
{
    /// <summary>
    /// Model for api response
    /// </summary>
    public class Api_Response
    {
        public string? Request_Id { get; set; } = "";

        // Api - Response Status
        public string Status { get; set; }

        public object? Data { get; set; } = null;

        public Api_Error Error { get; set; }

        public Api_Response()
        {
        }

        public Api_Response(JObject rawData)
        {
            if (rawData == null) return;

            try
            {
                this.Request_Id = (string)rawData["request_id"];
            }
            catch (Exception ex)
            {
            }

            try
            {
                this.Status = (string)rawData["status"];
            }
            catch (Exception ex)
            {
            }

            try
            {
                this.Data = (JObject)rawData["data"];
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (rawData.ContainsKey("error"))
                {
                    this.Error = new Api_Error((JObject)rawData["error"]);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}