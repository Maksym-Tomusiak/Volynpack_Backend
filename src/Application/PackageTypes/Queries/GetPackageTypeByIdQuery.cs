using Application.Common.Interfaces.Queries;
using Application.PackageTypes.Exceptions;
using Domain.PackageTypes;
using LanguageExt;

namespace Application.PackageTypes.Queries;

public record GetPackageTypeByIdQuery(Guid Id);

public static class GetPackageTypeByIdQueryHandler
{
    public static async Task<Either<PackageTypeException, PackageType>> Handle(
        GetPackageTypeByIdQuery query,
        IPackageTypeQueries packageTypeQueries,
        CancellationToken cancellationToken)
    {
        var packageTypeId = new PackageTypeId(query.Id);
        var result = await packageTypeQueries.GetById(packageTypeId, cancellationToken);
        return result.Match<Either<PackageTypeException, PackageType>>(
            packageType => packageType,
            () => new PackageTypeNotFoundException(query.Id));
    }
}
