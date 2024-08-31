///**********************************************************************
///Project: ServerApp
///
///Page: Home
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///API interface layer with DynamoDB topics table
///Includes basic CRUD functionality
///Includes option to get only due tasks
///**********************************************************************
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System.Text;

//User database client
//Can store and delete user
//Can get user from username or id

namespace ServerApp.Services
{
	public class TopicService
	{
		public readonly DynamoDBContext DbContext;
        public AmazonDynamoDBClient DynamoClient;
		public User _currentUser;
		//Initialises the connection to the user dataase hosted on AWS DynamoDB
		public TopicService(User user)
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

		//Stores a given topic to the topic database
		public void StoreTopic(TopicModel model)
		{
			DbContext.SaveAsync(model);
		}

		//Uses Query method to retrieve all topics for a specifc user
		public List<Topic> GetAllTasks()
		{
			IEnumerable<TopicModel> topics = DbContext.QueryAsync<TopicModel>(_currentUser.userId).GetRemainingAsync().Result;
            List<Topic> allTopics = new List<Topic>();
            foreach (TopicModel topic in topics)
            {
                allTopics.Add(new Topic(topic, _currentUser));
            }
            return allTopics;
		}

        //Uses Query method to retrieve all topics for a specifc user
        public List<Topic>[] GetTasks()
		{
			//Get all topics
			List<TopicModel> topics = new();

			try {
				topics = DbContext.QueryAsync<TopicModel>(_currentUser.userId).GetRemainingAsync().Result;
			} catch {
				return new List<Topic>[] { new() { } };
				};


			List<Topic> dueTopics = new List<Topic>();
			List<Topic> allTopics = new List<Topic>();

			foreach(TopicModel topic in topics)
			{
				//converts date string to datetime
				DateTime dueDate = DateTime.ParseExact((topic.dueDate), "yyyy/MM/dd", null);

				//If the topic is due, add to dueTopics list
				if(dueDate <= DateTime.UtcNow.AddHours(10))
				{
					dueTopics.Add(new Topic(topic, _currentUser));
				}

				if(allTopics.Count() > 0)
				{
					//Places topic into topic list in chronological order
                    for (int i = 0; i < allTopics.Count(); i++)
                    {
                        if (allTopics[i].dueDate >= dueDate)
                        {
                            allTopics.Insert(i, new Topic(topic, _currentUser));
                            break;
                        }                           
                    }
					//If topic is the latest chronologically, it is entered into the list here
					if(!allTopics.Contains(new Topic(topic, _currentUser)))
					{
                        allTopics.Add(new Topic(topic, _currentUser));
                    }
                }
				else
					allTopics.Add(new Topic(topic, _currentUser));
			}

			//Returns both all topics and the due topics as seperate lists
			return new List<Topic>[] { dueTopics, allTopics };
		}


		//Deletes topic by id
		public void DeleteTopic(string topicId)
		{
			//Defines what items with specific range value to retrieve
			List<string> rangeValues = new List<string>() { (topicId) };
			try
			{
				//Gets topic from topic database to remove
				TopicModel topic = DbContext.QueryAsync<TopicModel>(_currentUser.userId, Amazon.DynamoDBv2.DocumentModel.QueryOperator.Equal, rangeValues).GetRemainingAsync().Result.ToList()[0];
			
				//Removes topic
				DbContext.DeleteAsync<TopicModel>(topic);
			}catch { }
		}


		//Deletes all topics from a users account
		public void clearTopics()
		{
			try
			{
				//Gets all topics
				List<TopicModel> topics = DbContext.QueryAsync<TopicModel>(_currentUser.userId).GetRemainingAsync().Result;
				foreach(TopicModel topic in topics)
				{
					DeleteTopic(topic.topicId);
				}
			}
			catch { }
		}

		public async Task fullBackup()
		{
			var conditions = new List<ScanCondition>();
			List<TopicModel> table = await DbContext.ScanAsync<TopicModel>(conditions).GetRemainingAsync();
            var csv = new StringBuilder();
			csv.AppendLine("UserId, TopicId, Name, Description, Subject, Due Date, Repetitions, Difficulty, Files, Links");
            foreach (TopicModel topic in table)
			{
				csv.AppendLine($"{topic.userId}, {topic.topicId}, {topic.topicName}, {topic.description}, {topic.subjectName}, {topic.dueDate}, {topic.repetitions}, {topic.difficulty}, {topic.files}, {topic.links}");
			}
            File.WriteAllText(@$"wwwroot\Backups\Topics\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}.csv", csv.ToString());
        }
	}
}
