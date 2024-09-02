using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

namespace ServerApp.Services
{
	public abstract class AWS : IModel
	{
		public readonly DynamoDBContext DbContext;
		public AmazonDynamoDBClient DynamoClient;
		//Initialises the connection to the user database hosted on AWS DynamoDB

		public User _currentUser;
		public AWS(User user)
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
	}
}
