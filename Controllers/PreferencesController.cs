using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using trb_prefs.Common;
using trb_prefs.Dto;
using trb_prefs.Entities;

namespace trb_prefs.Controllers;

[ApiController]
[Route("api/preferences")]
public class PreferencesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PreferencesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("theme")]
    [SwaggerOperation(Summary = "Get user app theme")]
    public async Task<IActionResult> GetTheme([FromQuery] GetThemeQuery body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        if (userId == null)
            return Unauthorized();
        
        var theme = await _context.Themes.FirstOrDefaultAsync(x => x.UserId == userId);

        if (theme == null) return Ok();
        return Ok(new ThemeDto { ThemeDark = theme.IsThemeDark }) ;
    }

    [HttpGet]
    [Route("hidden-accounts")]
    [SwaggerOperation(Summary = "Get user hidden accounts")]
    public async Task<IActionResult> GetHiddenAccounts([FromQuery] GetHiddenAccountsQuery body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        if (userId == null)
            return Unauthorized();
        
        var accounts = await _context.Accounts.Where(x => x.UserId == userId).ToListAsync();

        return Ok(accounts.ConvertAll(x=>x.AccountId));
    }
    
    [HttpPut]
    [Route("theme")]
    [SwaggerOperation(Summary = "Change user app theme")]
    public async Task<IActionResult> ChangeTheme([FromBody] ChangeThemeDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        if (userId == null)
            return Unauthorized();

        var theme = await _context.Themes.FirstOrDefaultAsync(x => x.UserId == userId);
        if (theme == null) await _context.Themes.AddAsync( new Theme
        {
            Id = Guid.NewGuid(),
            UserId = userId, 
            IsThemeDark = body.ThemeDark
        });
        else theme.IsThemeDark = body.ThemeDark;

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPost]
    [Route("hidden-account")]
    [SwaggerOperation(Summary = "Hide user account")]
    public async Task<IActionResult> HideAccount([FromBody] AccountDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        if (userId == null)
            return Unauthorized();

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && x.AccountId == body.AccountId);
        if (account != null)
            _context.Accounts.Remove(account);
        else
        {
            var newHidedAccount = new HiddenAccount
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountId = body.AccountId
            };
            await _context.Accounts.AddAsync(newHidedAccount);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPost]
    [Route("show-account")]
    [SwaggerOperation(Summary = "Show user account")]
    public async Task<IActionResult> ShowAccount([FromBody] AccountDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        if (userId == null)
            return Unauthorized();

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && x.AccountId == body.AccountId);
        if (account != null)
            _context.Accounts.Remove(account);

        await _context.SaveChangesAsync();
        return Ok();
    }

    private static async Task<string?> GetUserIdFromToken(string token)
    {
        try
        {
            return await FirebaseUtil.GetUserIdFromToken(token);
        }
        catch (Exception)
        {
            return null;
        }
    }
}