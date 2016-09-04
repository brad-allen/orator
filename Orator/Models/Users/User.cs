using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orator.Models.Users
{
	public class User
	{
		
		public int Id { get; set; }

		[Index, StringLength(128)]
		public string AuthId { get; set; }

		[Index, StringLength(50)]
		public string Username { get; set; }

		[Index, StringLength(100)]
		public string FirstName { get; set; }

		[Index, StringLength(100)]
		public string LastName { get; set; }

		[Index, StringLength(254)]
		public string Email { get; set; }

		public string Bio { get; set; }

		public DateTime CreatedAt { get; set; }

		public int CreatedBy { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public int? UpdatedBy { get; set; }
		
	}
}