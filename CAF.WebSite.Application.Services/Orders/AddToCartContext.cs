using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CAF.WebSite.Application.Services.Orders
{
	public class AddToCartContext
	{
		public AddToCartContext()
		{
			Warnings = new List<string>();
			CustomerEnteredPrice = decimal.Zero;
			ChildItems = new List<ShoppingCartItem>();
		}

		public List<string> Warnings { get; set; }

		public ShoppingCartItem Item { get; set; }
		public List<ShoppingCartItem> ChildItems { get; set; }
		 

		public User User { get; set; }
		public Article Article { get; set; }
		public ShoppingCartType CartType { get; set; }
		public NameValueCollection AttributeForm { get; set; }
		public string Attributes { get; set; }
		public decimal CustomerEnteredPrice { get; set; }
		public int Quantity { get; set; }
		public bool AddRequiredArticles { get; set; }
		public int? SiteId { get; set; }

		 
	}
}
