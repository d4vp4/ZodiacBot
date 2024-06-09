using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Models;

public class Notification
{
    public int Id { get; set; }
    public Period Period { get; set; }
    public long UserId { get; set; }
    [MaxLength(30)] public string Zodiac { get; set; } = null!;
}
