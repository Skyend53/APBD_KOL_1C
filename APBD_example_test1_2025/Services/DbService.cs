using System.Data.Common;
using APBD_example_test1_2025.Exceptions;
using DeliveryService.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace DeliveryService.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;
    public DbService(IConfiguration configuration)
    {
        String def = "Default";
        _connectionString = configuration.GetConnectionString(def) ?? string.Empty;
    }

    public async Task<DeliveryDetailsDto> GetDeliveryByIdAsync(int deliveryId)
    {
        var query = @"
                    SELECT 
                    d.date,
                    c.first_name AS customer_first_name,
                    c.last_name AS customer_last_name,
                    c.date_of_birth,
                    a.first_name AS driver_first_name,
                    a.last_name AS driver_last_name,
                    a.licence_number,
                    b.name AS product_name,
                    b.price, 
                    e.amount
                    FROM Delivery d
                    JOIN Customer c ON d.customer_id = c.customer_id
                    JOIN Driver a ON d.driver_id = a.driver_id
                    LEFT JOIN Product_Delivery e ON d.delivery_id = e.delivery_id
                    LEFT JOIN Product b ON e.product_id = b.product_id
                    WHERE d.delivery_id = @deliveryId";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@deliveryId", deliveryId);

        await connection.OpenAsync();
        var read = await command.ExecuteReaderAsync();

        DeliveryDetailsDto? delivery = null;

        
        
        while (await read.ReadAsync())
        {
            if (delivery == null)
            {
                delivery = new DeliveryDetailsDto
                {
                    Date = read.GetDateTime(0),
                    Customer = new CustomerDto
                    {
                        FirstName = read.GetString(1),
                        LastName = read.GetString(2),
                        
                        DateOfBirth = read.GetDateTime(3)
                    },
                    Drivering = new DriveringDto
                    {
                        FirstName = read.GetString(4),
                        LastName = read.GetString(5),
                        
                        LicenceNumber = read.GetString(6)
                    },
                    
                    Products = new List<ProductDeliveryDto>()
                };
            }

            if (!read.IsDBNull(7))
            {
                delivery.Products.Add(new ProductDeliveryDto
                {
                    Name = read.GetString(7),
                    Price = read.GetDecimal(8),
                    Amount = read.GetInt32(9)
                });
            }
        }

        if (delivery == null)
        {
            String dnf = "Delivery not found.";
            throw new NotFoundException(dnf);
        }

        return delivery;
    }

    public async Task AddDeliveryAsync(CreateDeliveryReqDto deliveryReequest)
    {
        
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand();
        command.Connection = connection;
        

        await connection.OpenAsync();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            //delivery_id EXISTS
            command.CommandText = "SELECT 1 FROM Delivery WHERE delivery_id = @DeliveryId";
            command.Parameters.AddWithValue("@DeliveryId", deliveryReequest.DeliveryId);
            var deliveryExists = await command.ExecuteScalarAsync();
            //String strthr = $"Delivery with ID: {sth} :alreadyy exists.";
            if (deliveryExists is not null)
            {
                throw new ConflictException($"Delivery with ID: {deliveryReequest.DeliveryId} :alreadyy exists.");
            }

            //Customer
            command.CommandText = "SELECT 1 FROM Customer WHERE customer_id = @CustomerId";
            command.Parameters.AddWithValue("@CustomerId", deliveryReequest.CustomerId);
            var customerExists = await command.ExecuteScalarAsync();
            if (customerExists is null)
            {
                throw new NotFoundException("Customer with ID: "+ deliveryReequest.CustomerId + " :not found.");
            }

            //Driver
            command.CommandText = "SELECT 1 FROM Driver WHERE licence_number = @LicenceNumber";
            command.Parameters.AddWithValue("@LicenceNumber", deliveryReequest.LicenceNumber);
            var driverExists = await command.ExecuteScalarAsync();
            if (driverExists is null)
            {
                String dwl = "Driver with LicenceNumber: ";
                String nf = " :not found";
                throw new NotFoundException(dwl + deliveryReequest.LicenceNumber + nf);
            }

            //INSERT Delivery
            var deliveryDate = deliveryReequest.Date == DateTime.MinValue 
                ? DateTime.Now 
                : deliveryReequest.Date;

            command.CommandText = @"
                                    INSERT INTO Delivery (delivery_id, customer_id, driver_id, date)
                                    VALUES (@DeliveryId, @CustomerId, 
                                                                    (SELECT driver_id FROM Driver WHERE licence_number = @LicenceNumber), @Date)";
            command.Parameters.AddWithValue("@Date", deliveryDate);
            await command.ExecuteNonQueryAsync();

            
            foreach (var product in deliveryReequest.Products) //
            {
                command.Parameters.Clear();
                command.CommandText = "SELECT product_id FROM Product WHERE name = @ProductName";
                command.Parameters.AddWithValue("@ProductName", product.Name);
                
                var productId = await command.ExecuteScalarAsync();
                if (productId is null)
                    throw new NotFoundException($"Product {product.Name} not found.");
                
                command.Parameters.Clear();
                command.CommandText = @"
                                        INSERT INTO Product_Delivery (product_id, delivery_id, amount)
                                        VALUES (@ProductId, @DeliveryId, @Amount)";
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@DeliveryId", deliveryReequest.DeliveryId);
                command.Parameters.AddWithValue("@Amount", product.Amount);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}