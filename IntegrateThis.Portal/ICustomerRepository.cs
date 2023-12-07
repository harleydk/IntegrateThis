using IntegrateThis.DAL.Models;

namespace IntegrateThis.DAL;

public interface ICustomerRepository
{
    Task<Customer> GetCustomerAsync(Guid customerId);

    Task InsertCustomerAsync(Customer customer);
}