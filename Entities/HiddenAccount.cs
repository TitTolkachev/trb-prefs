using System.ComponentModel.DataAnnotations;

namespace trb_prefs.Entities;

public class HiddenAccount
{
    public Guid Id { get; set; }
    [Required] public string UserId { get; set; } = null!;
    [Required] public string AccountId { get; set; } = null!;
}