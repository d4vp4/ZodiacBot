using DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using ZodiacBot.Extensions;
using ZodiacBot.Jobs;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers().AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext();
services.AddZodiacServices();
services.AddTelegramBot(builder.Configuration["TelegramBot:Token"]!);
services.AddHostedService<BotWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ZodiacDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
