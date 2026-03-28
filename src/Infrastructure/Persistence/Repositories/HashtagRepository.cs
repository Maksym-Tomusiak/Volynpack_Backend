using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Hashtags;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

public class HashtagRepository(ApplicationDbContext context) : IHashtagRepository, IHashtagQueries
{
    public async Task<Hashtag> Add(Hashtag hashtag, CancellationToken cancellationToken)
    {
        await context.Hashtags.AddAsync(hashtag, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return hashtag;
    }

    public async Task<Hashtag> Update(Hashtag hashtag, CancellationToken cancellationToken)
    {
        context.Hashtags.Update(hashtag);
        await context.SaveChangesAsync(cancellationToken);
        return hashtag;
    }

    public async Task<Hashtag> Delete(Hashtag hashtag, CancellationToken cancellationToken)
    {
        context.Hashtags.Remove(hashtag);
        await context.SaveChangesAsync(cancellationToken);
        return hashtag;
    }

    public async Task<IReadOnlyList<Hashtag>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Hashtags
            .AsNoTracking()
            .OrderBy(x => x.Name.Uk)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Hashtag>> GetById(HashtagId id, CancellationToken cancellationToken)
    {
        var entity = await context.Hashtags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<Hashtag>.None : Option<Hashtag>.Some(entity);
    }

    public async Task<IReadOnlyList<Hashtag>> GetByIds(IEnumerable<HashtagId> ids, CancellationToken cancellationToken)
    {
        var idValues = ids.Select(x => x.Value).ToArray();

        if (idValues.Length == 0)
            return [];

        var param = new NpgsqlParameter<Guid[]>("ids", idValues);

        return await context.Hashtags
            .FromSqlRaw("SELECT * FROM hashtags WHERE id = ANY(@ids)", param)
            .ToListAsync(cancellationToken);
    }
}
