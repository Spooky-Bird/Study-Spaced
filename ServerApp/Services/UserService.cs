using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System.Text;

//User database client
//Can store and delete user
//Can get user from username or id

namespace ServerApp.Services
{
	public class UserService
	{
		public readonly DynamoDBContext DbContext;
        public AmazonDynamoDBClient DynamoClient;
		//Initialises the connection to the user dataase hosted on AWS DynamoDB

		User _currentUser;


        public UserService(User user)
		{
			var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(Environment.GetEnvironmentVariable("Access"), Environment.GetEnvironmentVariable("Secret"));
			AmazonDynamoDBClient DynamoClient = new AmazonDynamoDBClient(awsCredentials, Amazon.RegionEndpoint.APSoutheast2);
			_currentUser = user;
			DbContext = new DynamoDBContext(DynamoClient, new DynamoDBContextConfig
			{
				//Setting the Consistent property to true ensures that you'll always get the latest
				ConsistentRead = true,
				SkipVersionCheck = true
			});
		}

		//Stores a given userModel to user database
		public void StoreUser(UserModel model)
		{
			DbContext.SaveAsync(model);
		}

		//Uses Scan method to retrieve all accounts with a given username in a list of UserModels
		public List<UserModel> GetUsers(string username)
		{
			//Defines logic to get all items with property "username" that equals {username}
			ScanCondition SC = new ScanCondition("username", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, username);

			//Performs api call
			var users = DbContext.ScanAsync<UserModel>(new List<ScanCondition> { SC }).GetRemainingAsync().Result;
			return users;
		}

		//Gets user using user id using Load method, applies info to User object
		public void LoadUser(string userId)
		{
			//As userId is the database's primary key, we are able to use Load to retrieve the desired UserModel
			UserModel uModel = DbContext.LoadAsync<UserModel>(userId).Result;

            //Transfers info into User object
            _currentUser.Generate(uModel);
		}

		//Deletes user by id
		public void DeleteUser()
		{
			DbContext.DeleteAsync<UserModel>(_currentUser.userId);
		}
        public async Task fullBackup()
        {
            var conditions = new List<ScanCondition>();
            List<UserModel> table = await DbContext.ScanAsync<UserModel>(conditions).GetRemainingAsync();
            var csv = new StringBuilder();
            csv.AppendLine("UserId, Name, Email, Delays, Intervals, Deviation, Subject Names, Subject Colours");
            foreach (UserModel user in table)
            {
                csv.AppendLine($"{user.userId}, {user.username}, {user.password}, {user.email}, {user.delays}, {user.intervals}, {user.deviation}, {user.subjectNames}, {user.subjectColours}");
            }
            File.WriteAllText(@$"wwwroot\Backups\Users\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}.csv", csv.ToString());
        }
    }
}
