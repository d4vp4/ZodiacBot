using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class User
{
    public int Id { get; set; }
    public long UserId { get; set; }
    [MaxLength(200)] public string? NextAction { get; set; }
}
