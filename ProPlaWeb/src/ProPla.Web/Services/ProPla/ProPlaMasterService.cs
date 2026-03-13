using Microsoft.EntityFrameworkCore;
using ProPla.Web.Data;

namespace ProPla.Web.Services.ProPla;

public class ProPlaMasterService : IProPlaMasterService
{
    private readonly AppDbContext _dbContext;

    public ProPlaMasterService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductPlanning?> GetByIdAsync(string productPlanningId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductPlannings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductPlanningId == productPlanningId, cancellationToken);
    }
}
