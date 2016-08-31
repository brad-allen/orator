using Orator.Models.Users;
using System.Web.Http;

namespace Orator.Controllers.V1
{
	[Authorize]
	[RoutePrefix("v1/user")]
	public class UserController : BaseController
	{
		
		[HttpGet]
		[Route("{id:int}")]
		public IHttpActionResult Get(int id)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			if (CurrentUserId != id) return BadRequest();


			var currentModel = DBContext.GetUserResponse(id);

			if (currentModel == null) return NotFound();
			
			return Ok(ToJsonString(currentModel));
		}
		
		[HttpPost]
		[HttpPut]
		[Route("{id:int}")]
		public IHttpActionResult Update(int id, UpdateUserRequest request)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			if (CurrentUserId != id) return BadRequest();

			User currentModel = DBContext.GetFullUser(id);

			if (currentModel == null) return NotFound();

			currentModel.Bio = request.Bio;
			currentModel.FirstName = request.FirstName;
			currentModel.LastName = request.LastName;
			currentModel.Username = request.Username;
			currentModel.UpdatedAt = System.DateTime.UtcNow;
			currentModel.UpdatedBy = CurrentUserId;

			return Ok(ToJsonString(DBContext.UpdateUser(currentModel)));
		}

		[HttpGet]
		[Route("{userId:int}/chats")]
		public IHttpActionResult UserChats(int userId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			if (CurrentUserId != userId) return BadRequest();

			return Ok(ToJsonString(DBContext.GetUserChats(userId)));
		}

		[HttpGet]
		[Route("{userId:int}/chat_requests")]
		public IHttpActionResult UserChatRequests(int userId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			if (CurrentUserId != userId) return BadRequest();

			return Ok(ToJsonString(DBContext.GetChatRequests(userId)));
		}

	}
}