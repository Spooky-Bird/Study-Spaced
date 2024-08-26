///**********************************************************************
///Project: ServerApp
///
///Page: Login, Settings, Profile
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///Object to be stored in amazon database
///**********************************************************************
using Amazon.DynamoDBv2.DataModel;

namespace ServerApp
{
	//Defines which table object should be entered in
	[DynamoDBTable("UserTable")]
	public class UserModel
	{
		//Primary key, can be used to easily load objects
		[DynamoDBHashKey]
		public string userId { get; set; }
		[DynamoDBProperty]
		public string username { get; set; }
		[DynamoDBProperty]
		public string password { get; set; }
        [DynamoDBProperty]
        public string deviation { get; set; }
        [DynamoDBProperty]
		public string delays { get; set; }
		[DynamoDBProperty]
		public string intervals { get; set; }
		[DynamoDBProperty]
		public string email = "";
		[DynamoDBProperty]
		public string subjectNames = "";
		[DynamoDBProperty]
		public string subjectColours = "";


    }
}