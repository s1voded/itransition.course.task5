using Bogus;
using FakeUsersWebApp.Model;
using FakeUsersWebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FakeUsersWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFakeUsersService _usersService;

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public IEnumerable<SelectListItem> Locales { get; set; } = new List<SelectListItem> 
        { 
            new SelectListItem { Value = "ru", Text = "Russia" },
            new SelectListItem { Value = "en", Text = "United States" },
            new SelectListItem { Value = "de", Text = "Germany" },
        };

        public IList<User> Users { get; set; } = default!;

        public IndexModel(ILogger<IndexModel> logger, IFakeUsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        public void OnGet()
        {
            Users = _usersService.GetUsers(20, "ru", 10.5f, 100500).ToList();
        }
    }

    public class InputModel
    {
        public string Locale { get; set; }
    }
}
