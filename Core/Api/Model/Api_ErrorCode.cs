namespace Core.Api.Model
{
    /// <summary>
    /// Model class for store single error code.
    /// </summary>
    public class Api_ErrorCode
    {
        public long ErrorId { get; set; }
        public int Revision { get; set; }
        public string Namespace { get; set; } = "";
        public string Namespace_Code { get; set; } = "";
        public string Module { get; set; } = "";
        public string Module_Code { get; set; } = "";
        public string Action { get; set; } = "";
        public string Action_Code { get; set; } = "";
        public int Default_HTTP_Status { get; set; }
        public string System_Label { get; set; } = "";
        public string Error_Title_Th { get; set; } = "";
        public string Error_Message_Th { get; set; } = "";
        public string Error_Title_En { get; set; } = "";
        public string Error_Message_En { get; set; } = "";
        public string Detail { get; set; } = "";
    }
}
