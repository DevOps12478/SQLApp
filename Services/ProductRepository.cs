
using Microsoft.Data.SqlClient;
using ProductsMvc.Models;

namespace ProductsMvc.Services
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            // Load the correct connection string
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Id, Name, Price, Stock FROM dbo.Products ORDER BY Name", conn);

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Stock = reader.GetInt32(3)
                });
            }

            return products;
        }
    }
}
