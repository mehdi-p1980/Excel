using Acme.BookStore.Localization;
using Acme.BookStore.Permissions;
using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Acme.BookStore.Blazor.Menus;

public class BookStoreMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<BookStoreResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                "BookStore.Home",
                l["Menu:Home"],
                "/",
                icon: "fas fa-home"
            )
        );

        var bookStoreMenu = new ApplicationMenuItem(
            "BooksStore",
            l["Menu:BookStore"],
            icon: "fa fa-book"
        );

        context.Menu.AddItem(bookStoreMenu);

        //CHECK the PERMISSION
        if (await context.IsGrantedAsync(BookStorePermissions.Books.Default))
        {
            bookStoreMenu.AddItem(new ApplicationMenuItem(
                "BooksStore.Books",
                l["Menu:Books"],
                url: "/books"
            ));
        }

        if (await context.IsGrantedAsync(BookStorePermissions.Authors.Default))
        {
            bookStoreMenu.AddItem(new ApplicationMenuItem(
                "BooksStore.Authors",
                l["Menu:Authors"],
                url: "/authors"
            ));
        }

        // Add Excel Import menu item
        //CHECK the PERMISSION (assuming same permission as books for now)
        if (await context.IsGrantedAsync(BookStorePermissions.Books.Default))
        {
            bookStoreMenu.AddItem(new ApplicationMenuItem(
                "BooksStore.ExcelImport", // A unique name for the menu item
                l["Menu:ExcelImport"], // Display name - assuming you'll add this to localization
                url: "/excel-import", // URL of the new page
                icon: "fa fa-file-excel" // Icon for the menu item
            ));
        }
    }
}
