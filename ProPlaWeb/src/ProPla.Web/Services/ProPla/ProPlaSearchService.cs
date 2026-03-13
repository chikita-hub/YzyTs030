using Microsoft.EntityFrameworkCore;
using ProPla.Web.Data;
using ProPla.Web.Models.ProPla;

namespace ProPla.Web.Services.ProPla;

public class ProPlaSearchService : IProPlaSearchService
{
    private readonly AppDbContext _dbContext;

    public ProPlaSearchService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetStatusOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductPlannings
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.ProductReleaseStatus))
            .Select(x => x.ProductReleaseStatus!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProPlaSearchResultRow>> SearchAsync(ProPlaSearchCondition condition, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ProductPlannings.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(condition.Status))
        {
            query = query.Where(x => x.ProductReleaseStatus == condition.Status);
        }

        if (!string.IsNullOrWhiteSpace(condition.Keyword))
        {
            var keyword = condition.Keyword;
            query = query.Where(x =>
                (x.ProductPlanningId ?? string.Empty).Contains(keyword) ||
                (x.ProductCategory ?? string.Empty).Contains(keyword) ||
                (x.ProductPerson ?? string.Empty).Contains(keyword) ||
                (x.ProductNameOfficial ?? string.Empty).Contains(keyword) ||
                (x.ProductReleaseStatus ?? string.Empty).Contains(keyword));
        }

        return await query
            .OrderBy(x => x.ProductPlanningId)
            .Select(x => new ProPlaSearchResultRow
            {
                ProductCode = x.ProductPlanningId ?? string.Empty,
                ProductCategory = x.ProductCategory ?? string.Empty,
                ProductPerson = x.ProductPerson ?? string.Empty,
                ProductName = x.ProductNameOfficial ?? string.Empty,
                Status = x.ProductReleaseStatus ?? string.Empty
            })
            .ToListAsync(cancellationToken);
    }
}
