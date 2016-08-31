
using System.ComponentModel.DataAnnotations;

namespace Orator.Models.Messages
{
	public class CreateMessageRequest
	{
		[Required, Range(1, int.MaxValue)]
		public int ChatId { get; set; }

		[Required, Range(1, int.MaxValue)]
		public int UserId { get; set; }
		
		[Required, StringLength(2000), MinLength(1)]
		public string Content { get; set; }
		
	}
}