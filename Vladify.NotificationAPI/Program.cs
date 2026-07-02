using Scalar.AspNetCore;
using Vladify.NotificationAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddAppServices(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapGraphQL();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
