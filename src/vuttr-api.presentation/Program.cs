using Microsoft.OpenApi.Models;
using vuttr_api.infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongoDbContext(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddMappingProfile();
builder.Services.ConfigureServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s => s.SwaggerDoc("v1", new OpenApiInfo { Title = "vuttr-api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "vuttr-api v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
