using CampaignTracker.Api.Auth;
using CampaignTracker.Api.Data;
using CampaignTracker.Api.Models.Entities;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CampaignService>();
builder.Services.AddScoped<CharacterService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

await app.MigrateDatabaseAsync();

// Unhandled exceptions and empty error responses (e.g. 401 from the JWT handler)
// are returned as RFC 7807 ProblemDetails, same as validation errors.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
