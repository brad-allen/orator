using Orator.Models.Users;
using System.Web.Http;

namespace Orator.Controllers.V1
{
	[Authorize]
	[RoutePrefix("v1/user")]
	public class UserController : BaseController
	{
		
		[HttpGet]
		[Route("")]
		public IHttpActionResult Get()
		{
			if (!IsUserAuthenticated) return Unauthorized();

			var currentModel = DBContext.GetUserResponse(CurrentUserId);

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
			if(!ModelState.IsValid) return BadRequest();
			
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
		[Route("chats")]
		public IHttpActionResult UserChats()
		{
			if (!IsUserAuthenticated) return Unauthorized();

			return Ok(ToJsonString(DBContext.GetUserChats(CurrentUserId)));
		}

		[HttpGet]
		[Route("chat_requests")]
		public IHttpActionResult UserChatRequests()
		{
			if (!IsUserAuthenticated) return Unauthorized();

			return Ok(ToJsonString(DBContext.GetChatRequests(CurrentUserId)));
		}

	}
}