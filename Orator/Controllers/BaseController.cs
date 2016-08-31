using System.Web.Http;
using System.Web.Script.Serialization;
using Orator.DB;
using System.Linq;
using System;
using System.Net.Http;
using System.Web;

namespace Orator.Controllers
{
	public class BaseController : ApiController
	{
		public virtual DatabaseContext DBContext { get; set; }

		protected string CurrentEmail { get; private set; }
		protected int CurrentUserId { get; private set; }
		protected string CurrentUsername { get; private set; }

		public virtual bool IsUserAuthenticated => CheckCurrentUserCookie() && User.Identity.IsAuthenticated;
		public string AuthName => User.Identity.Name;
		
		public BaseController()
		{
			DBContext = new DatabaseContext();//TODO add IoC layer
		}
		
		public string ToJsonString(object currentModel)
		{
			return new JavaScriptSerializer().Serialize(currentModel);
		}

		public bool CheckCurrentUserCookie()
		{
			var cookies = Request.Headers.GetCookies();
			
			foreach (var cookie in cookies?.First()?.Cookies)
			{
				if(cookie.Name == "orator_user")
				{
					//TODO expiration and refresh of cookie

					int userId = 0;
					foreach(var key in cookie.Values.AllKeys)
					{
						CurrentUsername = (key == "username" ? cookie.Values[key] : CurrentUsername);
						CurrentUserId = (key == "userid" ? (int.TryParse(cookie.Values[key], out userId) ? userId : 0) : CurrentUserId);
						CurrentEmail = (key == "email" ? cookie.Values[key] : CurrentEmail);
					}
					return true;
				}
			}

			//else the cookie was not found
			if (AuthName != null)
			{
				var user = DBContext.GetUserByAuthId(AuthName);
				if(user != null)
				{
					CreateUserCookie(user.Id, user.Username, user.Email);
					CurrentUsername = user.Username;
					CurrentUserId = user.Id;
					CurrentEmail = user.Email;
					return true;
				}
				
			}
			return false;
		}

		private void CreateUserCookie(int userId, string username, string email)
		{
			//TODO encrypt
			HttpCookie userCookie = new HttpCookie("orator_user");
			userCookie["userid"] = userId.ToString();
			userCookie["username"] = username;
			userCookie["email"] = email; //TODO for now email is also username
			userCookie.Expires = DateTime.Now.AddDays(1);
			HttpContext.Current.Response.Cookies.Add(userCookie);
		}

	}
}
 