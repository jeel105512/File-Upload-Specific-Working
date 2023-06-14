using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem {Controller = "Home", Action = "Index", Label = "Home"},
                new MenuItem {Controller = "Libraries", Action = "Index", Label = "Library", DropdownItems = new List<MenuItem>{
                    new MenuItem {Controller = "Libraries", Action = "Index", Label = "List"},
                    new MenuItem {Controller = "Libraries", Action = "Create", Label = "Create"},
                } },
                new MenuItem {Controller = "Books", Action = "Index", Label = "Book", DropdownItems = new List<MenuItem>{
                    new MenuItem {Controller = "Books", Action = "Index", Label = "List"},
                    new MenuItem {Controller = "Books", Action = "Create", Label = "Create"},
                } },
                new MenuItem {Controller = "Home", Action = "Privacy", Label = "Privacy"},
            };
            return View(menuItems);
        }
    }
}
