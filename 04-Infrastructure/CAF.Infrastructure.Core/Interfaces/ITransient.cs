using System;

namespace CAF.Infrastructure.Core
{
	public interface ITransient
	{
		bool IsTransient { get; set; }
	}
}
