namespace Core.Exceptions
{
    public class DataNotFoundException : BL_Exception
    {
        public DataNotFoundException()
        {
        }

        public DataNotFoundException(string message)
            : base(message)
        {
        }

        public DataNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public DataNotFoundException(string messageEn, string messageTh)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            Message_En = messageEn;
            Message_Th = messageTh;
        }


        public DataNotFoundException(string systemLabel, string messageEn, string messageTh)
            : base(messageEn)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(messageEn, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string message, string systemLabel, string messageEn, string messageTh)
            : base(message)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string message, string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(message, inner)
        {
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string message, AlertLevel alertLevel, string systemLabel, string messageEn, string messageTh)
            : base(message)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }

        public DataNotFoundException(string message, AlertLevel alertLevel, string systemLabel, string messageEn, string messageTh, Exception inner)
            : base(message, inner)
        {
            AlertLevel = alertLevel;
            SystemLabel = systemLabel;
            Message_En = messageEn;
            Message_Th = messageTh;
        }
    }
}
