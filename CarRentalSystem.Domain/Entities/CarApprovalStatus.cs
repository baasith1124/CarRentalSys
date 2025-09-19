using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class CarApprovalStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;  // , Pending, Approved, Rejected
        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }

}
