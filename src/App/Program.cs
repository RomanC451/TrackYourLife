var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CORSPolicy",
        builder =>
        {
            builder.AllowAnyMethod().AllowAnyHeader().WithOrigins("https://192.168.1.8:44497");
        }
    );
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices();
builder.Services.AddPresentationServices();

var app = builder.Build();

app.UseCors(
    builder =>
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("https://192.168.1.8:44497")
);

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

app.Urls.Add("https://192.168.1.8:7072");

app.Run();
