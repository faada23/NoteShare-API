using System.Globalization;
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

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient(typeof(IValidatorInterceptor), typeof(LoggingValidatorInterceptor));

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("NoteShareConnection")));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<INoteService,NoteService>();
builder.Services.AddScoped<ISharedService,SharedService>();
builder.Services.AddScoped<IModeratorService,ModeratorService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));
builder.Services.AddScoped<IJwtProvider,JwtProvider>();

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();
builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseStaticFiles();

app.UseRouting();

app.UseValidationLogging();

app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.Run();
