using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data;

public abstract class BaseRepositoryTests : IAsyncLifetime
{
    internal static readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithUsername("Nutrition")
        .WithPassword("postgres")
        .WithDatabase("postgres")
        .WithPortBinding(5432, true)
        .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
        .Build();

    internal static bool _containerStarted;
    internal static NutritionWriteDbContext _writeDbContext = null!;
    internal static NutritionReadDbContext _readDbContext = null!;
    private static readonly object _lock = new object();

    private bool _disposed;

    public virtual async Task InitializeAsync()
    {
        // Ensure container and context are initialized only once
        if (!_containerStarted)
        {
            bool shouldInitialize = false;
            lock (_lock)
            {
                if (!_containerStarted)
                {
                    shouldInitialize = true;
                }
            }

            if (shouldInitialize)
            {
                await _dbContainer.StartAsync();

                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:Database"] = _dbContainer.GetConnectionString(),
                        }
                    )
                    .Build();

                var writeOptions = new DbContextOptionsBuilder<NutritionWriteDbContext>()
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .Options;

                var readOptions = new DbContextOptionsBuilder<NutritionReadDbContext>()
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .Options;

                _writeDbContext = new NutritionWriteDbContext(writeOptions, configuration);
                _readDbContext = new NutritionReadDbContext(readOptions, configuration);

                await _writeDbContext.Database.ExecuteSqlRawAsync(
                    "CREATE EXTENSION IF NOT EXISTS pgcrypto;"
                );
                await _writeDbContext.Database.MigrateAsync();

                lock (_lock)
                {
                    _containerStarted = true;
                }
            }
        }
    }

    protected static async Task CleanupDbSet<T>(DbSet<T> dbSet)
        where T : class
    {
        await dbSet.ExecuteDeleteAsync();
    }

    protected static async Task CleanupAllDbSets()
    {
        if (_writeDbContext == null)
            return;

        var dbSetProperties = _writeDbContext
            .GetType()
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
            );

        foreach (var property in dbSetProperties)
        {
            var dbSet = property.GetValue(_writeDbContext);

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

    public async Task DisposeAsync()
    {
        if (!_disposed)
        {
            // Clean up all data from the database
            if (_writeDbContext != null)
            {
                await CleanupAllDbSets();
            }
            _disposed = true;
        }
    }
}

// [CollectionDefinition("NutritionRepositoryTests")]
// public class NutritionRepositoryTestsCollection : ICollectionFixture<DummyClass>
// {
//     // This class has no code, and is never created. Its purpose is to be the place to apply
//     // [CollectionDefinition] and all the ICollectionFixture<> interfaces.
// }

// public class DummyClass : IAsyncLifetime
// {
//     public Task InitializeAsync()
//     {
//         return Task.CompletedTask;
//     }

//     public async Task DisposeAsync()
//     {
//         if (BaseRepositoryTests._containerStarted)
//         {
//             if (BaseRepositoryTests._writeDbContext != null)
//             {
//                 await BaseRepositoryTests._writeDbContext.DisposeAsync();
//             }
//             if (BaseRepositoryTests._readDbContext != null)
//             {
//                 await BaseRepositoryTests._readDbContext.DisposeAsync();
//             }
//             await BaseRepositoryTests._dbContainer.DisposeAsync();
//         }
//     }
// }
