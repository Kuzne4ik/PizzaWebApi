using Hangfire.Dashboard;

namespace PizzaWebApi.Web
{
    /// <summary>
    /// Access to UI Hangfire Dashboard
    /// </summary>
    public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Auth rule
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            // No limit acess
            return true;
        }
    }
}
