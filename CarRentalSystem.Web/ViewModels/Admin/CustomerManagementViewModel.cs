using CarRentalSystem.Application.Contracts.Customer;
using CarRentalSystem.Application.Features.Customers.Commands.CreateCustomer;
using CarRentalSystem.Application.Features.Customers.Commands.UpdateCustomer;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class CustomerManagementViewModel
    {
        public List<CustomerDto> Customers { get; set; } = new();
        public int TotalCustomers { get; set; }
        public int KycVerifiedCustomers { get; set; }
        public int PendingKycCustomers { get; set; }
        public string? FilterStatus { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class CreateCustomerViewModel : CreateCustomerCommand
    {
        // Additional properties for the view if needed
    }

    public class EditCustomerViewModel : UpdateCustomerCommand
    {
        // Additional properties for the view if needed
    }
}
