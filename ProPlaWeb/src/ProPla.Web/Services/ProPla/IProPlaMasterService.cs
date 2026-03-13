using ProPla.Web.Data;

namespace ProPla.Web.Services.ProPla;

public interface IProPlaMasterService
{
    Task<ProductPlanning?> GetByIdAsync(string productPlanningId, CancellationToken cancellationToken = default);
}
