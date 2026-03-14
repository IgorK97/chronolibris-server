using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace ChronolibrisPrototype.Hangfire
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Вариант 1: только для авторизованных
            return httpContext.User.Identity?.IsAuthenticated ?? false;

            // Вариант 2: только для роли Admin
            // return httpContext.User.IsInRole("Admin");

            // Вариант 3: только в Development (небезопасно для прода!)
            // return true;
        }
    }
}
