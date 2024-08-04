///Controls the user authentication process
///Checks if user exists, if password is correct, and can then sign the user in

namespace ServerApp.Services
{
	public class Auth
	{
		internal  List<UserModel> Users { get; set; }

		//On startup makes call to DB to get all users with the username provided
		public Auth(string username, UserService userService)
		{
			Users = userService.GetUsers((username));
		}
		//Checks if an account exists with the required username
		public bool UserExists()
		{
			return Users.Count() > 0;
		}

		//Checks if user with provided password exists
		public bool validLogin(string password)
		{
			//If user doesnt exist, foreach will not run
			foreach(UserModel user in Users)
			{
				//If user password is correct, then return true
				//Otherwise continues until all accounts have been checked
				if (user.password == (password))
				{
					return true;
				}
			}
			return false;
		}

		//Assigns User object's information to correct account
		public void verfyUser(string password)
		{
			User.Generate(Users.Where(x => x.password == (password)).FirstOrDefault());
		}
	}
}
