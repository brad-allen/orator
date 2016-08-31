using System;
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Chats
{
	public enum ConnectionStatus
	{
		Unknown,
		Invited,
		Denied,
		Accepted
	}

	public class ChatConnection
	{
		public int Id { get; set; }

		[Required]
		public ConnectionStatus Status { get; set; }
		
		[Required]
		public int ChatId { get; set; }

		[Required]
		public int UserId { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int UpdatedBy { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		[Required]
		public int CreatedBy { get; set; }
	}
}