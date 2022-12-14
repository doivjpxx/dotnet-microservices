using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add application layer service
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// add migration
app.MigrateDatabase<OrderContext>((context, provider) =>
{
    var logger = provider.GetService<ILogger<OrderContextSeed>>();

    OrderContextSeed.SeedAsync(context, logger).Wait();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();