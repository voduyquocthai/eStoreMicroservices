using System;
using System.Threading.Tasks;
using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repositories
{
	public class DiscountRepository : IDiscountRepository
	{
		private readonly IConfiguration _configuration;
		public DiscountRepository(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException();
		}
		public async Task<Coupon> GetDiscount(string productName)
		{
			await using var connection =
				new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

			var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
				"SELECT * FROM Coupon WHERE ProductName = @ProductName", new {ProductName = productName});
			if (coupon == null)
			{
				return new Coupon
				{
					ProductName = "No Discount",
					Amount = 0,
					Description = "No Discount Desc"
				};
			}

			return coupon;
		}

		public async Task<bool> CreateDiscount(Coupon coupon)
		{
			await using var connection =
				new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

			var createdCount = await connection.ExecuteAsync(
				"INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
				new {ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount});

			return createdCount != 0;
		}

		public async Task<bool> UpdateDiscount(Coupon coupon)
		{
			await using var connection =
				new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

			var updatedCount = await connection.ExecuteAsync(
				"UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
				new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

			return updatedCount != 0;
		}

		public async Task<bool> DeleteDiscount(string productName)
		{
			await using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

			var deletedCount = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
				new { ProductName = productName });

			return deletedCount != 0;
		}
	}
}
