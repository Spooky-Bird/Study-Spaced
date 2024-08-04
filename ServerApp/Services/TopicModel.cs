using Amazon.DynamoDBv2.DataModel;

namespace ServerApp.Services
{
    //Defines which table object should be entered in
    [DynamoDBTable("TaskTable")]
    public class TopicModel
    {
        //Primary key, can be used to easily load objects
        [DynamoDBHashKey]
        public string userId { get; set; }
        [DynamoDBRangeKey]
        public string topicId { get; set; }
        [DynamoDBProperty]
        public string topicName { get; set; }
        [DynamoDBProperty]
        public string dueDate { get; set; }
        [DynamoDBProperty]
        public string subjectName { get; set; }
        [DynamoDBProperty]
        public string repetitions { get; set; }
        [DynamoDBProperty]
        public string description { get; set; }
        [DynamoDBProperty]
        public string difficulty { get; set; }
        [DynamoDBProperty]
        public string links { get; set; }

        [DynamoDBProperty]
        public string files { get; set; }
    }
}
