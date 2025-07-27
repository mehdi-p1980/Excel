using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Acme.BookStore.Blazor.Pages;

public partial class Index
{
    [Inject]
    private MembershipStatusChecker MembershipStatusChecker { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await MembershipStatusChecker.CheckAndRedirect();
    }
}
