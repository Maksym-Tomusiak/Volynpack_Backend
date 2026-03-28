using Domain.PrintingOptions;

namespace Application.Common.Interfaces.Queries;

public interface IPrintingOptionQueries
{
    Task<IReadOnlyList<PrintingOption>> GetAll(CancellationToken cancellationToken);
}