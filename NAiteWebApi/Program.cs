using Microsoft.EntityFrameworkCore;
using NAiteEntities.Models;
using NAiteWebApi;
using NAiteWebApi.Attributes;
using NAiteWebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

NAiteSettings.Configuration = builder.Configuration;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

app.AddGlobalErrorHandler();
app.UseCurrentRequestContext();
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});

app.UseAuthorization();

app.MapControllers();

app.Run();
