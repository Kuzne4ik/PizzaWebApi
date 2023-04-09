using Microsoft.AspNetCore.Mvc;

namespace PizzaWebApi.Web.Api
{
    /// <summary>
    /// Base controller class
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
