using System.Web.Http;
using Orator.Models.Chats;
using Orator.Models.Messages;
using System.Linq;

namespace Orator.Controllers.V1
{
	[Authorize]
	[RoutePrefix("v1/chat")]
	public class ChatController : BaseController
	{
		[HttpGet]
		[Route("{id:int}")]
		public IHttpActionResult Get(int id)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == id)) return BadRequest();
			
			var currentModel = DBContext.GetChat(id);
			if (currentModel == null) return NotFound();

			return Ok(ToJsonString(currentModel));
		}


		[HttpGet]
		[Route("{chatId:int}/messages")]
		public IHttpActionResult GetMessages(int chatId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == chatId)) return BadRequest();
			
			var currentModel = DBContext.GetChatMessages(chatId);
			if (currentModel == null) return NotFound();

			return Ok(ToJsonString(currentModel));
		}

		[HttpGet]
		[Route("{chatId:int}/users")]
		public IHttpActionResult GetUsers(int chatId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == chatId)) return BadRequest();
			
			var currentModel = DBContext.GetChatUsers(chatId);
			if (currentModel == null) return NotFound();

			return Ok(ToJsonString(currentModel));
		}

		[HttpPost]
		[Route("")]
		public IHttpActionResult Create(CreateChatRequest request)
		{
			if (!IsUserAuthenticated) return Unauthorized();

			Chat chat = new Chat { AllowHtml = request.AllowHtml, Title = request.Title, };

			chat.UpdatedAt = System.DateTime.UtcNow;
			chat.UpdatedBy = CurrentUserId;
			chat.CreatedAt = System.DateTime.UtcNow;
			chat.CreatedBy = CurrentUserId;
				
			return Ok(ToJsonString(DBContext.CreateChat(chat)));
		}
		
		[HttpPost]
		[Route("{chatId:int}/invite/{userId:int}")]
		public IHttpActionResult InviteToChat(int chatId, int userId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			var currentConnections = DBContext.GetChatConnectionsByUserId(userId);
			if (!chatConnections.Any(i => i.ChatId == chatId)) return BadRequest();
			if (currentConnections.Any(i => i.ChatId == chatId)) return BadRequest();

			ChatConnection chatConnection = new ChatConnection { ChatId = chatId, Status = ConnectionStatus.Invited, UserId = userId };

			chatConnection.UpdatedAt = System.DateTime.UtcNow;
			chatConnection.UpdatedBy = CurrentUserId;
			chatConnection.CreatedAt = System.DateTime.UtcNow;
			chatConnection.CreatedBy = CurrentUserId;

			return Ok(ToJsonString(DBContext.CreateChatRequest(chatConnection)));
		}

		[HttpPost]
		[Route("{chatId:int}/deny")]
		public IHttpActionResult DenyChatConnection(int chatId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == chatId && i.Status == ConnectionStatus.Invited)) return BadRequest();

			DBContext.DenyChatRequest(chatId, CurrentUserId);

			return Ok();
		}

		[HttpPost]
		[Route("{chatId:int}/accept")]
		public IHttpActionResult AcceptChatConnection(int chatId)
		{
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == chatId && i.Status == ConnectionStatus.Invited)) return BadRequest();

			DBContext.ApproveChatRequest(chatId, CurrentUserId);

			return Ok();
		}
			
		[HttpPost]
		[Route("{chatId:int}/new_message")]
		public IHttpActionResult Create(int chatId, CreateMessageRequest request)
		{ 
			if (!IsUserAuthenticated) return Unauthorized();
			var chatConnections = DBContext.GetChatConnectionsByUserId(CurrentUserId);
			if (!chatConnections.Any(i => i.ChatId == request.ChatId && i.Status == ConnectionStatus.Accepted) || request.ChatId != chatId) return BadRequest();

			Message message = new Message { ChatId = request.ChatId, UserId = request.UserId, Content = request.Content };

			message.UpdatedAt = System.DateTime.UtcNow;
			message.UpdatedBy = CurrentUserId;
			message.CreatedAt = System.DateTime.UtcNow;
			message.CreatedBy = CurrentUserId;

			return Ok(ToJsonString(DBContext.CreateMessage(message)));
		}
	}
}