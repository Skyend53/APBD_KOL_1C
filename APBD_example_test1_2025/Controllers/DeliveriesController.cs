using APBD_example_test1_2025.Exceptions;

using DeliveryService.Models.DTOs;
using DeliveryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveriesController : ControllerBase
{
    private readonly IDbService _dbService;

    public DeliveriesController(IDbService dbService)
    {
        _dbService = dbService;
    }

    
    
    //GET http://localhost:5000/api/deliveries/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryById(int id)
    {
        try
        {
            var result = await _dbService.GetDeliveryByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException exxx)
        {
            return NotFound(exxx.Message);
        }
    }

    //POST http://localhost:5000/api/deliveries
    [HttpPost]
    public async Task<IActionResult> AddDelivery(CreateDeliveryReqDto req)
    {
        if (req.Products == null || !req.Products.Any())
        {
            return BadRequest("At least one entry of a product is required.");
        }

        try
        {
            await _dbService.AddDeliveryAsync(req);
            return CreatedAtAction(nameof(GetDeliveryById), new { id = req.DeliveryId }, req);
        }
        catch (NotFoundException eyyy)
        {
            return NotFound(eyyy.Message);
        }
        catch (ConflictException ezzz)
        {
            return Conflict(ezzz.Message);
        }
    }
}