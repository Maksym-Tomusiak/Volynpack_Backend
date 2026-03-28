using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.ConsultationRequest;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConsultationRequestRepository(ApplicationDbContext context) 
    : IConsultationRequestRepository, IConsultationRequestQueries
{
    public async Task<ConsultationRequest> Add(ConsultationRequest request, CancellationToken cancellationToken)
    {
        await context.ConsultationRequests.AddAsync(request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<ConsultationRequest> Update(ConsultationRequest request, CancellationToken cancellationToken)
    {
        var tracked = await context.ConsultationRequests
            .FirstAsync(x => x.Id == request.Id, cancellationToken);

        context.Entry(tracked).CurrentValues.SetValues(request);

        await context.SaveChangesAsync(cancellationToken);
        return tracked;
    }

    public async Task<ConsultationRequest> Delete(ConsultationRequest request, CancellationToken cancellationToken)
    {
        context.ConsultationRequests.Remove(request);
        await context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<Option<ConsultationRequest>> GetById(ConsultationRequestId id, CancellationToken cancellationToken)
    {
        var entity = await context.ConsultationRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<ConsultationRequest>.None : Option<ConsultationRequest>.Some(entity);
    }

    public async Task<PaginatedResult<ConsultationRequest>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.ConsultationRequests.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = $"%{parameters.SearchTerm}%";
            query = query.Where(x => EF.Functions.ILike(x.PhoneNumber, term));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "createdat" => parameters.SortDescending 
                    ? query.OrderByDescending(x => x.CreatedAt) 
                    : query.OrderBy(x => x.CreatedAt),
                "isactive" => parameters.SortDescending 
                    ? query.OrderByDescending(x => x.IsActive) 
                    : query.OrderBy(x => x.IsActive),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ConsultationRequest>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}