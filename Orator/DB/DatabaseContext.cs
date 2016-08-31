using System.Collections.Generic;
using System.Data.Entity;
using Orator.Models.Chats;
using Orator.Models.Users;
using Orator.Models.Messages;
using System.Linq;
using System.Configuration;

namespace Orator.DB
{
	public class DatabaseContext :DbContext
	{
		public DatabaseContext() : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString) {}

		public DbSet<Chat> Chats { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<ChatConnection> ChatConnections { get; set; }

		public virtual UserResponse GetUserResponse(int id)
		{
			using (var db = new DatabaseContext())
			{
				return db.Users.Where(i => i.Id == id).AsEnumerable().Select(p => GetUserResponse(p)).SingleOrDefault();
			}
		}

		public User GetFullUser(int id)
		{
			using (var db = new DatabaseContext())
			{
				return db.Users.Where(i => i.Id == id).SingleOrDefault();
			}
		}

		public User GetUserByAuthId(string authId)
		{
			using (var db = new DatabaseContext())
			{
				return db.Users.Where(i => i.AuthId == authId).SingleOrDefault();
			}
		}


		public User GetUserByEmail(string email)
		{
			using (var db = new DatabaseContext())
			{
				return db.Users.Where(i => i.Email.ToLower() == email.ToLower()).SingleOrDefault();
			}
		}
		

		public UserResponse UpdateUser(User user)
		{
			using (var db = new DatabaseContext())
			{
				var updatedRow = db.Users.Where(i => i.Id == user.Id).First();
				db.Entry(updatedRow).CurrentValues.SetValues(user);
				db.SaveChanges();
			}

			return GetUserResponse(user);
		}

		public UserResponse CreateUser(User user)
		{
			using (var db = new DatabaseContext())
			{
				db.Users.Add(user);
				db.SaveChanges();
			}

			return GetUserResponse(user);
		}

		public List<UserResponse> AllUsers()
		{
			using (var db = new DatabaseContext())
			{
				return db.Users.AsEnumerable().Select(p => GetUserResponse(p)).ToList();
			}
		}

		public List<ChatResponse> GetUserChats(int userId)
		{
			using (var db = new DatabaseContext())
			{
				var approvedChats = db.ChatConnections.Where(i => i.UserId == userId && i.Status == ConnectionStatus.Accepted).ToList();

				List<int> approvedChatList = new List<int>();
				foreach (var chat in approvedChats)
				{
					approvedChatList.Add(chat.ChatId);
				}
				
				return db.Chats.Where(x => approvedChatList.Contains(x.Id)).OrderBy(i => i.Id).AsEnumerable().Select(p => GetChatResponse(p)).ToList();
			}
		}

		public ChatResponse GetChat(int chatId)
		{
			using (var db = new DatabaseContext())
			{
				return db.Chats.Where(i => i.Id == chatId).AsEnumerable().Select(p => GetChatResponse(p)).SingleOrDefault();
			}
		}

		public List<MessageResponse> GetChatMessages(int chatId)
		{
			using (var db = new DatabaseContext())
			{
				return db.Messages.Where(i => i.ChatId == chatId).OrderBy(i => i.Id).AsEnumerable().Select(p => GetMessageResponse(p)).ToList();
			}
		}
		
		public List<UserResponse> GetChatUsers(int chatId)
		{
			using (var db = new DatabaseContext())
			{
				var connections = db.ChatConnections.Where(i => i.ChatId == chatId);
				List<int> connectionList = new List<int>();
				foreach(var connection in connections)
				{
					connectionList.Add(connection.Id);
				}

				return db.Users.Where(x => connectionList.Contains(x.Id)).OrderBy(i => i.Id).AsEnumerable().Select(p => GetUserResponse(p)).ToList();
			}
		}

		public MessageResponse CreateMessage(Message message)
		{
			using (var db = new DatabaseContext())
			{
				db.Messages.Add(message);
				db.SaveChanges();
			}

			return GetMessageResponse(message);
		}

		public ChatResponse CreateChat(Chat chat)
		{
			using (var db = new DatabaseContext())
			{
				db.Chats.Add(chat);
				db.SaveChanges();
				CreateChatRequest(new ChatConnection { ChatId = chat.Id, Status = ConnectionStatus.Accepted, UserId = chat.CreatedBy, CreatedBy = chat.CreatedBy, CreatedAt = chat.CreatedAt, UpdatedAt = chat.UpdatedAt, UpdatedBy = chat.UpdatedBy });
				db.SaveChanges();
			}

			return GetChatResponse(chat);
		}

		public List<ChatConnectionResponse> GetChatRequests(int userId)
		{
			using (var db = new DatabaseContext())
			{
				return db.ChatConnections.Where(i => i.UserId == userId && i.Status == ConnectionStatus.Invited).AsEnumerable().Select(p => GetChatConnectionsResponse(p)).ToList();
			}
		}

		public List<ChatConnectionResponse> GetChatConnectionsByUserId(int userId)
		{
			using (var db = new DatabaseContext())
			{
				return db.ChatConnections.Where(i => i.UserId == userId).AsEnumerable().Select(p => GetChatConnectionsResponse(p)).ToList();
			}
		}

		public ChatConnectionResponse CreateChatRequest(ChatConnection chatConnection)
		{
			using (var db = new DatabaseContext())
			{
				db.ChatConnections.Add(chatConnection);
				db.SaveChanges();
			}

			return GetChatConnectionsResponse(chatConnection);
		}

		public void ApproveChatRequest(int chatId, int userId)
		{
			using (var db = new DatabaseContext())
			{
				var chatRequest = db.ChatConnections.Where(i => i.UserId == userId && i.Status == ConnectionStatus.Invited && i.ChatId == chatId).SingleOrDefault();
				chatRequest.Status = ConnectionStatus.Accepted;
				db.Entry(chatRequest).CurrentValues.SetValues(chatRequest);
				db.SaveChanges();
			}
		}

		public void DenyChatRequest(int chatId, int userId)
		{
			using (var db = new DatabaseContext())
			{
				var chatRequest = db.ChatConnections.Where(i => i.UserId == userId && i.Status == ConnectionStatus.Invited && i.ChatId == chatId).SingleOrDefault();
				chatRequest.Status = ConnectionStatus.Denied;
				db.Entry(chatRequest).CurrentValues.SetValues(chatRequest);
				db.SaveChanges();
			}
		}

		private static UserResponse GetUserResponse(User user)
		{
			return new UserResponse
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Bio = user.Bio,
				Email = user.Email,
				UpdatedAt = user.UpdatedAt,
				Username = user.Username,
				Id = user.Id,
				CreatedAt = user.CreatedAt
			};

		}

