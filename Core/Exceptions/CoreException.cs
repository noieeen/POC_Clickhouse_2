using Core.Api.Model;

namespace Core.Exceptions
{
    public class CoreException : Exception
    {
        public int Http_StatusCode { get; set; }
        public Api_Error ApiError { get; set; }

        public CoreException() { }

        public CoreException(int statusCode, Api_ErrorCode error, string message = "") 
            : base(message)
        {
            this.Http_StatusCode = statusCode;
            
            Api_Error apiError = new Api_Error(error, message);
            this.ApiError = apiError;
        }

        public CoreException(Api_ErrorCode error, string message = "")
            : base(message)
        {
            this.Http_StatusCode = error.Default_HTTP_Status;

            Api_Error apiError = new Api_Error(error, message);
            this.ApiError = apiError;            
        }

        public CoreException(int statusCode, Api_ErrorCode error, Exception inner, string message = "")
            : base(message, inner)
        {
            this.Http_StatusCode = statusCode;

            Api_Error apiError = new Api_Error(error, message);
            this.ApiError = apiError;
        }

        public CoreException(Api_ErrorCode error, Exception inner, string message = "")
            : base(message, inner)
        {
            this.Http_StatusCode = error.Default_HTTP_Status;

            Api_Error apiError = new Api_Error(error, message);
            this.ApiError = apiError;            
        }
    }
}
