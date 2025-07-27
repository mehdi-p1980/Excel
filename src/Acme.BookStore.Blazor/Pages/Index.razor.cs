using System.Threading.Tasks;

namespace Acme.BookStore.Blazor.Pages;

public partial class Index
{
    protected override async Task OnInitializedAsync()
    {
        await MembershipStatusChecker.CheckAndRedirect();
    }
}
