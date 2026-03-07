using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.PackageFittings.Exceptions;
using Domain.PackageFittings;
using LanguageExt;

namespace Application.PackageFittings.Commands;

public record DeletePackageFittingCommand(Guid Id);

public static class DeletePackageFittingCommandHandler
{
    public static async Task<Either<PackageFittingException, PackageFitting>> Handle(
        DeletePackageFittingCommand command,
        IPackageFittingRepository packageFittingRepository,
        IPackageFittingQueries packageFittingQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var fittingId = new PackageFittingId(command.Id);
        var existing = await packageFittingQueries.GetById(fittingId, cancellationToken);
        if (existing.IsNone)
            return new PackageFittingNotFoundException(command.Id);

        try
        {
            var fitting = existing.IfNoneUnsafe((PackageFitting)null!)!;

            // Delete associated image file
            if (!string.IsNullOrEmpty(fitting.FittingImageUrl))
                await fileService.DeleteFileAsync(fitting.FittingImageUrl, cancellationToken);

            return await packageFittingRepository.Delete(fitting, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageFittingUnknownException(command.Id, ex);
        }
    }
}
