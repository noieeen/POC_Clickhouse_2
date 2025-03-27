namespace Core.Exceptions
{
    /// <summary>
    /// ทำให้ return api response code 400 <br/>
    /// Recommended for case want to return BadRequest (http status code = 400) to client.<br/>
    /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
    /// </summary>
    public class BadRequestException : AppException
    {
        protected Dictionary<string, string> _dictParamMessages = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary of (paramName, message)
        /// </summary>
        public Dictionary<string, string> ParamMessages 
        { 
            get { return new Dictionary<string, string>(_dictParamMessages); } 
        }

        /// <summary>
        /// Type of dialog to display.
        /// </summary>
        public AlertLevel AlertLevel { get; protected set; }
        /// <summary>
        /// Recommended to be unique string ex. Invalid_User_Pass
        /// </summary>
        public string SystemLabel { get; protected set; } = "";
        /// <summary>
        /// For show dialog
        /// </summary>
        public string Message_En { get; protected set; } = "";
        /// <summary>
        /// For show dialog
        /// </summary>
        public string Message_Th { get; protected set; } = "";

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client.<br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException()
        { 
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string messageEn, string messageTh)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string systemLabel, string messageEn, string messageTh)
            : base(messageEn)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message, string systemLabel, string messageEn, string messageTh)
            : base(message)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message, string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(message, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message, AlertLevel alertLevel, string systemLabel, string messageEn, string messageTh)
            : base(message)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        /// <summary>
        /// Recommended for case want to return BadRequest (http status code = 400) to client. <br/>
        /// Otherwise if want to return Unprocessable (http status code = 422) used <see cref="AppException"/> instead.
        /// </summary>
        public BadRequestException(string message, AlertLevel alertLevel, string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(message, inner)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BadRequestException AddError(string paramName, string message)
        {
            string paramNameTrimed = paramName.Trim();
            if (!_dictParamMessages.ContainsKey(paramNameTrimed))
                _dictParamMessages.Add(paramNameTrimed, message);
            else
                _dictParamMessages[paramNameTrimed] = message;

            return this;
        }

        public string GetErrorMessage(string paramName)
        {
            string paramNameTrimed = paramName.Trim();
            if (!_dictParamMessages.ContainsKey(paramNameTrimed))
                return "";
            else
            {
                string msg = _dictParamMessages[paramNameTrimed];
                return msg;
            }
        }
    }
}
