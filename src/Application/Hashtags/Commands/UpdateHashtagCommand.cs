using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Hashtags.Exceptions;
using Domain.Hashtags;
using LanguageExt;

namespace Application.Hashtags.Commands;

public record UpdateHashtagCommand(Guid Id, string NameUk, string NameEn);

public static class UpdateHashtagCommandHandler
{
    public static async Task<Either<HashtagException, Hashtag>> Handle(
        UpdateHashtagCommand command,
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
            var name = new Domain.LocalizedString(command.NameUk, command.NameEn);
            hashtag.Name = name;
            return await hashtagRepository.Update(hashtag, cancellationToken);
        }
        catch (Exception ex)
        {
            return new HashtagUnknownException(command.Id, ex);
        }
    }
}

