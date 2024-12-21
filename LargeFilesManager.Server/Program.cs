using LargeFilesManager.BL.Extensions;
using LargeFilesManager.Core.Extensions;
using LargeFilesManager.Files.Extensions;
using LargeFilesManager.StringsGeneration.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCoreServices();
builder.Services.AddStringsGenerationServices();
builder.Services.AddFilesServices();
builder.Services.AddBLServices();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
