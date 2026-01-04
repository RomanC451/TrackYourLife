using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data;

/// <summary>
/// Fixture that manages the PostgreSQL container lifecycle.
/// Disposed only once after ALL tests in the collection complete.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer DbContainer { get; } =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithUsername("Nutrition")
            .WithPassword("postgres")
            .WithDatabase("postgres")
            .WithPortBinding(5432, true)
            .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
            .Build();

    public NutritionWriteDbContext WriteDbContext { get; private set; } = null!;
    public NutritionReadDbContext ReadDbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = DbContainer.GetConnectionString(),
                }
            )
            .Build();

        var writeOptions = new DbContextOptionsBuilder<NutritionWriteDbContext>()
            .UseNpgsql(DbContainer.GetConnectionString())
            .Options;

        var readOptions = new DbContextOptionsBuilder<NutritionReadDbContext>()
            .UseNpgsql(DbContainer.GetConnectionString())
            .Options;

        WriteDbContext = new NutritionWriteDbContext(writeOptions, configuration);
        ReadDbContext = new NutritionReadDbContext(readOptions, configuration);
        await WriteDbContext.Database.ExecuteSqlRawAsync(
            "CREATE EXTENSION IF NOT EXISTS pgcrypto;"
        );

        await WriteDbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (WriteDbContext != null)
        {
            await WriteDbContext.DisposeAsync();
        }
        if (ReadDbContext != null)
        {
            await ReadDbContext.DisposeAsync();
        }
        await DbContainer.DisposeAsync();
    }
}

/// <summary>
/// Collection definition - groups tests that share the same DatabaseFixture.
/// </summary>
[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

/// <summary>
/// Base class for repository tests. Uses collection fixture for shared container.
/// </summary>
[Collection(nameof(DatabaseCollection))]
public abstract class BaseRepositoryTests : IAsyncLifetime
{
    protected readonly DatabaseFixture _fixture;
    protected NutritionWriteDbContext WriteDbContext => _fixture.WriteDbContext;
    protected NutritionReadDbContext ReadDbContext => _fixture.ReadDbContext;

    protected BaseRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    protected static async Task CleanupDbSet<T>(DbSet<T> dbSet)
        where T : class
    {
        await dbSet.ExecuteDeleteAsync();
    }

    protected async Task CleanupAllDbSets()
    {
        if (WriteDbContext == null)
            return;

        var dbSetProperties = WriteDbContext
            .GetType()
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
            );

        foreach (var property in dbSetProperties)
        {
            var dbSet = property.GetValue(WriteDbContext);

            if (dbSet != null)
            {
                var method = typeof(BaseRepositoryTests)
                    .GetMethod(nameof(CleanupDbSet), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(property.PropertyType.GetGenericArguments()[0]);

                if (method != null)
                {
                    await (Task)method.Invoke(null, new[] { dbSet })!;
                }
            }
        }
    }

    public virtual async Task DisposeAsync()
    {
        // Clean up test data after each test (but don't dispose the container)
        await CleanupAllDbSets();
    }
}
