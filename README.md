# orator
## Running the app 
	You'll need to setup and connect a SQL Database in order to run the application.  
	In SQL Server, you may need to create an 'Orator' database (depending on how you connect) but EF will create the rest of the tables on first use.
	Other than that, you should be able to rebuild the app and run it as is.

## Using the app

	Register an account or sign in.
	
	Once you have logged in through the MVC portion of the application, you are authorized to access the API routes
	provided in order to create chats and messages, invite people to chats, accept and deny chat requests, view chats you are a part of,
	and view users and messages in chats.
	
	Uses JSON for data transfer - POST request body eg:
	
	{
	 "Username":"NewUserName",
	 "FirstName:"Brad",
	 "LastName:"Allen",
	 "Bio:"I like this and that..."
	}
	
## API User Endpoints:

	GET http://localhost:53437/v1/user
	Returns basic information of the logged in user
	

	POST or PUT http://localhost:53437/v1/user
	Update the user profile information...
	Body Fields: Username, FirstName, LastName, Bio
	

	GET http://localhost:53437/v1/user/chats
	Gets the chats of the current user
	

	GET http://localhost:53437/v1/user/chat_requests
	Gets the chats requests of the current user
	

## API Chat Endpoints:
	
	GET http://localhost:53437/v1/chat/{chatId}
	Get the chat information - no messages or users
	

	GET http://localhost:53437/v1/chat/{chatId}/messages
	Get the chat messsages
	

	GET http://localhost:53437/v1/chat/{chatId}/users
	Get the users in the specified chat
	

	POST http://localhost:53437/v1/chat
	Create a new chat
	Body Fields: Title, AllowHtml
	

	POST http://localhost:53437/v1/chat/{chatId}/invite/:{userId}
	Invite someone to the chat
	

	POST or PUT  http://localhost:53437/v1/chat/{chatId}/deny
	Deny the chat invite for the current user
	

	POST or PUT http://localhost:53437/v1/chat/{chatId}/accept
	Accept the chat invite for the current user
	

	POST http://localhost:53437/v1/chat/{chatId}/new_message
	Create a message in the acccepted chat
	Body Fields: ChatId, UserId, Content
	
