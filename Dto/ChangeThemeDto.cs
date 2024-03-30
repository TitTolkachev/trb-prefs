using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Dto;

public class ChangeThemeDto
{
    [Required] public string Token { get; set; } = null!;
    [Required] public bool ThemeDark { get; set; }
}