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

            List<ApplicationUser> listOfUsers = null;

            if (!userManager.Users.Any())
            {
                listOfUsers = new List<ApplicationUser>
                {
                    new ApplicationUser{ UserName="randomuser1@sample.com",
                        Email = "randomuser1@sample.com",
                        LastName="RandomUser",
                        FirstName="James" ,
                        Gender="Male",
                        PhoneNumber = "09876543212",
                        DOB="1/1/1999",
                        PlaceOfBirth= "Minna",
                        State = "Jos",
                        LGA = "Rururu",
                        HomeAddress ="10, wayside",
                        IsVerified = true
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
                        HomeAddress ="10, wayside",
                        IsVerified = true
                    }
                };
            }


            if (!ctx.Request.Any())
            {
                foreach (var user in listOfUsers)
                {
                    var requestId = Guid.NewGuid().ToString();
                    ctx.Request.Add(new Request
                    {
                        RequestId = requestId,
                        PhoneNumber = user.PhoneNumber,
                        IsVerified = true,
                        RequestAttempt = 1,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                    int addedReq = ctx.SaveChanges();
                    
                    if(addedReq > 0)
                    {
                        var result = await userManager.CreateAsync(user, "1234");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, "Admin");
                            var adminId = Guid.NewGuid().ToString();
                            ctx.Admins.Add(new Admin { AdminId = adminId, ApplicationUserId = user.Id });
                            ctx.SaveChanges();
                        }
                    }
                }

            }

            string[] BuildingTypesArr = {"Duplex","Bungalow", "Block of Flats",
            "Detached House","Semi-Detached House" , "Storey Building",
            "Terraced House", "Mud House", "Wooden House" };
            var adminIdFk = ctx.Admins.FirstOrDefault().AdminId;
            foreach (var type in BuildingTypesArr)
            {
                var buildingTypeId = Guid.NewGuid().ToString();
                ctx.BuildingTypes.Add(new BuildingType {TypeId = buildingTypeId,  TypeName = type, AdminId = adminIdFk, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
                ctx.SaveChanges();
            }            
        }
    }
}
