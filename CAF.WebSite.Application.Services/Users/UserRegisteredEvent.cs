using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Users
{
	/// <summary>
	/// An event message, which gets published after customer was registered
	/// </summary>
    public class UserRegisteredEvent
    {
        public User User
		{ 
			get; 
			set; 
		}
	}
}
