
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Users
{
	public class UpdateUserRequest
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string FirstName { get; set; }
		
		[Required]
		public string LastName { get; set; }

		[Required, StringLength(4000)]
		public string Bio { get; set; }
	}
}