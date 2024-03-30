using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Dto;

public class GetThemeQuery
{
    [Required] public string Token { get; set; } = null!;
}