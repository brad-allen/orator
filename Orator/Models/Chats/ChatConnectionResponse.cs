using System;

namespace Orator.Models.Chats
{
	public class ChatConnectionResponse
	{
		public int Id { get; set; }
		public ConnectionStatus Status { get; set; }
		public int ChatId { get; set; }
		public int UserId { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public int CreatedBy { get; set; }
	}
}