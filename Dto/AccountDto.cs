using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Dto;

public class AccountDto
{
    [Required] public string Token { get; set; } = null!;
    [Required] public string AccountId { get; set; } = null!;
}