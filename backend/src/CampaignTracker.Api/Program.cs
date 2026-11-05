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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
