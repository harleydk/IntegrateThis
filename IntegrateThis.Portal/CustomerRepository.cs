using IntegrateThis.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrateThis.DAL;

public class CustomerRepository(ShopContext shopContext) : ICustomerRepository
{
    public async Task<Customer> GetCustomerAsync(Guid CustomerId)
    {
        Customer customer = await shopContext.Customers.FirstOrDefaultAsync(customer => customer.Id == CustomerId)
            ?? throw new ArgumentException("No such customer");
        return customer;
    }

    public async Task InsertCustomerAsync(Customer customer)
    {
        _ = await shopContext.Customers.AddAsync(customer);
        _ = await shopContext.SaveChangesAsync();
    }
}