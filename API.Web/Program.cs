using System.Globalization;
using API.Core.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<CreateNoteRequestValidator>();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("NoteShareConnection")));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<INoteService,NoteService>();
builder.Services.AddScoped<ISharedService,SharedService>();
builder.Services.AddScoped<IModeratorService,ModeratorService>();

builder.Services.AddSingleton<INotePopularityService,NotePopularityService>();

builder.Services.AddHostedService<NoteHostedCaching>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));
builder.Services.AddScoped<IJwtProvider,JwtProvider>();

builder.Services.AddRedisCache();
builder.Services.AddRedisDb();

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();
builder.Host.UseSerilog();

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseStaticFiles();

app.UseSerilogRequestLogging(options =>{
     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("LogType", "http");
        };
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope()) 
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    await DatabaseInitializer.Initialize(scope.ServiceProvider);
}

app.Run();
