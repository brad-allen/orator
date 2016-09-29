using Orator.Models.Users;
using System.Web.Http;
using MemoryCache;
using MemoryCache.Tests.TestObjects;

namespace Orator.Controllers.V1
{
	[Authorize]
	[RoutePrefix("v1/user")]
	public class UserController : BaseController
	{
		
		[AllowAnonymous]
		[HttpGet]
		[Route("cache")]
		public IHttpActionResult TestCache()
		{
			CacheSettings settings = new CacheSettings();
			settings.BlocksPerSet = 4;
			settings.NumberOfSets = 2;
			settings.ReplacementAlgorithm = ReplacementAlgorithm.LRU;
			settings.CacheDefaultExpirationMinutes = 1;


			Orator.MvcApplication.TestCache.UpdateCacheSettings(settings);

			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 1, Test2 = "Test1" }, new TestValue { Test1 = 1, Test2 = "Test10" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 2, Test2 = "Test2" }, new TestValue { Test1 = 2, Test2 = "Test20" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 3, Test2 = "Test3" }, new TestValue { Test1 = 3, Test2 = "Test30" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 4, Test2 = "Test4" }, new TestValue { Test1 = 4, Test2 = "Test40" });

			TestValue outValue = new TestValue();
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 2, Test2 = "Test2" }, ref outValue);

			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 5, Test2 = "Test5" }, new TestValue { Test1 = 1, Test2 = "Test50" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 6, Test2 = "Test6" }, new TestValue { Test1 = 2, Test2 = "Test60" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 7, Test2 = "Test7" }, new TestValue { Test1 = 3, Test2 = "Test70" });

			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 5, Test2 = "Test5" }, new TestValue { Test1 = 1, Test2 = "Test50000000" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 6, Test2 = "Test6" }, new TestValue { Test1 = 2, Test2 = "Test62222220" });

			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 6, Test2 = "Test16" }, new TestValue { Test1 = 2, Test2 = "Test62222220" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 6, Test2 = "Test17" }, new TestValue { Test1 = 2, Test2 = "Test62222220" });

			outValue = new TestValue();
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 7, Test2 = "Test7" }, ref outValue);

			outValue = Orator.MvcApplication.TestCache.Find(new TestKey { Test1 = 7, Test2 = "Test7" });

			Orator.MvcApplication.TestCache.SweepItem(new TestKey { Test1 = 7, Test2 = "Test7" });

			outValue = new TestValue();
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 7, Test2 = "Test7" }, ref outValue);
			outValue = Orator.MvcApplication.TestCache.Find(new TestKey { Test1 = 7, Test2 = "Test7" });



			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 8, Test2 = "Test8" }, new TestValue { Test1 = 4, Test2 = "Test80" });

			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 9, Test2 = "Test9" }, new TestValue { Test1 = 9, Test2 = "Test5000" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 10, Test2 = "Test10" }, new TestValue { Test1 = 20, Test2 = "Test60000" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 11, Test2 = "Test11" }, new TestValue { Test1 = 31, Test2 = "Test7000" });
			Orator.MvcApplication.TestCache.Set(new TestKey { Test1 = 12, Test2 = "Test12" }, new TestValue { Test1 = 42, Test2 = "Test80000" });


			outValue = null;
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 2, Test2 = "Test2" }, ref outValue);

			outValue = null;
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 1, Test2 = "Test1" }, ref outValue);

			outValue = null;
			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 2, Test2 = "Te" }, ref outValue);


			Orator.MvcApplication.TestCache.TryFind(new TestKey { Test1 = 12, Test2 = "Test12" }, ref outValue);


			/*Orator.MvcApplication.TestCache.Set("2", "1456");
			Orator.MvcApplication.TestCache.Set("4", "234");
			Orator.MvcApplication.TestCache.Set("BBB", "385");
			Orator.MvcApplication.TestCache.Set("6A", "1234567");
			

			string outValue = "";
			Orator.MvcApplication.TestCache.TryFind("BBB", out outValue);
			*/


			return Ok();
		}


		[HttpGet]
		[Route("")]
		public IHttpActionResult Get()
		{
			if (!HasValidCookie) return Unauthorized();

			var currentModel = DBContext.GetUserResponse(CurrentUserId);

			if (currentModel == null) return NotFound();
			
			return Ok(ToJsonString(currentModel));
		}
		
		[HttpPost]
		[HttpPut]
		[Route("{id:int}")]
		public IHttpActionResult Update(int id, UpdateUserRequest request)
		{
			if (!HasValidCookie) return Unauthorized();
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
			if (!HasValidCookie) return Unauthorized();

			return Ok(ToJsonString(DBContext.GetUserChats(CurrentUserId)));
		}

		[HttpGet]
		[Route("chat_requests")]
		public IHttpActionResult UserChatRequests()
		{
			if (!HasValidCookie) return Unauthorized();

			return Ok(ToJsonString(DBContext.GetChatRequests(CurrentUserId)));
		}

	}
}