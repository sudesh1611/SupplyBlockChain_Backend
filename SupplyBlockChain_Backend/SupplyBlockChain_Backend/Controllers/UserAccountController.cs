using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupplyBlockChain_Backend.Data;
using SupplyBlockChain_Backend.Models;

namespace SupplyBlockChain_Backend.Controllers
{
    public class UserAccountController : ControllerBase
    {
        private readonly UserDbContext _userContext;
        public UserAccountController(UserDbContext userDbContext)
        {
            _userContext = userDbContext;
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
                    if(AccessRights.Contains("CreateAccount"))
                    {
                        var newUser = JsonConvert.DeserializeObject<User>(user);
                        foreach (var item in _userContext.UserAccounts)
                        {
                            if (item.UserName == newUser.UserName)
                            {
                                return "False";
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
                    if (AccessRights.Contains("CreateAccount"))
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