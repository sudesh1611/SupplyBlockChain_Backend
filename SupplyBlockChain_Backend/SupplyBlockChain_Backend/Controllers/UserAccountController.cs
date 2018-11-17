using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SupplyBlockChain_Backend.Data;
using SupplyBlockChain_Backend.Models;

namespace SupplyBlockChain_Backend.Controllers
{
    public class UserAccountController : ControllerBase
    {
        private readonly UserDbContext _userContext;
        private readonly IConfiguration configuration;
        public UserAccountController(UserDbContext userDbContext, IConfiguration cfg)
        {
            _userContext = userDbContext;
            configuration = cfg;
            var user = userDbContext.UserAccounts.SingleOrDefault(m => m.ID == 1);
            //If there is no Admin Account. Create one using settings from appsettings file
            if(user==null)
            {
                var adminUser = new User()
                {
                    FullName = cfg.GetConnectionString("DefaultFullName"),
                    UserName = cfg.GetConnectionString("DefaultUserName"),
                    Password = cfg.GetConnectionString("DefaultPassword"),
                    ConfirmPassword = cfg.GetConnectionString("DefaultPassword"),
                    EmailID = cfg.GetConnectionString("DefaultEmailID"),
                    AssociatedProductTypes = JsonConvert.SerializeObject(new List<string>()),
                    AccessRights = JsonConvert.SerializeObject(new List<string>() { "Admin" })
                };
                userDbContext.UserAccounts.Add(adminUser);
                userDbContext.SaveChanges();
            }
        }

        [HttpPost]
        public async Task<string> CreateAccount(string userName,string password, string user)
        {
            var adminUser = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if(adminUser!=null)
            {
                if(adminUser.Password==password)
                {
                    var AccessRights = JsonConvert.DeserializeObject<List<string>>(adminUser.AccessRights);
                    if(AccessRights.Contains("CreateAccount") || AccessRights.Contains("Admin"))
                    {
                        var Accounts = await _userContext.UserAccounts.ToListAsync();
                        var newUser = JsonConvert.DeserializeObject<User>(user);
                        foreach (var item in Accounts)
                        {
                            if (item.UserName == newUser.UserName)
                            {
                                return "UserName";
                            }
                        }
                        await _userContext.UserAccounts.AddAsync(newUser);
                        await _userContext.SaveChangesAsync();
                        return "True";
                    }
                }
            }
            return "False";
        }

        [HttpPost]
        public async Task<string> UpdateAccount(string userName, string password, string user)
        {
            var adminUser = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if (adminUser != null)
            {
                if (adminUser.Password == password)
                {
                    var AccessRights = JsonConvert.DeserializeObject<List<string>>(adminUser.AccessRights);
                    if (AccessRights.Contains("CreateAccount") || AccessRights.Contains("Admin"))
                    {
                        var oldUser = JsonConvert.DeserializeObject<User>(user);
                        _userContext.UserAccounts.Update(oldUser);
                        await _userContext.SaveChangesAsync();
                        return "True";
                    }
                }
            }
            return "False";
        }

        [HttpPost]
        public async Task<string> LogIn(string userName, string password)
        {
            var user = await _userContext.UserAccounts.FirstOrDefaultAsync(m => m.UserName == userName);
            if(user!=null)
            {
                if(user.Password==password)
                {
                    return JsonConvert.SerializeObject(user);
                }
            }
            return "False";
        }
    }
}