using System;
using Orator.Models.Messages;
using Orator.Models.Users;
using System.Collections.Generic;

namespace Orator.Models.Chats
{
	public class ChatResponse
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public bool AllowHtml { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public int CreatedBy { get; set; }
	}
}