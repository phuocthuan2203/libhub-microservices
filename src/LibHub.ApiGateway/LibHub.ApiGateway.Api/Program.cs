using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = "http://userservice";
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "LibHubAudience",
            ValidateIssuer = true,
            ValidIssuer = "LibHub.UserService"
        };
    });

builder.Services.AddOcelot()
        .AddConsul();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
