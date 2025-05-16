namespace APBD_example_test1_2025.Models.DTOs;

public class DeliveryDetailsDto
{
    public DateTime Date { get; set; }
    public int Customer { get; set; }
    public int Driver { get; set; }
    public List<DeliveryDto> Products { get; set; } = new List<DeliveryDto>();
}

public class DeliveryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
}