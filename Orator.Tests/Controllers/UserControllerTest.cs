using Orator.Models.Users;
using Orator.Controllers.V1;
using Xunit;
using Moq;
using Orator.DB;
using System.Web.Http.Results;

namespace MakeMusicAdmin.Tests.Controllers
{
	public class UserControllerTest
	{
		Mock<DatabaseContext> _dbContext;
		TestUserController _controller;

		public class TestUserController :UserController
		{
			bool _hasValidCookie = true;
			public void OverrideAuthentication(bool isAuthed)
			{
				_hasValidCookie = isAuthed;
			}
			public override bool HasValidCookie => _hasValidCookie;
		}

		public UserControllerTest()
		{
			_dbContext = new Mock<DatabaseContext>();
			_controller = new TestUserController();
			_dbContext.Setup(i => i.GetUserResponse(It.IsAny<int>())).Returns(GetValidUserResponse());
			_controller.DBContext = _dbContext.Object;
		}

		private static UserResponse GetValidUserResponse()
		{
			return new UserResponse
			{
				Bio = "bio",
				Id = 1,
				FirstName = "Brad",
				LastName = "Allen",
				Email = "email@email.com",
				CreatedAt = System.DateTime.UtcNow,
				UpdatedAt = System.DateTime.UtcNow,
				Username = "Brad"
			};
		}
		
		[Fact]
		public void GetUser_HasNoCookie_UnauthorizedResponse()
		{
			_controller.OverrideAuthentication(false);

			var result = _controller.Get() as UnauthorizedResult;

			Assert.NotNull(result);
		}

		[Fact]
		public void GetUser_UserNotFound_NotFoundResponse()
		{;
			_dbContext.Setup(i => i.GetUserResponse(It.IsAny<int>())).Returns((UserResponse)null);

			var result = _controller.Get() as NotFoundResult;

			Assert.NotNull(result);
		}

		[Fact]
		public void GetUser_ValidRequest_OkResponse()
		{
			var result = _controller.Get() as OkNegotiatedContentResult<string>;

			Assert.NotNull(result);
		}
	}
}
