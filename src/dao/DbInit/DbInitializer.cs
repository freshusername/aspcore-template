using api.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32.SafeHandles;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace dao.DbInit
{
    public class DbInitializer : IDisposable
    {
        private static User _seededUser;
        private bool _disposed = false;
        private SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public DbInitializer(IServiceProvider serviceProvider)
        {
            _seededUser = new User();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        }

        public async Task SeedUserData()
        {
            await SeedRoles();
            await SeedUsers();
        }

        private async Task SeedRoles()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new Role("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new Role("User"));
            }
        }
        private async Task SeedUsers()
        {
            var admin = new User
            {

                Email = "admin@admin.com",
                Name = "Admin",
                UserName = "admin@admin.com",
                PhoneNumber = "+380960000000",
                EmailConfirmed = true
            };

            var user = new User
            {
                Email = "user@gmail.com",
                Name = "User",
                UserName = "simple-user",
                PhoneNumber = "+380970000000",
                EmailConfirmed = true
            };

            if (await _userManager.FindByEmailAsync(admin.Email) == null)
            {
                IdentityResult result;
                result = await _userManager.CreateAsync(admin, "adminHashPasswd_622");

                if (result.Succeeded)
                    _userManager.AddToRoleAsync(admin, "Admin").Wait();
            }

            if (_userManager.FindByEmailAsync(user.Email).Result == null)
            {
                IdentityResult result;
                result = await _userManager.CreateAsync(user, "userHashPasswd_623");

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "User").Wait();
                }
            }
            _seededUser = await _userManager.FindByEmailAsync(user.Email);
        }

        public static void SeedOtherTables(ApplicationDbContext context)
        {
            if (!context.Records.Any())
            {
                context.Records.AddRange(
                new Record
                {
                    AvgSpeed = 8.5f,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsPublic = true,
                    AmountKmRun = 25.7f,
                    UserId = _seededUser.Id
                });
                context.SaveChanges();

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _handle.Dispose();
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
