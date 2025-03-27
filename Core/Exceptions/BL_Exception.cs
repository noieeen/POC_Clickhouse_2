namespace Core.Exceptions
{
    /// <summary>
    /// Business Logic Exception. 
    /// สำหรับสร้าง exception ที่เกี่ยวข้องกับ business logic.
    /// ถ้าต้องการใส่ message for log แต่ไม่ต้องการ show to user ให้ใช้ property Message
    /// </summary>
    public class BL_Exception : AppException
    {
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

        public BL_Exception()
        {
        }

        public BL_Exception(string message)
            : base(message)
        {
        }

        public BL_Exception(string message, Exception inner)
            : base(message, inner)
        {
        }

        public BL_Exception(string messageEn, string messageTh)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }


        public BL_Exception(string systemLabel, string messageEn, string messageTh)
            : base(messageEn)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string message, string systemLabel, string messageEn, string messageTh)
            : base(message)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string message, string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(message, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string message, AlertLevel alertLevel, string systemLabel, string messageEn,
            string messageTh)
            : base(message)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public BL_Exception(string message, AlertLevel alertLevel, string systemLabel, string messageEn,
            string messageTh, Exception inner)
            : base(message, inner)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }
    }
}