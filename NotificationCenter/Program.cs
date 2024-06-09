using Newtonsoft.Json.Converters;
using NotificationCenter.Jobs;
using ZodiacBot.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers().AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
});

services.AddEndpointsApiExplorer();
services.AddDbContext();
services.AddHostedService<NotificationWorker>();

var app = builder.Build();

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();

app.Run();
