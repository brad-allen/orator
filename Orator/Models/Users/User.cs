using System;

namespace Orator.Models.Users
{
	public class User
	{
		
		public int Id { get; set; }

		public string AuthId { get; set; }

		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string Bio { get; set; }

		public DateTime CreatedAt { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public int? UpdatedBy { get; set; }
		
	}
}