using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace DOCVIDEO.ErrorLoggingContext
{
    public class ElmahErrorAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        public override void OnException(
             System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
               Elmah.ErrorSignal.FromCurrentContext().Raise(actionExecutedContext.Exception);
               // Elmah.ErrorSignal.FromCurrentContext().Raise(new CustomException(actionExecutedContext.Exception.ToString, actionExecutedContext.Exception.InnerException));
            base.OnException(actionExecutedContext);
        }
    }

    public class CustomException : Exception
    {
        public CustomException()
            : base() { }

        public CustomException(string message)
            : base(message) { }

        public CustomException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomException(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) {  }

    }
}
