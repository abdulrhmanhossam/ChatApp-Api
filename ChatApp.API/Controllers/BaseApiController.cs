using ChatApp.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
