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
                        PhoneNumber = "09876543211",
                        DOB="1/1/1999",
                        PlaceOfBirth= "Minna",
                        State = "Jos",
                        LGA = "Rururu",
                        HomeAddress ="10, wayside"
                    },
                    new ApplicationUser{ UserName="randomuser2@sample.com",
                        Email = "randomuser2@sample.com", 
                        LastName="RandomUser", FirstName="John",
                        Gender="Male",
                        PhoneNumber = "09876543211",
                        DOB="1/1/1999",
                        PlaceOfBirth= "Minna",
                        State = "Jos",
                        LGA = "Rururu",
                        HomeAddress ="10, wayside"
                    }
                };
                
                foreach (var user in listOfUsers)
                {
                    var result = await userManager.CreateAsync(user, "1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                        var adminId = Guid.NewGuid().ToString();
                        var requestId = Guid.NewGuid().ToString();
                        ctx.Admins.Add(new Admin { AdminId = adminId, ApplicationUserId = user.Id });
                        ctx.Request.Add(new Request 
                                { 
                                    RequestId = requestId,
                                    PhoneNumber = user.PhoneNumber, 
                                    IsVerified = true, 
                                    RequestAttempt = 1,
                                    CreatedAt = DateTime.Now, 
                                    UpdatedAt = DateTime.Now 
                                });
                        ctx.SaveChanges();
                    }
                }
            }
        }
    }
}