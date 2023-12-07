using IntegrateThis.DAL;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace IntegrateThis.IntegrationTests;

/// <summary>
/// Base-class for integration tests. These are run in containers, provided by the use of 'Testcontainers'
/// NuGet-package. Ensure the availability of Docker, or corresponding container run-time provider, in order to
/// execute them.
/// </summary>
[TestClass]
[TestCategory("IntegrationTest")]
public abstract class IntegrationTestBase
{
    protected static readonly MsSqlContainer _msSqlContainer; // Will be shared among all the tests

    protected ShopContext? _sqlServerShopContext; // Will be individually instantiated with every single test

    /// <summary>
    /// This static constructor initializes a container-based SQL server. We use this same one
    /// throughout all these tests - hence the static construction - so we won't have to instantiate
    /// numerous containers; this would be a waste of time and resources. Instead we create
    /// test-specific databases, to ensure that each test has an exclusive db-scope.
    /// </summary>
    static IntegrationTestBase()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433) // You don't have to specify this; a random port will be used if not.
            .Build();
        _msSqlContainer.StartAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// / By default, a dbContext will be created off the 'master' database. We'll replace the database name with a generic one,
    /// so as to run a number of integration-tests in parallel, against the same server but a different database for every test.
    /// </summary>
    [TestInitialize]
    public void TestSetup()
    {
        string connectionString = _msSqlContainer.GetConnectionString();
        string databaseName = Guid.NewGuid().ToString();
        string individualTestConnectionString = _msSqlContainer.GetConnectionString().Replace("master", databaseName);

        DbContextOptionsBuilder<ShopContext> optionsBuilder = new();
        _ = optionsBuilder.UseSqlServer(individualTestConnectionString);

        _sqlServerShopContext = new ShopContext(optionsBuilder.Options);
        _ = _sqlServerShopContext.Database.EnsureCreated(); // create a new database for each individual test
        System.Diagnostics.Debug.WriteLine("Created database {0}", individualTestConnectionString);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        System.Diagnostics.Debug.WriteLine("deleting database {0}", _sqlServerShopContext?.Database.GetConnectionString());
        _ = (_sqlServerShopContext?.Database.EnsureDeleted()); // delete the individual database after use
    }
}
