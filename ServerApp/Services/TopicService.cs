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
		//Initialises the connection to the user dataase hosted on AWS DynamoDB
		public TopicService()
		{
			var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("AKIA4MTWG6LQHO536SWH", "GDDcWwvn6BN0iijtf3YGnCP/oH+mZZefZyXFYNuw");
			AmazonDynamoDBClient DynamoClient = new AmazonDynamoDBClient(awsCredentials, Amazon.RegionEndpoint.APSoutheast2);

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
			IEnumerable<TopicModel> topics = DbContext.QueryAsync<TopicModel>(User.userId).GetRemainingAsync().Result;
            List<Topic> allTopics = new List<Topic>();
            foreach (TopicModel topic in topics)
            {
                allTopics.Add(new Topic(topic));
            }
            return allTopics;
		}

        //Uses Query method to retrieve all topics for a specifc user
        public List<Topic>[] GetTasks()
		{
			List<TopicModel> topics = DbContext.QueryAsync<TopicModel>(User.userId).GetRemainingAsync().Result;
			List<Topic> dueTopics = new List<Topic>();
			List<Topic> allTopics = new List<Topic>();
			foreach(TopicModel topic in topics)
			{
				DateTime dueDate = DateTime.ParseExact((topic.dueDate), "yyyy/MM/dd", null);
				if(dueDate <= DateTime.UtcNow.AddHours(10))
				{
					dueTopics.Add(new Topic(topic));
				}
				if(allTopics.Count() > 0)
				{
                    for (int i = 0; i < allTopics.Count(); i++)
                    {
                        if (allTopics[i].dueDate >= dueDate)
                        {
                            allTopics.Insert(i, new Topic(topic));
                            break;
                        }                           
                    }
					if(!allTopics.Contains(new Topic(topic)))
					{
                        allTopics.Add(new Topic(topic));
                    }
                }
				else
					allTopics.Add(new Topic(topic));
			}

			return new List<Topic>[] { dueTopics, allTopics };
		}


		//Deletes topic by id
		public void DeleteTopic(string topicId)
		{
			//Defines what items with specific range value to retrieve
			List<string> rangeValues = new List<string>() { (topicId) };

			//Gets topic from topic database to remove
            TopicModel topic = DbContext.QueryAsync<TopicModel>(User.userId, Amazon.DynamoDBv2.DocumentModel.QueryOperator.Equal, rangeValues).GetRemainingAsync().Result.ToList()[0];
			
			//Removes topic
			DbContext.DeleteAsync<TopicModel>(topic);
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
