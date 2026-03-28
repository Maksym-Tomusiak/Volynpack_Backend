using Application.Common.Interfaces.Repositories;
using Application.Hashtags.Exceptions;
using Domain.Hashtags;
using LanguageExt;

namespace Application.Hashtags.Commands;

public record CreateHashtagCommand(string NameUk, string NameEn);

public static class CreateHashtagCommandHandler
{
    public static async Task<Either<HashtagException, Hashtag>> Handle(
        CreateHashtagCommand command,
        IHashtagRepository hashtagRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var name = new Domain.LocalizedString(command.NameUk, command.NameEn);
            var hashtag = Hashtag.New(name);
            return await hashtagRepository.Add(hashtag, cancellationToken);
        }
        catch (Exception ex)
        {
            return new HashtagUnknownException(Guid.Empty, ex);
        }
    }
}

