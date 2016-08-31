using System;
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Messages
{
	public class MessageResponse
	{
		public int Id { get; set; }

		[Required]
		public int ChatId { get; set; }

		[Required]
		public int MessageOrder { get; set; }

		[Required]
		public int UserId { get; set; }
		
		[Required]
		public string Content { get; set; }
		
		public DateTime UpdatedAt { get; set; }

		public int UpdatedBy { get; set; }

		public DateTime CreatedAt { get; set; }

		public int CreatedBy { get; set; }
	}
}