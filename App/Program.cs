using TrackYourLifeDotnet.Persistence;
using MediatR;
using Scrutor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TrackYourLifeDotnet.App.OptionsSetup;
using App.OptionsSetup;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddApplicationPart(TrackYourLifeDotnet.Presentation.AssemblyReference.Assembly);

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();

builder.Services.Scan(
    selector =>
        selector
            .FromAssemblies(
                TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly,
                TrackYourLifeDotnet.Persistence.AssemblyReference.Assembly
            )
            .AddClasses(false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(TrackYourLifeDotnet.Application.AssemblyReference.Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
