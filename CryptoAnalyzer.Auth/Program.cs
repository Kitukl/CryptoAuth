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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionsHandler<,>));
builder.Services.AddScoped(typeof(IRepository<IdentityUser>), typeof(UserRepository));
builder.Services.AddScoped(typeof(IRepository<RefreshToken>), typeof(RefreshTokenRepository));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);

    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(RegisterCommandHandler).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddJwtAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JWTOptions>>());

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddScoped<JWTProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();
app.Run();