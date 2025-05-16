using DeliveryService.Models.DTOs;
namespace DeliveryService.Services;
public interface IDbService
{
    
    
    
    Task<DeliveryDetailsDto> GetDeliveryByIdAsync(int deliveryId);
    Task AddDeliveryAsync(CreateDeliveryReqDto deliveryReequest);
    
    
    
}