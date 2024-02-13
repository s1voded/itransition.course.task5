using Bogus;
using FakeUsersWebApp.Model;
using FakeUsersWebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FakeUsersWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFakeUsersService _fakeUsersService;

        [BindProperty(SupportsGet = true)]
        public string Locale { get; set; }

        [BindProperty(SupportsGet = true)]
        public float CountErrors { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Seed { get; set; }

        [BindProperty(SupportsGet = true)]
        public int UserIds { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CountUsers { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; }

        public IEnumerable<SelectListItem> Locales { get; set; } = new List<SelectListItem> 
        { 
            new SelectListItem { Value = "ru", Text = "Russia" },
            new SelectListItem { Value = "en", Text = "United States" },
            new SelectListItem { Value = "de", Text = "Germany" },
        };

        public IList<User> Users { get; set; } = default!;

        public IndexModel(ILogger<IndexModel> logger, IFakeUsersService fakeUsersService)
        {
            _logger = logger;
            _fakeUsersService = fakeUsersService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetPartialTable()
        {
            Users = _fakeUsersService.GetUsers(CountUsers, UserIds, Locale, CountErrors, Seed + PageNum).ToList();
            return Partial("_PartialTable", Users);
        }
    }
}
