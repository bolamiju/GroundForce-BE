using Groundforce.Services.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Groundforce.Services.Data
{
    public static class PreSeeder
    {
        public static async Task Seeder(AppDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {

            // pre-load data to roles table
            ctx.Database.EnsureCreated();
            if (!roleManager.Roles.Any())
            {
                var listOfRoles = new List<IdentityRole>
                {
                    new IdentityRole("admin"),
                    new IdentityRole("client"),
                    new IdentityRole("agent")
                };
                foreach (var role in listOfRoles)
                {
                    await roleManager.CreateAsync(role);
                }
            }


            // pre-load data of admin type to the users table
            List<ApplicationUser> listOfUsers = null;
            if (!userManager.Users.Any())
            {
                listOfUsers = new List<ApplicationUser>
                {
                    new ApplicationUser{ UserName="randomuser1@sample.com",
                        Email = "randomuser1@sample.com",
                        LastName="RandomUser",
                        FirstName="James" ,
                        Gender="m",
                        PhoneNumber = "+2349876543212",
                        DOB="1/1/1999",
                        IsVerified = true,
                        IsActive = true
                    },
                    new ApplicationUser{ UserName="randomuser2@sample.com",
                        Email = "randomuser2@sample.com",
                        LastName="RandomUser", 
                        FirstName="John",
                        Gender="m",
                        PhoneNumber = "+2349876543211",
                        DOB="1/1/1999",
                        IsVerified = true,
                        IsActive = true
                    }
                };

                // add user record to the request table
                if (!ctx.Request.Any())
                {
                    foreach (var user in listOfUsers)
                    {
                        var requestId = Guid.NewGuid().ToString();
                        ctx.Request.Add(new Request
                        {
                            RequestId = requestId,
                            PhoneNumber = user.PhoneNumber,
                            Status = "confirmed",
                            RequestAttempt = 1
                        });
                        int addedReq = ctx.SaveChanges();

                        if (addedReq > 0)
                        {
                            var result = await userManager.CreateAsync(user, "1234");
                            if (result.Succeeded)
                            {
                                await userManager.AddToRoleAsync(user, "Admin");
                                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                                await userManager.ConfirmEmailAsync(user, token);
                            }
                        }
                    }

                }

                // pre-load data to the building table
                string[] BuildingTypesArr = {"Duplex","Bungalow", "Block of Flats",
            "Detached House","Semi-Detached House" , "Story Building",
            "Terraced House", "Mud House", "Wooden House" };
                foreach (var type in BuildingTypesArr)
                {
                    var buildingTypeId = Guid.NewGuid().ToString();
                    ctx.BuildingTypes.Add(new BuildingType { TypeId = buildingTypeId, TypeName = type });
                    ctx.SaveChanges();
                }

                // fetch data from Data.json file and preload the tables
                string strResultJson = File.ReadAllText(@"../Groundforce.Services.Data/Data.json");
                var agents = JsonConvert.DeserializeObject<List<ApplicationUser>>(strResultJson);
                foreach (var agent in agents)
                {
                    var requestId = Guid.NewGuid().ToString();
                    ctx.Request.Add(new Request
                    {
                        RequestId = requestId,
                        PhoneNumber = agent.PhoneNumber,
                        Status = "confirmed",
                        RequestAttempt = 1
                    });

                    int requestAdded = ctx.SaveChanges();

                    if (requestAdded > 0)
                    {
                        var userId = Guid.NewGuid().ToString();
                        var userObj = new ApplicationUser
                        {
                            Id = userId,
                            UserName = agent.Email,
                            Email = agent.Email,
                            LastName = agent.LastName,
                            FirstName = agent.FirstName,
                            Gender = agent.Gender,
                            PhoneNumber = agent.PhoneNumber,
                            DOB = agent.DOB,
                            IsVerified = true,
                            IsActive = true
                        };

                        var userAdded = await userManager.CreateAsync(userObj, "1234");

                        var agentId = "";
                        if (userAdded.Succeeded)
                        {
                            await userManager.AddToRoleAsync(userObj, "Agent");

                            agentId = userId;
                            var agentObj = new FieldAgent
                            {
                                ApplicationUserId = agentId,
                                State = agent.FieldAgent.State,
                                LGA = agent.FieldAgent.LGA,
                                ZipCode = agent.FieldAgent.ZipCode,
                                ResidentialAddress = agent.FieldAgent.ResidentialAddress,
                                Longitude = agent.FieldAgent.Longitude,
                                Latitude = agent.FieldAgent.Latitude,
                                AdditionalPhoneNumber = agent.FieldAgent.AdditionalPhoneNumber,
                                BankName = agent.FieldAgent.BankName,
                                AccountName = agent.FieldAgent.AccountName,
                                AccountNumber = agent.FieldAgent.AccountNumber,
                            };

                            ctx.FieldAgents.Add(agentObj);
                            ctx.SaveChanges();
                        }


                        var itemId = Guid.NewGuid().ToString();
                        //var appUserId = await userManager.GetUsersInRoleAsync("Admin");
                        //string AddedBy = appUserId.FirstOrDefault().Id;
                        var item = new VerificationItem
                        {
                            ItemId = itemId,
                            Title = agent.FieldAgent.Missions.ToList()[0].VerificationItem.Title,
                            Description = agent.FieldAgent.Missions.ToList()[0].VerificationItem.Description,
                            ApplicationUserId = listOfUsers[0].Id,
                            UpdatedBy = listOfUsers[0].Id
                        };

                        ctx.VerificationItems.Add(item);
                        var itemAdded = ctx.SaveChanges();

                        if (itemAdded > 0)
                        {
                            var missionId = Guid.NewGuid().ToString();
                            var buildingType = ctx.BuildingTypes.FirstOrDefault().TypeId;
                            var mission = new Mission
                            {
                                MissionId = missionId,
                                VerificationItemId = itemId,
                                FieldAgentId = agentId
                            };

                            ctx.Missions.Add(mission);
                            ctx.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
