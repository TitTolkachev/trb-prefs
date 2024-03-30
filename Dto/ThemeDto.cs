using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Dto;

public class ThemeDto
{
    [Required] public bool IsThemeDark { get; set; }
}