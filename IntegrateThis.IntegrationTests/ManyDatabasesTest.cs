using IntegrateThis.DAL;
using IntegrateThis.DAL.Models;

namespace IntegrateThis.IntegrationTests;

[TestClass]
public class ManyDatabasesTest : IntegrationTestBase
{
    public static IEnumerable<object[]> GeneratedCustomerIds
    {
        get
        {
            const int numberOfCustomerIds = 50;
            for (int i = 0; i < numberOfCustomerIds; i++)
            {
                yield return new object[] { Guid.NewGuid() };
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(GeneratedCustomerIds))]
    public async Task Test(Guid customerId)
    {
        // arrange
        System.Diagnostics.Debug.WriteLine(customerId.ToString());
        CustomerRepository customerRepository = new(_sqlServerShopContext!);
        Customer customer = new()
        {
            Id = customerId,
            Name = customerId.ToString()[..10]
        };

        // act
        await customerRepository.InsertCustomerAsync(customer);

        // assert
        Customer insertedCustomer = await customerRepository.GetCustomerAsync(customer.Id);
        Assert.IsNotNull(insertedCustomer);
        Assert.AreEqual(customer.Id, insertedCustomer.Id);
    }
}