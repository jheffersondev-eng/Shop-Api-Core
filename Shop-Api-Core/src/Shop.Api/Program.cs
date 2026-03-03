using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shop.Api.Middleware;
using Shop.Api.Services;
using Shop.Application;
using Shop.Application.Common.Interfaces;
using Shop.Application.Common.Options;
using Shop.Application.Customers.Interfaces;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(typeof(ApplicationAssemblyMarker).Assembly);
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ShopDb")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICognitoUserService, CognitoUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.Configure<CognitoOptions>(
    builder.Configuration.GetSection("Authentication:Cognito"));

var cognitoAuthority = builder.Configuration["Authentication:Cognito:Authority"];
var cognitoAudience = builder.Configuration["Authentication:Cognito:Audience"];
var cognitoRegion = builder.Configuration["Authentication:Cognito:Region"];
var cognitoUserPoolId = builder.Configuration["Authentication:Cognito:UserPoolId"];

if (string.IsNullOrWhiteSpace(cognitoAuthority)
    || string.IsNullOrWhiteSpace(cognitoAudience)
    || string.IsNullOrWhiteSpace(cognitoRegion)
    || string.IsNullOrWhiteSpace(cognitoUserPoolId))
{
    throw new InvalidOperationException("Cognito settings are missing.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = cognitoAuthority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = cognitoAuthority,
            ValidateAudience = true,
            ValidAudience = cognitoAudience,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
