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
                        Active = true
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
                        Active = true
                    }
                };


                if (!ctx.Request.Any())
                {
                    foreach (var user in listOfUsers)
                    {
                        var requestId = Guid.NewGuid().ToString();
                        ctx.Request.Add(new Request
                        {
                            RequestId = requestId,
                            PhoneNumber = user.PhoneNumber,
                            IsConfirmed = true,
                            RequestAttempt = 1,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                        int addedReq = ctx.SaveChanges();

                        if (addedReq > 0)
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
            "Detached House","Semi-Detached House" , "Story Building",
            "Terraced House", "Mud House", "Wooden House" };
                var adminIdFk = ctx.Admins.FirstOrDefault().AdminId;
                foreach (var type in BuildingTypesArr)
                {
                    var buildingTypeId = Guid.NewGuid().ToString();
                    ctx.BuildingTypes.Add(new BuildingType { TypeId = buildingTypeId, TypeName = type, AdminId = adminIdFk, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
                    ctx.SaveChanges();
                }


                string strResultJson = File.ReadAllText(@"../Groundforce.Services.Data/Data.json");
                var agents = JsonConvert.DeserializeObject<List<ApplicationUser>>(strResultJson);
                foreach (var agent in agents)
                {
                    var requestId = Guid.NewGuid().ToString();
                    ctx.Request.Add(new Request
                    {
                        RequestId = requestId,
                        PhoneNumber = agent.PhoneNumber,
                        IsConfirmed = true,
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
                            PlaceOfBirth = agent.PlaceOfBirth,
                            State = agent.State,
                            LGA = agent.LGA,
                            HomeAddress = agent.HomeAddress,
                            Active = true
                        };

                        var userAdded = await userManager.CreateAsync(userObj, "1234");

                        var agentId = "";
                        int agentAdded = 0;
                        if (userAdded.Succeeded)
                        {
                            await userManager.AddToRoleAsync(userObj, "Agent");

                            agentId = Guid.NewGuid().ToString();
                            var agentObj = new FieldAgent
                            {
                                FieldAgentId = agentId,
                                ApplicationUserId = userId,
                                Longitude = agent.FieldAgent.Longitude,
                                Latitude = agent.FieldAgent.Latitude,
                                Religion = agent.FieldAgent.Religion,
                                AdditionalPhoneNumber = agent.FieldAgent.AdditionalPhoneNumber,
                            };

                            ctx.FieldAgents.Add(agentObj);
                            agentAdded = ctx.SaveChanges();
                        }

                        if (agentAdded > 0)
                        {
                            var accountId = Guid.NewGuid().ToString();
                            var account = new BankAccount
                            {
                                AccountId = accountId,
                                AccountName = agent.FieldAgent.BankAccounts.AccountName,
                                AccountNumber = agent.FieldAgent.BankAccounts.AccountNumber,
                                FieldAgentId = agentId
                            };

                            ctx.BankAccounts.Add(account);
                            ctx.SaveChanges();
                        }

                        var itemId = Guid.NewGuid().ToString();
                        var appUserId = ctx.Admins.FirstOrDefault().ApplicationUserId;
                        var item = new VerificationItem
                        {
                            ItemId = itemId,
                            ItemName = agent.FieldAgent.Missions.ToList()[0].VerificationItem.ItemName,
                            ApplicationUserId = appUserId
                        };

                        ctx.VerificationItems.Add(item);
                        var itemAdded = ctx.SaveChanges();

                        if (itemAdded > 0)
                        {
                            var missionId = Guid.NewGuid().ToString();
                            var buildingType = ctx.BuildingTypes.FirstOrDefault().TypeId;
                            var adminId = ctx.Admins.FirstOrDefault().AdminId;
                            var mission = new Mission
                            {
                                MissionId = missionId,
                                VerificationItemId = itemId,
                                BuildingTypeId = buildingType,
                                FieldAgentId = agentId,
                                AdminId = adminId
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
