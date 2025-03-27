namespace Core.Exceptions
{
    /// <summary>
    /// Base class for Custom app exception.
    /// ปัจจุบันความสามารถเทียบเท่า class Exception แต่แค่อยากให้มี parent class ของ custom exception 
    /// มาคั่นระหว่าง class Exception กับ Custom-Exception เท่านั้น
    /// </summary>
    public class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message)
            : base(message)
        {
        }

        public AppException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
