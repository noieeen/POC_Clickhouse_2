using Core.Api.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Api.Response
{
    public class ModelState_ResultBuilder
    {
        // Default MVC Model State
        public ModelStateDictionary ModelState { get; set; }        

        /// <summary>
        /// Get or Set config get first message error of each field. Default false.
        /// if false will get all error message of each field.
        /// </summary>
        public bool Config_GetFirstError { get; set; } = false;
        
        public ModelState_ResultBuilder(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }

        public ModelState_ResultBuilder(ModelStateDictionary modelState, bool getFirstError)
        {
            ModelState = modelState;
            Config_GetFirstError = getFirstError;
        }

        public static ModelState_ResultBuilder Init(ModelStateDictionary modelState, bool getFirstError = false)
        {
            return new ModelState_ResultBuilder(modelState, getFirstError);
        }

        public ModelState_ResultBuilder AddModelError(string field, string message)
        {
            ModelState.AddModelError(field, message);
            return this;
        }

        public Validate_Errors Build()
        {
            Validate_Errors errors = new Validate_Errors();            
            errors.Fields = new List<Field_Error>();

            errors.Fields = ModelState.Select(x =>
            {
                Field_Error err = new Field_Error();
                err.Key = x.Key;
                err.Val_Result = x.Value.RawValue;

                if (Config_GetFirstError == true)
                {
                    err.Message = x.Value.Errors.Count > 0 ? x.Value.Errors.First().ErrorMessage : "";
                }
                else
                {
                    err.Message = string.Join(" ; ", x.Value.Errors.Select(s => s.ErrorMessage));
                }
                return err;

            }).ToList();

            return errors;
        }

        /// <summary>
        /// Get List of string error in this format, "property name : errorMsg1, errorMsg2, ..."
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private List<string> GetAllErrors(ModelStateDictionary modelState, bool getFirstError)
        {
            List<string> listErrorMsgs;
            
            if (getFirstError)
            {
                // Format 
                // "property name : errorMsg1"

                listErrorMsgs = modelState
                    .Select(x => string.Format("{0} : {1}", x.Key, x.Value.Errors.First())).ToList();
            }
            else
            {
                // Format :
                // "property name : errorMsg1 ; errorMsg2, ..."

                listErrorMsgs = modelState
                    .Select(x => string.Format("{0} : {1}", x.Key, string.Join(" ; ", x.Value.Errors.Select(s => s.ErrorMessage)))).ToList();
            }

            return listErrorMsgs;
        }
    }
}
