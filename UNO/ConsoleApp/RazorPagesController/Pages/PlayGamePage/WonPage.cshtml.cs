using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesController.Pages.PlayGamePage;

public class WonPageModel : PageModel
{
    
    [BindProperty]
    public string winnerName { get; set; }
    
    public async Task<IActionResult> OnGetAsync(string name)
    {
        winnerName = name;
        return Page();
    }
}