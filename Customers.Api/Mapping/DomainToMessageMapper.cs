using Customers.Api.Domain;
using Customers.Consumer;

namespace Customers.Api.Mapping
{
    public static class DomainToMessageMapper
    {
        public static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
        {
            return new CustomerCreated
            {
                Id = customer.Id,
                GitHubUsername = customer.GitHubUsername,
                FullName = customer.FullName,
                Email = customer.Email,
                DateOfBirth = customer.DateOfBirth,
            };
        }
        public static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
        {
            return new CustomerUpdated
            {
                Id = customer.Id,
                Email = customer.Email,
                GitHubUsername = customer.GitHubUsername,
                DateOfBirth = customer.DateOfBirth,
                FullName = customer.FullName,
            };
        }
    }
}
