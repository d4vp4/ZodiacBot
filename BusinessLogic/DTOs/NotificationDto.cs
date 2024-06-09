using DataAccess.Enums;
using DataAccess.Models;

namespace BusinessLogic.DTOs;

public record NotificationDto(long UserId, Period Period, string Zodiac);
