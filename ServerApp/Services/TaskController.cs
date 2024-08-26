///**********************************************************************
///Project: ServerApp
///
///Page: Home
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///Controls topic interaction functionality
///Including ui control, editing and submitting topics, and displaying information
///**********************************************************************
using Microsoft.AspNetCore.Components;
using ServerApp.Pages;

///Controls task behaviour including adding, removing, and updating 
namespace ServerApp.Services
{
    public class TaskController
    {
        //Dictates whether to show the task entry popup
        public bool displayTaskEntry = false;

        //Dictates whether to show the task histery popup
        public bool displayTaskHistory = false;

        //Dictates whether to show the task edit popup
        public bool displayTaskEdit = false;

        public Topic currentTask = new Topic();


        public string name = "";
        public int difficulty = 0;
        public Subject selectedSubject = new Subject("NULL", "NULL");
        public string description = "";
        public List<Resource> taskResources = new List<Resource>();
        public Dictionary<string, string> files = new Dictionary<string, string>();
        public List<string> links = new List<string>();
        public string link = "";

        public User _currentUser;

        public TaskController(User user)
        {
            _currentUser = user;
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>(), _currentUser);
        }

        //Opens and closes the task entry popup
        public void toggleTask()
        {
            displayTaskEntry = !displayTaskEntry;
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>(), _currentUser);
        }

        //Opens and closes the task history popup
        public void toggleHistory()
        {
            displayTaskHistory = !displayTaskHistory;
        }

        //Opens the task edit popup
        public void toggleEdit(Topic topic = null)
        {
            //toggles display bool
            displayTaskEdit = !displayTaskEdit;

            //if opening a topic, assign values to current task + variables
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

        //When confirm clicked on edit page
        //Updates info in database
        public async Task confirmEdit()
        {
            //gets topic by id
            Topic topic = _currentUser.allTopics.Where(t => t.id == currentTask.id).First();

            //overwrites topics variable 
            topic = currentTask;

            //sets seleted subject from dropdown
            topic.subject = selectedSubject;

            //sets difficulty indicator to have as many !s as in difficulty
            topic.difficultyIndicator = "";
            for (int i = 0; i < difficulty; i++)
            {
                topic.difficultyIndicator += "!";
            }

            //Updates topic not in due topic list
            if (topic.dueDate <= DateTime.UtcNow.AddHours(10))
            {
                int index = _currentUser.topics.FindIndex(t => t.id == currentTask.id);
                _currentUser.topics[index] = currentTask;
                _currentUser.topics[index].subject = selectedSubject;
            }

            //Saves all uplodadeed
            FileService fileService = new FileService(_currentUser);
            foreach (KeyValuePair<string, string> file in files)
            {
                //Saves file to s3 database
                fileService.saveFile($"wwwroot/TempFiles/{_currentUser.userId}/{file.Key}", file.Key, file.Value);

                //gets file url to view file
                string url = fileService.getUrl($@"{_currentUser.userId}/{file.Key}", "resource-storage-study-spaced").Result;

                //Adds file to topic list
                topic.files.Add(file.Key, url);
            }

            //Saves topic to database
            TopicService topicService = new TopicService(_currentUser);
            topicService.StoreTopic(topic.Model(topic.dueDate.ToString("yyyy/MM/dd")));

            //Resets all variables
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>(), _currentUser);
        }

        //Takes all inputs from the ui and saves it to a topic object
        public void taskSubmitted()
        {
            //Creates new topic from inputted variables
            Topic topic = new Topic(currentTask.name, selectedSubject, 0, currentTask.difficulty, currentTask.description, currentTask.files.Select(x => x.Key).ToList(), currentTask.links, _currentUser);
            
            //Sets the due dates
            topic.dueDate = DateTime.UtcNow.AddDays(calculateInterval(topic.repetitions, topic.difficulty)).AddHours(10);
            string dueDate = topic.dueDate.ToString("yyyy/MM/dd");

            //Saves topic to database
            TopicService topicService = new TopicService(_currentUser);
            topicService.StoreTopic(topic.Model(dueDate));

            //Saves topic to topic list
            _currentUser.allTopics.Add(topic);

            //Saves uploaded files to s3 bucket
            FileService fileService = new FileService(_currentUser);
            foreach(KeyValuePair<string, string> file in currentTask.files)
            {
                //Saves file
                fileService.saveFile($"wwwroot/TempFiles/{_currentUser.userId}/{file.Key}", file.Key, file.Value);
            }
            //Deletes all temp files for current user
            try
            {
                Directory.Delete(@$"wwwroot/TempFiles/{_currentUser.userId}/", true);
            }
            catch { }

            //Clears input variables
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>(), _currentUser);

        }

        //Runs when task is completed
        public void taskCompleted(Topic topic)
        {
            //Increments repetitions 
            topic.repetitions++;

            //Updates topic with updated values
            TopicService topicService = new TopicService(_currentUser);
            string dueDate = DateTime.Now.AddDays(calculateInterval(topic.repetitions, topic.difficulty)).ToString("yyyy/MM/dd");
            topicService.StoreTopic(topic.Model(dueDate));
        }

        //Removes topic from topic list
        public void removeTask(Topic topic)
        {
            _currentUser.topics.Remove(topic);
            _currentUser.allTopics.Remove(topic);
            TopicService topicService = new TopicService(_currentUser);
            topicService.DeleteTopic(topic.id);
        }

        //Deletes and replaces topic with current version
        public void updateTask(Topic task)
        {
            TopicService topicService = new TopicService(_currentUser);
            topicService.DeleteTopic(task.id);
            topicService.StoreTopic(task.Model());
        }

        //Defines the interval until the next time the topic is presented to the user
        public int calculateInterval(int repetitions, int difficulty)
        {
            Random rand = new Random();
            //Higher difficulty will reduce repetitions
            //More difficult topics will be revised more often and for longer
            repetitions /= difficulty+1;

            //If there are no more intervals defined for the current repetition
            if (repetitions >= _currentUser.intervals.Count())
                //set interval to final interval ± 0-deviation
                return Math.Abs(_currentUser.intervals[^1].delay + rand.Next(-_currentUser.deviation, 1 + _currentUser.deviation));
            //If repetitions are past first inteval, apply random deviation
            if(repetitions > 1)
                return Math.Abs(_currentUser.intervals[repetitions].delay + rand.Next(-_currentUser.deviation, 1 + _currentUser.deviation));

            //Return interval with no deivation
            return _currentUser.intervals[repetitions].delay;

        }

        //Wipes info from task edit / creation screen
        public void cancel()
        {
            try
            {
                currentTask.name = name;
                currentTask.description = description;
            }
            catch { }
            selectedSubject = new Subject("NULL", "NULL");
            links = new List<string>();
            files = new Dictionary<string, string>();
            currentTask = new Topic("", new Subject("NULL", "NULL"), 0, 0, "", new List<string>(), new List<string>(), _currentUser);
            try
            {
                Directory.Delete(@$"wwwroot/TempFiles/{_currentUser.userId}/", true);
            }
            catch { }

            displayTaskEdit = false;
            displayTaskEntry = false;
        }

        //Enters link to topic object
        public void addLink()
        {
            currentTask.links.Insert(0,link);
            link = "";
        }
    }
}
