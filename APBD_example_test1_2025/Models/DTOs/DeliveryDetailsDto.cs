namespace DeliveryService.Models.DTOs;

//1. DELIVERY
public class DeliveryDetailsDto
{
    public DateTime Date { get; set; }
    public CustomerDto Customer { get; set; } = null!;
    public DriveringDto Drivering { get; set; } = null!;
    public List<ProductDeliveryDto> Products { get; set; } = new List<ProductDeliveryDto>();
}

//2. DODAWANIE NOWEGO DELIVERY
public class CreateDeliveryReqDto
{
    public int DeliveryId { get; set; }
    public int CustomerId { get; set; }
    public string LicenceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<ProductDeliveryRequestDto> Products { get; set; } = new List<ProductDeliveryRequestDto>();
}

//CLIENT DLA 1.
public class CustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

//DRIVER DLA 1.
public class DriveringDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LicenceNumber { get; set; } = string.Empty;
}

//PRODUCT DLA 1.
public class ProductDeliveryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Amount { get; set; }
}

//PRODUCT DLA 2.
public class ProductDeliveryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
}