using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Website3.Web.Models;

namespace Website3.Web.Controllers
{
    [Route("api/[Controller]"), Authorize]
    public class AppController(IDbContextFactory<ApplicationDbContext> dbFactory, UserManager<User> um, AppSettings appSettings) 
        : BaseApiController(dbFactory, um, appSettings)
    {
        [HttpGet, Route("settings")]
        public IActionResult Get()
        {
            // return the settings for all logged in users
            return Ok(
                new
                {
                    dbSettings.SetupCompleted
                }
            );
        }

        // needs to be run on login page (anonymous)
        [HttpGet, Route("setupcheck"), AllowAnonymous]
        public IActionResult SetupCheck()
        {
            return Ok(
                new
                {
                    dbSettings.SetupCompleted
                }
            );
        }

    }
}
