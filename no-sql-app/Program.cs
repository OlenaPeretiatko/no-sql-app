using NoSQLApp.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<DbService>(sp =>
{
    string connectionString = configuration["CosmosDB:ConnectionString"];
    string databaseName = configuration["CosmosDB:DatabaseName"];
    string containerName = configuration["CosmosDB:ContainerName"];

    return new DbService(connectionString, databaseName, containerName);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
