
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Chats
{
	public class CreateChatRequest
	{
		[Required, StringLength(100)]
		public string Title { get; set; }

		[Required]
		public bool AllowHtml { get; set; } = false;

	}
}