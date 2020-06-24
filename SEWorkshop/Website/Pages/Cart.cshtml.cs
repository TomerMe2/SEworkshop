using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;

namespace Website.Pages
{
    public class CartModel : PageModel
    {
        public IUserManager UserManager { get; }

        [BindProperty (SupportsGet = true)]
        public string ErrorMsg { get; set; }
        public DataCart? Cart { get; private set; }

        public CartModel(IUserManager userManager)
        {
            UserManager = userManager;
            ErrorMsg = "";
        }

        public void OnGet()
        {
            Cart = UserManager.GetUser(HttpContext.Session.Id).Cart;
        }

        public IActionResult OnPostBillingAsync(string StoreName)
        {
            return RedirectToPage("./Billing", new {storeName = StoreName});
        }
        
        public IActionResult OnPostRemoveFromCartAsync(string StoreName, string ProductName, string Quantity)
        {
            try
            {
                UserManager.RemoveProductFromCart(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Quantity));
            }
            catch
            {
                ErrorMsg = "An Error Has Occured";
                return new PageResult();
            }
            return RedirectToPage("./Cart");
        }
    }
}