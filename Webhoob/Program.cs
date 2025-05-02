
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using Webhoob.DbContext;
using Webhoob.Service;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.



builder.Services.AddDbContext<ObjectDbContext>(options =>
    options.UseSqlite("Data Source=mydatabase.db"));

builder.Services.AddScoped<WebhookService>();
builder.Services.AddHttpClient();

//Swagger

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Webhook system API",
        Version = "v1",
        Description = "API documentation for the Webhook system"
    });

    // Configure Swagger to Use XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webhook system");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webhook system");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
});

app.UseAuthorization();

app.MapControllers();

// Apply database migrations
// This will create the database if it doesn't exist and apply any pending migrations.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ObjectDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
