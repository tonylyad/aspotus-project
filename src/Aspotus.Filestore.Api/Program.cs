using Aspotus.Filestore.Api.Infrastructure;
using Aspotus.Filestore.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<S3AccountSettings>(
    builder.Configuration.GetSection(S3AccountSettings.SectionName));
builder.Services.AddSingleton<S3Account>();
builder.Services.AddHostedService<S3AccountService>();
builder.Services.AddScoped<IFileService, S3StoreFileService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
