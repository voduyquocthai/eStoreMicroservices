using System;
using System.Threading.Tasks;
using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDistributedCache _redisCache;
		public BasketRepository(IDistributedCache redisCache)
		{
			_redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
		}
		public async Task<ShoppingCart> GetBasket(string userName)
		{
			var basket = await _redisCache.GetStringAsync(userName);

			if (string.IsNullOrEmpty(basket))
			{
				return null;
			}

			return JsonConvert.DeserializeObject<ShoppingCart>(basket);
		}

		public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
		{
			await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

			return await GetBasket(basket.UserName);
		}

		public async Task DeleteBasket(string username)
		{
			await _redisCache.RemoveAsync(username);
		}
	}
}
