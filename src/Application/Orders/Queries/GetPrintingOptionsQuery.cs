using Application.Common.Interfaces.Queries;
using Domain.PrintingOptions;

namespace Application.Orders.Queries;

public record GetPrintingOptionsQuery();

public static class GetPrintingOptionsQueryHandler
{
    public static async Task<IReadOnlyList<PrintingOption>> Handle(
        GetPrintingOptionsQuery query,
        IPrintingOptionQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAll(cancellationToken);
    }
}