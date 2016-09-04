using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orator.Models.Messages
{
	public class Message
	{
		public int Id { get; set; }
		
		[Required, Index]
		public int ChatId { get; set; }
		
		[Required, Index]
		public int UserId { get; set; }
		
		[Required]
		public string Content { get; set; }
		
		public DateTime UpdatedAt { get; set; }

		public int UpdatedBy { get; set; }

		public DateTime CreatedAt { get; set; }

		public int CreatedBy { get; set; }
	}
}