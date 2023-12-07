using IntegrateThis.DAL;
using IntegrateThis.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrateThis.IntegrationTests;

[TestClass]
[TestCategory("IntegrationTestButNotReally")]
public class SQLiteTests
{
    private readonly ShopContext _sqliteShopContext;

    public SQLiteTests()
    {
        string sqliteInMemoryConnectionstring = "Data Source=:memory:";
        DbContextOptionsBuilder<ShopContext> dbContextOptionsBuilder = new();
        dbContextOptionsBuilder.UseSqlite(sqliteInMemoryConnectionstring);

        _sqliteShopContext = new(dbContextOptionsBuilder.Options);
    }

    [TestInitialize]
    public void SetupTest()
    {
        _sqliteShopContext.Database.GetDbConnection().Open(); // SQLite in-memory databases require an open connection for the duration of the test.
        _ = _sqliteShopContext.Database.EnsureCreated(); // Create in-memory tables for the db-sets in the context.
    }

    [TestCleanup]
    public void CleanupTest()
    {
        _sqliteShopContext.Database.GetDbConnection().Close();
        _sqliteShopContext.Database.GetDbConnection().Dispose();
    }

    [TestMethod]
    public async Task TestInsertCustomer()
    {
        // arrange
        CustomerRepository customerRepository = new(_sqliteShopContext);
        Customer customer = new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString()
        };

        // act
        await customerRepository.InsertCustomerAsync(customer);

        // assert
        Customer insertedCustomer = await customerRepository.GetCustomerAsync(customer.Id);
        Assert.IsNotNull(insertedCustomer);
        Assert.AreEqual(customer.Id, insertedCustomer.Id);
        Assert.AreEqual(customer.Name, insertedCustomer.Name);
    }
}