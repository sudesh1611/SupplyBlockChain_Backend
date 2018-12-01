using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SupplyBlockChain_Backend.Data;
using SupplyBlockChain_Backend.Models;

namespace SupplyBlockChain_Backend.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserDbContext userDbContext;
        private readonly IConfiguration cfg;

        public LoginModel(UserDbContext udb, IConfiguration cfgg)
        {
            userDbContext = udb;
            cfg = cfgg;
        }

        public void OnGet()
        {
            
            var user = userDbContext.UserAccounts.SingleOrDefault(m => m.ID == 1);
            //If there is no Admin Account. Create one using settings from appsettings file
            if (user == null)
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

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            public string Password { get; set; }
        }

        [BindProperty]
        public InputModel LoginInput { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (ModelState.IsValid)
            {
                LoginInput.UserName = LoginInput.UserName.ToLower();
                var user = await userDbContext.UserAccounts.SingleOrDefaultAsync(m => m.UserName == LoginInput.UserName);
                if (user != null)
                {
                    if (user.Password == LoginInput.Password)
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name,user.FullName),
                                new Claim(ClaimTypes.GivenName,user.UserName),
                            };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                        };
                        await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);
                        return RedirectToPage("/ShowProducts");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                    return Page();
                }
            }
            else
            {
                return Page();
            }
        }

    }
}