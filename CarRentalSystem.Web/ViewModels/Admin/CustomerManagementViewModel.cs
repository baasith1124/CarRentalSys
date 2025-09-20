using CarRentalSystem.Application.Contracts.Customer;
using CarRentalSystem.Application.Features.Customers.Commands.CreateCustomer;
using CarRentalSystem.Application.Features.Customers.Commands.UpdateCustomer;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Generate Random Password")]
        public bool GenerateRandomPassword { get; set; } = true;

        [Display(Name = "Custom Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string? CustomPassword { get; set; }

        [Display(Name = "Send Login Credentials via Email")]
        public bool SendCredentialsEmail { get; set; } = true;
    }

    public class EditCustomerViewModel : UpdateCustomerCommand
    {
        // Additional properties for the view if needed
    }
}
