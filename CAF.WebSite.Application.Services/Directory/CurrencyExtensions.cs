﻿using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Directory;
using System;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Directory
{
	public static class CurrencyExtensions
	{
		public static bool HasDomainEnding(this Currency currency, string domain)
		{
			if (currency != null && domain.HasValue() && currency.DomainEndings.HasValue())
			{
				var endings = currency.DomainEndings.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var ending in endings)
				{
					if (domain.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase))
						return true;
				}
			}
			return false;
		}

		public static Currency GetByDomainEnding(this IEnumerable<Currency> currencies, string domain)
		{
			if (currencies != null && domain.HasValue())
			{
				foreach (var currency in currencies)
				{
					if (currency.Published && currency.HasDomainEnding(domain))
						return currency;
				}
			}
			return null;
		}
	}
}
