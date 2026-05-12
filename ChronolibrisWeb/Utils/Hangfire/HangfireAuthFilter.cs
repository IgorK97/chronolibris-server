//using Hangfire.Dashboard;

//namespace ChronolibrisWeb.Middleware.Hangfire
//{
//    public class HangfireAuthFilter : IDashboardAuthorizationFilter
//    {
//        public bool Authorize(DashboardContext context)
//        {
//            var httpContext = context.GetHttpContext();

//            //только для авторизованных
//            return httpContext.User.Identity?.IsAuthenticated ?? false;

//            //только для роли Admin
//            // return httpContext.User.IsInRole("Admin");

//            // return true;
//        }
//    }
//}
