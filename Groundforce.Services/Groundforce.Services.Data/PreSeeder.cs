using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Groundforce.Services.Data
{
    public static class PreSeeder
    {
        public static async Task Seeder(AppDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            ctx.Database.EnsureCreated();
            if (!roleManager.Roles.Any())
            {
                var listOfRoles = new List<IdentityRole>
                {
                    new IdentityRole("Super Admin"),
                    new IdentityRole("Admin"),
                    new IdentityRole("Client"),
                    new IdentityRole("Agent")
                };
                foreach (var role in listOfRoles)
                {
                    await roleManager.CreateAsync(role);
                }
            }
            if (!userManager.Users.Any())
            {
                var listOfUsers = new List<ApplicationUser>
                {
                    new ApplicationUser{ UserName="randomuser1@sample.com",
                        Email = "randomuser1@sample.com",
                        LastName="RandomUser",
                        FirstName="James" ,
                        Gender="Male",
                        DOB="1/1/1999",
                        PlaceOfBirth= "Minna",
                        State = "Jos",
                        LGA = "Rururu",
                        HomeAddress ="10, wayside"
                    },
                    new ApplicationUser{ UserName="randomuser2@sample.com",
                        Email = "randomuser2@sample.com", LastName="RandomUser", FirstName="John",
                        Gender="Male",
                        DOB="1/1/1999",
                        PlaceOfBirth= "Minna",
                        State = "Jos",
                        LGA = "Rururu",
                        HomeAddress ="10, wayside"
                    }
                };
                int counter = 0;
                foreach (var user in listOfUsers)
                {
                    var result = await userManager.CreateAsync(user, "1234");
                    if (result.Succeeded)
                    {
                        if (counter == 0)
                        {
                            await userManager.AddToRoleAsync(user, "Super Admin");
                        }
                        else
                        {
                            await userManager.AddToRoleAsync(user, "Admin");
                            ctx.Admins.Add(new Admin { ApplicationUserId = user.Id });
                            ctx.SaveChanges();
                        }
                    }
                    counter++;
                }
            }
        }
    }
}