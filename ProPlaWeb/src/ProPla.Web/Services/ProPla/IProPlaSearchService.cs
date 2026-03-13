using ProPla.Web.Models.ProPla;

namespace ProPla.Web.Services.ProPla;

public interface IProPlaSearchService
{
    Task<List<ProPlaSearchResultRow>> SearchAsync(ProPlaSearchCondition condition, CancellationToken cancellationToken = default);
    Task<List<string>> GetStatusOptionsAsync(CancellationToken cancellationToken = default);
}
