using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Entities;

public class Theme
{
    public Guid Id { get; set; }
    [Required] public string UserId { get; set; } = null!;
    [Required] public bool IsThemeDark { get; set; }
}