using MySqlConnector;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Conex�o com MySQL
builder.Services.AddTransient<MySqlConnection>(_ =>
    new MySqlConnection(builder.Configuration.GetConnectionString("MySqlConnection")));

// Conex�o com Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
    $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}"));

// Adiciona os controllers
builder.Services.AddControllers();

// Configura Swagger para documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura��o do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
