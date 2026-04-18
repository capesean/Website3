using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website3.Web.Code;
using Website3.Web.Models;

namespace Website3.Web.Controllers
{
    [Route("api/[Controller]")]
    public class SetupController(IDbContextFactory<ApplicationDbContext> dbFactory, UserManager<User> _um, RoleManager<Role> _rm, AppSettings _appSettings)
        : BaseApiController(dbFactory, _um, _appSettings)
    {
        private readonly RoleManager<Role> roleManager = _rm;

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> CheckSetup()
        {
            return Ok(new { runSetup = !(await db.Settings.SingleAsync()).SetupCompleted });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> RunSetup(SetupDTO setupDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await db.Users.AnyAsync()) throw new HandledException("The database already has a user account.");

            if (dbSettings.SetupCompleted) throw new HandledException("The setup process has already completed.");

            var user = new User();
            user.FirstName = setupDTO.FirstName;
            user.LastName = setupDTO.LastName;
            user.Email = setupDTO.Email;
            user.UserName = setupDTO.Email;
            user.Disabled = false;

            var saveResult = await userManager.CreateAsync(user, setupDTO.Password);

            if (!saveResult.Succeeded)
                return GetErrorResult(saveResult);

            var appRoles = await roleManager.Roles.ToListAsync();

            foreach (var roleName in appRoles)
                await userManager.AddToRoleAsync(user, roleName.Name);

            dbSettings.SetupCompleted = true;
            db.Entry(dbSettings).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok();
        }


    }
}