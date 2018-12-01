using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SupplyBlockChain_Backend.Pages
{
    public class GenerateQrCodeModel : PageModel
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductID { get; set; }

        public IActionResult OnGet(string name, string type, string ID)
        {
            if (User.Identity.IsAuthenticated)
            {
                ProductName = name;
                ProductType = type;
                ProductID = ID;
                return Page();
            }
            else
            {
                return RedirectToPage("/Login");
            }
        }
    }
}