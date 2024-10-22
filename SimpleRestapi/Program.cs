using Microsoft.EntityFrameworkCore;
using Npgsql;
using SimpleRestapi;
using SimpleRestapi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITodoService, TodoService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));

if (connectionString != null)
{
    CreateDatabaseIfNotExists(connectionString);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Automatically db migration applying
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void CreateDatabaseIfNotExists(string connectionString)
{
    var builder = new NpgsqlConnectionStringBuilder(connectionString);
    var databaseName = builder.Database;
    builder.Database = "postgres";

    using var connection = new NpgsqlConnection(builder.ConnectionString);
    connection.Open();

    using var command = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection);
    var databaseExists = command.ExecuteScalar() != null;

    if (!databaseExists)
    {
        using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
        createCommand.ExecuteNonQuery();
    }
}
