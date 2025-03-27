using Core.Exceptions;

namespace Core
{
    public interface ICommon_Exception_Factory
    {
        public CoreException Build(long errorId, string message = "");

        public CoreException Build(long errorId, string message_en, string message_th);

        public CoreException Build(long errorId, string message, string message_en, string message_th);

        public CoreException Build(long errorId, string title_en, string message_en, string title_th, string message_th);

        public CoreException Build(long errorId, string message, string title_en, string message_en, string title_th, string message_th);

        public CoreException Build(long errorId, string systemLabel, string message, string title_en, string message_en, string title_th, string message_th);

        /// <summary>
        /// if <paramref name="overrideIfNotEmpty"/> is true, 
        /// Will override systemLabel, message, title_en, message_en, title_th, message_th to the Api_Error model 
        /// if each former param is not empty
        /// otherwise will keep original default message.
        /// </summary>
        public CoreException Build(long errorId, string systemLabel, string message, string title_en, string message_en, string title_th, string message_th, bool overrideIfNotEmpty);
        // =====

        public CoreException Build(int statusCode, long errorId, string message = "");

        // =====

        public CoreException Build(Exception inner, string message = "");
        public CoreException Build(long errorId, Exception inner, string message = "");

        public CoreException Build(string resourceKey);

        CoreException GenerateFatalBCRMException(string message = "");
        CoreException GenerateFatalBCRMException(Exception inner, string message = "");
    }
}
