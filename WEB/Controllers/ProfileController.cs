using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website3.Models;

namespace Website3.Controllers
{
    [Route("api/[Controller]")]
    public class ProfileController(IDbContextFactory<ApplicationDbContext> dbFactory, UserManager<User> _um, AppSettings _appSettings)
        : BaseApiController(dbFactory, _um, _appSettings)
    {
        [HttpGet]
        public async Task<IActionResult> Profile()
        {

            var profile = new ProfileModel
            {
                Email = CurrentUser.Email,
                FirstName = CurrentUser.FirstName,
                LastName = CurrentUser.LastName,
                FullName = CurrentUser.FullName,
                UserId = CurrentUser.Id,
                UserName = CurrentUser.UserName
            };

            return Ok(profile);
        }

    }
}