using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website3.Web.Models;

namespace Website3.Web.Controllers
{
    [Route("api/[Controller]"), AuthorizeRoles(Roles.Administrator)]
    public class ErrorsController(IDbContextFactory<ApplicationDbContext> dbFactory, UserManager<User> um, AppSettings appSettings) 
        : BaseApiController(dbFactory, um, appSettings)
    {
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]SearchOptions pagingOptions)
        {
            pagingOptions ??= new SearchOptions();

            IQueryable<Error> results = db.Errors;

            results = results.OrderByDescending(o => o.DateUtc);

            return Ok((await GetPaginatedResponse(results, pagingOptions)).Select(o => new { id = o.Id, message = o.Message, dateUtc = o.DateUtc }));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var error = await db.Errors
                .Include(o => o.Exception)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (error == null)
                return NotFound();

            error.Exception?.InnerException = await GetInnerExceptionAsync(error.Exception);

            return Ok(error);
        }

        private async Task<ErrorException> GetInnerExceptionAsync(ErrorException exception)
        {
            ErrorException innerException = null;
            if (exception.InnerExceptionId != null)
            {
                innerException = await db.Exceptions.FirstAsync(o => o.Id == exception.InnerExceptionId);
                innerException.InnerException = await GetInnerExceptionAsync(innerException);
            }
            return innerException;
        }
    }
}
