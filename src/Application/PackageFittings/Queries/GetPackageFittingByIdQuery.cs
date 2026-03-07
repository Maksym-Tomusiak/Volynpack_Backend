using Application.Common.Interfaces.Queries;
using Application.PackageFittings.Exceptions;
using Domain.PackageFittings;
using LanguageExt;

namespace Application.PackageFittings.Queries;

public record GetPackageFittingByIdQuery(Guid Id);

public static class GetPackageFittingByIdQueryHandler
{
    public static async Task<Either<PackageFittingException, PackageFitting>> Handle(
        GetPackageFittingByIdQuery query,
        IPackageFittingQueries packageFittingQueries,
        CancellationToken cancellationToken)
    {
        var fittingId = new PackageFittingId(query.Id);
        var result = await packageFittingQueries.GetById(fittingId, cancellationToken);
        return result.Match<Either<PackageFittingException, PackageFitting>>(
            fitting => fitting,
            () => new PackageFittingNotFoundException(query.Id));
    }
}
