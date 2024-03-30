using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Dto;

public class GetHiddenAccountsQuery
{
    [Required] public string Token { get; set; } = null!;
}