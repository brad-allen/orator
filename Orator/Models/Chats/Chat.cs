using System;
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Chats
{
	public class Chat
	{
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }
		
		[Required]
		public bool AllowHtml { get; set; }

		public DateTime UpdatedAt { get; set; }

		public int UpdatedBy { get; set; }

		public DateTime CreatedAt { get; set; }

		public int CreatedBy { get; set; }
		
	}
}