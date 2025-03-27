namespace Core.Api.Model
{
    // Model - Validation
    public class Validate_Errors
    {
        public List<Field_Error> Fields { get; set; } = new List<Field_Error>();
    }

    public class Field_Error
    {
        public string Key { get; set; } = "";
        public object? Val_Result { get; set; }
        public string Message { get; set; } = "";
    }
}
