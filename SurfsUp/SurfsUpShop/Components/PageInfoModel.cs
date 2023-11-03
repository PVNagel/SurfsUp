using Microsoft.AspNetCore.Components;

namespace SurfsUpShop.Web.BlazorWasm.Components
{
    public abstract class PageInfoModel : ComponentBase
    {
        [Parameter]
        public virtual string? Title { get; set; }

        [Parameter]
        public virtual string? Description { get; set; }
    }
}
