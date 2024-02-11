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
        private readonly IUsersService _usersService;

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<SelectListItem> Locales { get; set; } = new List<SelectListItem> 
        { 
            new SelectListItem { Value = "ru", Text = "Russia" },
            new SelectListItem { Value = "en", Text = "USA" },
        };

        public IList<User> Users { get; set; } = default!;

        public IndexModel(ILogger<IndexModel> logger, IUsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        public void OnGet()
        {
            Users = _usersService.GetUsers().ToList();
        }
    }

    public class InputModel
    {
        public string Locale { get; set; }
    }
}
