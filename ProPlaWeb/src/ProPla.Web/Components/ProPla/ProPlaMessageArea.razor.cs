using Microsoft.AspNetCore.Components;

namespace ProPla.Web.Components.ProPla;

public partial class ProPlaMessageArea
{
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string ErrorMessage { get; set; } = string.Empty;
    [Parameter] public string SuccessMessage { get; set; } = string.Empty;
}
