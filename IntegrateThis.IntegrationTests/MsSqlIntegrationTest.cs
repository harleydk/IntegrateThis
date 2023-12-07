using IntegrateThis.DAL;
using IntegrateThis.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace IntegrateThis.IntegrationTests;

[TestClass]
[TestCategory("IntegrationTest")]
public class SqlServerTests
{
    private ShopContext? _sqlServerShopContext;

    [TestInitialize]
    public async Task Initialize()
    {
        MsSqlContainer msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433) // You don't have to specify this; a random port will be used if not.
            .Build();
        await msSqlContainer.StartAsync();

        string connectionString = msSqlContainer.GetConnectionString();
        System.Diagnostics.Debug.WriteLine(connectionString);
        DbContextOptionsBuilder<ShopContext> dbContextOptionsBuilder = new();
        _ = dbContextOptionsBuilder.UseSqlServer(connectionString);

        _sqlServerShopContext = new(dbContextOptionsBuilder.Options);
        _ = _sqlServerShopContext.Database.EnsureCreated();
    }

    [TestMethod]
    public async Task TestInsertCustomer()
    {
        // arrange
        CustomerRepository customerRepository = new(_sqlServerShopContext!);
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