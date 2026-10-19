using System.Net;

namespace Students_Info_Api.Service
{  
        public enum ErrorType
        {
            None = HttpStatusCode.OK,
            ValidationError = HttpStatusCode.BadRequest,
            NotFound = HttpStatusCode.NotFound,
            Conflict = HttpStatusCode.Conflict,
            Unauthorized = HttpStatusCode.Unauthorized,
            InternalError = HttpStatusCode.InternalServerError,

        }          
}
