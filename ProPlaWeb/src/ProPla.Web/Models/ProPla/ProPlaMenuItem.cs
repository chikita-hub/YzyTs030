namespace ProPla.Web.Models.ProPla;

public class ProPlaMenuItem
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string ButtonKind { get; set; } = "Normal";
}
