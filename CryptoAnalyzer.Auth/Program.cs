using CryptoAnalyzer.Auth.Extensions;
using CryptoAuth.BLL.Commands;
using CryptoAuth.BLL.Commands.RegisterCommandHandler;
using CryptoAuth.BLL.Exctentions;
using CryptoAuth.BLL.Validations;
using CryptoAuth.DAL;
using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using dotenv.net;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionsHandler<,>));
builder.Services.AddScoped(typeof(IRepository<RefreshToken>), typeof(RefreshTokenRepository));
builder.Services.AddScoped(typeof(IRepository<ResetPasswordCode>), typeof(RessetPasswordCodeRepository));

builder.Services.Configure<FrontEndOptions>(builder.Configuration.GetSection("FrontEnd"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<ISendGridClient>(sg => new SendGridClient(builder.Configuration.GetValue<string>("EmailSettings:ApiKey")));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);

    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(RegisterCommandHandler).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddJwtAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JWTOptions>>());

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

var frontEndUrl = builder.Configuration.GetValue<string>("FrontEnd:Url") ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(frontEndUrl);
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

builder.Services.AddScoped<JWTProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); 

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();