		private static MessageResponse GetMessageResponse(Message message)
		{
			return new MessageResponse
			{
				ChatId = message.ChatId,
				Content = message.Content,
				UserId = message.UserId,
				Id = message.Id,
				CreatedBy = message.CreatedBy,
				CreatedAt = message.CreatedAt,
				UpdatedAt = message.UpdatedAt,
				UpdatedBy = message.UpdatedBy
			};

		}

		private static ChatResponse GetChatResponse(Chat chat)
		{
			return new ChatResponse
			{
				AllowHtml = chat.AllowHtml,
				Title = chat.Title,
				Id = chat.Id,
				CreatedBy = chat.CreatedBy,
				CreatedAt = chat.CreatedAt,
				UpdatedAt = chat.UpdatedAt,
				UpdatedBy = chat.UpdatedBy
			};

		}


		private static ChatConnectionResponse GetChatConnectionsResponse(ChatConnection chatConnection)
		{
			return new ChatConnectionResponse
			{
				ChatId = chatConnection.ChatId,
				Id = chatConnection.Id,
				UserId = chatConnection.UserId,
				Status = chatConnection.Status,
				CreatedBy = chatConnection.CreatedBy,
				CreatedAt = chatConnection.CreatedAt,
				UpdatedAt = chatConnection.UpdatedAt,
				UpdatedBy = chatConnection.UpdatedBy

			};

		}
	}
}