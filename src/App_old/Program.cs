using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(
//         "CORSPolicy",
//         builder =>
//         {
//             builder.AllowAnyMethod().AllowAnyHeader().WithOrigins("https://localhost:44497");
//         }
//     );
// });

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices();
builder.Services.AddPresentationServices();

var app = builder.Build();

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
// app.UseCors(
//     builder =>
//         builder
//             .AllowAnyHeader()
//             .AllowAnyMethod()
//             .AllowCredentials()
//             .WithOrigins("https://localhost:44497")
// );

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// app.UseAuthentication();
// app.UseRouting();

// app.UseAuthorization();

// app.MapControllers();

// if (!app.Environment.IsDevelopment())
// {
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.Environment.WebRootFileProvider = new PhysicalFileProvider(
//     Path.Combine(
//         new DirectoryInfo(builder.Environment.ContentRootPath).Parent!.FullName,
//         @"Presentation/ClientApp/build"
//     )
// );

// app.UseStaticFiles();

// app.MapFallbackToFile("index.html");

// app.Run();
