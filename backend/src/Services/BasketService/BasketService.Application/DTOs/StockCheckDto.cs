namespace BasketService.Application.DTOs;

public class StockCheckRequest
{
    public List<StockCheckItemDto> Items { get; set; } = new();
}

public class StockCheckItemDto
{
    public Guid ProductId { get; set; }
    public int RequiredQuantity { get; set; }
}

public class StockCheckResponse
{
    public bool IsAvailable { get; set; }
    public List<StockIssueDto> Issues { get; set; } = new();
}

public class StockIssueDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int RequiredQuantity { get; set; }
    public int AvailableStock { get; set; }
}
