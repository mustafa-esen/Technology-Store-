using MediatR;

namespace ProductService.Application.Features.Products.Queries.CheckStock;

public class CheckStockQuery : IRequest<CheckStockResult>
{
    public List<StockCheckItem> Items { get; set; } = new();
}

public class StockCheckItem
{
    public Guid ProductId { get; set; }
    public int RequiredQuantity { get; set; }
}

public class CheckStockResult
{
    public bool IsAvailable { get; set; }
    public List<StockIssue> Issues { get; set; } = new();
}

public class StockIssue
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int RequiredQuantity { get; set; }
    public int AvailableStock { get; set; }
}
