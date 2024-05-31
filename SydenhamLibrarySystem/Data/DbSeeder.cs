using Microsoft.AspNetCore.Identity;
using SydenhamLibrarySystem.Constants;
using System;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<ApplicationUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();
            //adding some roles to db
            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.Guest.ToString()));

            // create admin user

            var admin = new ApplicationUser
            {
                FirstName = "Super",
                LastName = "Admin",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin,Roles.Admin.ToString());
            }

        }
    }
}
