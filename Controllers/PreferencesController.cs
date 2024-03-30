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
    public async Task<ThemeDto> GetTheme([FromQuery] GetThemeQuery body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        
        var theme = await _context.Themes.FirstOrDefaultAsync(x => x.UserId == userId);

        if (theme != null) return new ThemeDto { IsThemeDark = theme.IsThemeDark };
        var ex = new Exception();
        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(), "Not Found");
        throw ex;
    }

    [HttpGet]
    [Route("hidden-accounts")]
    [SwaggerOperation(Summary = "Get user hidden accounts")]
    public async Task<List<string>> GetHiddenAccounts([FromQuery] GetHiddenAccountsQuery body)
    {
        var userId = await GetUserIdFromToken(body.Token);
        
        var accounts = await _context.Accounts.Where(x => x.UserId == userId).ToListAsync();

        return accounts.ConvertAll(x=>x.AccountId);
    }
    
    [HttpPut]
    [Route("theme")]
    [SwaggerOperation(Summary = "Change user app theme")]
    public async Task ChangeTheme([FromBody] ChangeThemeDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);

        var theme = await _context.Themes.FirstOrDefaultAsync(x => x.UserId == userId);
        if (theme == null) await _context.Themes.AddAsync( new Theme
        {
            Id = Guid.NewGuid(),
            UserId = userId, 
            IsThemeDark = body.IsThemeDark
        });
        else theme.IsThemeDark = body.IsThemeDark;

        await _context.SaveChangesAsync();
    }
    
    [HttpPost]
    [Route("hidden-account")]
    [SwaggerOperation(Summary = "Hide user account")]
    public async Task HideAccount([FromBody] AccountDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);

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
    }
    
    [HttpPost]
    [Route("show-account")]
    [SwaggerOperation(Summary = "Show user account")]
    public async Task ShowAccount([FromBody] AccountDto body)
    {
        var userId = await GetUserIdFromToken(body.Token);

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId && x.AccountId == body.AccountId);
        if (account != null)
            _context.Accounts.Remove(account);

        await _context.SaveChangesAsync();
    }

    private static async Task<string> GetUserIdFromToken(string token)
    {
        try
        {
            return await FirebaseUtil.GetUserIdFromToken(token);
        }
        catch (Exception e)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status403Forbidden.ToString(), e.Message);
            throw ex;
        }
    }
}