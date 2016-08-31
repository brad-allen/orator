using Orator.Models.Messages;
using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Collections.Generic;

namespace MakeMusicAdmin.Tests.Models
{
	public class CreateMessageRequestTest
	{
		private static CreateMessageRequest GetValidCreateMessageRequest()
		{
			return new CreateMessageRequest
			{
				ChatId = 1,
				UserId = 1,
				Content = "My Content"
			};
		}
		private bool IsValid(CreateMessageRequest request)
		{
			return Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true);
		}

		[Fact]
		public void CreateMessageRequest_PassesValidation()
		{
			var request = GetValidCreateMessageRequest();
			Assert.True(IsValid(request));
		}

		[Fact]
		public void CreateMessageRequest_InvalidChatId_FailsValidation()
		{
			var request = GetValidCreateMessageRequest();
			request.ChatId = -1;
			Assert.False(IsValid(request));
		}

		[Fact]
		public void CreateMessageRequest_InvalidUserId_FailsValidation()
		{
			var request = GetValidCreateMessageRequest();
			request.UserId = -1;
			Assert.False(IsValid(request));
		}

		[Fact]
		public void CreateMessageRequest_NoContent_FailsValidation()
		{
			var request = GetValidCreateMessageRequest();
			request.Content = null;
			Assert.False(IsValid(request));
		}
	}
}
