using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2
{
    public class TestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        { 
            base.OnActionExecuting(context);
        }
    }
}
