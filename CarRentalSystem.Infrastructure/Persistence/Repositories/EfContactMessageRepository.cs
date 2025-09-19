using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfContactMessageRepository : IContactMessageRepository
    {
        private readonly ApplicationDbContext _context;
        public EfContactMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveMessageAsync(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
        }
    }

}
