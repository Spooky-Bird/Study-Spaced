
using Microsoft.AspNetCore.Components;
using ServerApp.Pages;

///Controls task behaviour including adding, removing, and updating 
namespace ServerApp.Services
{
    public class TaskController
    {
        //Dictates whether to show the task entry popup
        public static bool displayTaskEntry = false;

        //Dictates whether to show the task histery popup
        public static bool displayTaskHistory = false;

        //Dictates whether to show the task edit popup
        public static bool displayTaskEdit = false;

        public static Topic currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>());


        public static string name = "";
        public static int difficulty = 0;
        public static Subject selectedSubject = new Subject("NULL", "NULL");
        public static string description = "";
        public static List<Resource> taskResources = new List<Resource>();
        public static Dictionary<string, string> files = new Dictionary<string, string>();
        public static List<string> links = new List<string>();
        public static string link = "";

        public TaskController() { }

        //Opens and closes the task entry popup
        public static void toggleTask()
        {
            displayTaskEntry = !displayTaskEntry;
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>());
        }

        //Opens and closes the task history popup
        public static void toggleHistory()
        {
            displayTaskHistory = !displayTaskHistory;
        }

        //Opens the task edit popup
        public static void toggleEdit(Topic topic = null)
        {
            displayTaskEdit = !displayTaskEdit;
            if (topic != null)
            {
                currentTask = topic;
                selectedSubject = topic.subject;
                difficulty = topic.difficulty;
                links = topic.links;
                name = topic.name;
                description = topic.description;
            }
        }

        public static async Task confirmEdit()
        {
            Topic topic = User.allTopics.Where(t => t.id == currentTask.id).First();
            topic = currentTask;
            topic.subject = selectedSubject;

            topic.difficultyIndicator = "";
            for (int i = 0; i < difficulty; i++)
            {
                topic.difficultyIndicator += "!";
            }

            if (topic.dueDate <= DateTime.UtcNow.AddHours(10))
            {
                int index = User.topics.FindIndex(t => t.id == currentTask.id);
                User.topics[index] = currentTask;
                User.topics[index].subject = selectedSubject;
            }

            FileService fileService = new FileService();
            foreach (KeyValuePair<string, string> file in files)
            {
                fileService.saveFile($"wwwroot/TempFiles/{User.userId}/{file.Key}", file.Key, file.Value);
                string url = fileService.getUrl($@"{User.userId}/{file.Key}", "resource-storage-study-spaced").Result;
                topic.files.Add(file.Key, url);
            }

            TopicService topicService = new TopicService();
            topicService.StoreTopic(topic.Model(topic.dueDate.ToString("yyyy/MM/dd")));
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>());
        }

        //Takes all inputs from the ui and saves it to a topic object
        public static void taskSubmitted()
        {
            Topic topic = new Topic(currentTask.name, selectedSubject, 0, currentTask.difficulty, currentTask.description, currentTask.files.Select(x => x.Key).ToList(), currentTask.links);
            TopicService topicService = new TopicService();
            string dueDate = DateTime.UtcNow.AddDays(calculateInterval(topic.repetitions, topic.difficulty)).AddHours(10).ToString("yyyy/MM/dd");
            topicService.StoreTopic(topic.Model(dueDate));
            topic.dueDate = DateTime.UtcNow.AddDays(calculateInterval(topic.repetitions, topic.difficulty)).AddHours(10);
            User.allTopics.Add(topic);

            FileService fileService = new FileService();
            foreach(KeyValuePair<string, string> file in files)
            {
                fileService.saveFile($"wwwroot/TempFiles/{User.userId}/{file.Key}", file.Key, file.Value);
            }
            try
            {
                Directory.Delete(@$"wwwroot/TempFiles/{User.userId}/", true);
            }
            catch { }
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>());

        }

        public static void taskCompleted(Topic topic)
        {
            TopicService topicService = new TopicService();
            topic.repetitions++;
            string dueDate = DateTime.Now.AddDays(calculateInterval(topic.repetitions, topic.difficulty)).ToString("yyyy/MM/dd");
            topicService.StoreTopic(topic.Model(dueDate));
        }

        //Removes topic from topic list
        public static void removeTask(Topic topic)
        {
            User.topics.Remove(topic);
            User.allTopics.Remove(topic);
            TopicService topicService = new TopicService();
            topicService.DeleteTopic(topic.id);
        }

        public static void updateTask(Topic task)
        {
            TopicService topicService = new TopicService();
            topicService.DeleteTopic(task.id);
            topicService.StoreTopic(task.Model());
        }

        public static int calculateInterval(int repetitions, int difficulty)
        {
            Random rand = new Random();
            repetitions /= difficulty+1;
            if (repetitions >= User.intervals.Count())
                return Math.Abs(User.intervals[^1].delay + rand.Next(-User.deviation, 1 + User.deviation));
            if(repetitions > 1)
                return Math.Abs(User.intervals[repetitions].delay + rand.Next(-User.deviation, 1 + User.deviation));
            return User.intervals[repetitions].delay;

        }

        public static void cancel()
        {
            currentTask.name = name;
            currentTask.description = description;
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>());
            try
            {
                Directory.Delete(@$"wwwroot/TempFiles/{User.userId}/", true);
            }
            catch { }

            displayTaskEdit = false;
            displayTaskEntry = false;
        }


        public static void addLink(bool isEditing)
        {
            if (isEditing)
                currentTask.links.Insert(0, link);
            else
                currentTask.links.Insert(0,link);
            link = "";
        }
    }
}
