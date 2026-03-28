using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Hashtags.Exceptions;
using Domain.Hashtags;
using LanguageExt;

namespace Application.Hashtags.Commands;

public record DeleteHashtagCommand(Guid Id);

public static class DeleteHashtagCommandHandler
{
    public static async Task<Either<HashtagException, Hashtag>> Handle(
        DeleteHashtagCommand command,
        IHashtagRepository hashtagRepository,
        IHashtagQueries hashtagQueries,
        CancellationToken cancellationToken)
    {
        var hashtagId = new HashtagId(command.Id);
        var existing = await hashtagQueries.GetById(hashtagId, cancellationToken);
        if (existing.IsNone)
            return new HashtagNotFoundException(command.Id);

        try
        {
            var hashtag = existing.IfNoneUnsafe((Hashtag)null!)!;
            return await hashtagRepository.Delete(hashtag, cancellationToken);
        }
        catch (Exception ex)
        {
            return new HashtagUnknownException(command.Id, ex);
        }
    }
}

