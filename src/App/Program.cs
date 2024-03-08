var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CORSPolicy",
        builder =>
        {
            builder
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("http://192.168.1.8:5173");
            // .SetIsOriginAllowed(origin => new Uri(origin).Host == "192.168.1.8");
        }
    );
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices();
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();
app.UseRouting();

app.UseCors("CORSPolicy");

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

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}
