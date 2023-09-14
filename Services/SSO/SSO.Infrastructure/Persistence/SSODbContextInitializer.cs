using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Extensions;
using SSO.Domain.Entities;
using SSO.Domain.Enums;
using System;
using System.Linq;

namespace SSO.Infrastructure.Persistence
{
    public class SSODbContextInitializer
    {
        private readonly ILogger<SSODbContextInitializer> _logger;
        private readonly SSODbContext _context;
        private readonly IEncryptionService _encryptionService;

        public SSODbContextInitializer(ILogger<SSODbContextInitializer> logger, SSODbContext context, IEncryptionService encryptionService)
        {
            _logger = logger;
            _context = context;
            _encryptionService = encryptionService;
        }

        public void Initialize()
        {
            try
            {
                _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public void Seed()
        {
            try
            {
                TrySeed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public void TrySeed()
        {
            if (_context.Users.Any() == false)
            {
                var roles = SystemRolesInitializer.Roles;
                _context.Roles.AddRange(roles);

                var group = new Group { Caption = "Administrator" };
                group.Roles.AddRange(roles);
                _context.Groups.Add(group);

                var admin = new User { UserName = "admin", FirstName = "مدیر", LastName = "سیستم", Password = _encryptionService.HashPassword("123"), Mobile = "+989121234567", Gender = GenderType.Female, CreatedDate = DateTime.Now };
                admin.Groups.Add(group);
                _context.Users.Add(admin);

                _context.SaveChanges();
            }
            else
            {
                var roles = SystemRolesInitializer.Roles;
                var existRoles = _context.Roles.Where(r => roles.Select(rr => rr.Title).Contains(r.Title)).ToList();
                if (existRoles.Count != roles.Count)
                {
                    var newRoles = roles.Where(r => existRoles.Select(e => e.Title).Contains(r.Title) == false).ToList();
                    _context.Roles.AddRange(newRoles);

                    var group = _context.Groups.FirstOrDefault(g => g.Caption == "Administrator");
                    group.Roles.AddRange(newRoles);

                    _context.SaveChanges();
                }
            }
        }
    }
}
