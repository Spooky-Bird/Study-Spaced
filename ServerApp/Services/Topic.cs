///**********************************************************************
///Project: ServerApp
///
///Page: Home
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///Defines object to store all information for a topic
///Includes functionality to convert between a Topic and Topic Model and visa versa
///**********************************************************************
namespace ServerApp.Services
{
    public class Topic
    {
        public string name { get; set; }
        public int overdue;
        public int difficulty;
        public Subject subject;
        public string description;
        public string difficultyIndicator = "";
        public string timerIcon = "fa-solid fa-play";
        public Dictionary<string, string> files = new Dictionary<string, string>();
        public List<string> links = new List<string>();

        public int repetitions = 0;
        public int duration = 300;
        public int elapsedSeconds;
        public DateTime dueDate;
        public string id;
        public string timePretty;

        User _currentUser;

        //Constructs topic with given info
        public Topic(string name, Subject subject, int overdue, int difficulty, string description, List<string> files, List<string> links, User user)
        {
            //Assigns variables
            _currentUser = user;
            this.name = name;
            this.subject = subject;
            this.overdue = overdue;
            this.difficulty = difficulty;
            this.description = description;
            this.links = links;

            //Saves gets all files from database
            FileService fileService = new FileService(_currentUser);
            foreach(string fileName in files)
            {
                try
                {
                    //gets url
                    string url = fileService.getUrl($"{_currentUser.userId}/{fileName}", "resource-storage-study-spaced").Result;
                    this.files.Add(fileName, url);
                }
                catch { }
            }
            //create unique identifyer
            this.id = generateId();

            //convert difficulty from int to visual representation
            // 3 => !!!
            for (int i = 0; i < difficulty; i++)
            {
                difficultyIndicator += "!";
            }
        }

        public Topic() { }

        //Construct topic from topic model taken from the database
        public Topic(TopicModel model, User user)
        {
            _currentUser = user;
            this.name = (model.topicName);
            this.subject = new Subject("NULL", "NULL");
            //Checks if entered subject exists
            //if not, subject is NULL
            foreach (Subject subject in _currentUser.subjects)
            {
                if (subject.name == (model.subjectName))
                    this.subject = subject;
            }
            id = (model.topicId);

            //conv date string to datetime
            dueDate = DateTime.ParseExact((model.dueDate), "yyyy/MM/dd", null);

            //gets number of days since dueDate
            this.overdue = (int)(DateTime.UtcNow.AddHours(10) - dueDate).TotalDays;
            this.difficulty = int.Parse((model.difficulty));
            this.description = (model.description);
            repetitions = int.Parse((model.repetitions));

            //Splits link string and assigns to list
            try
            {
                this.links = (model.links).Split(";").ToList();
            } catch { this.links = new List<string>(); }
            List<string> fileNames = new List<string>();
            //Splits file string and assigns to list
            try
            {
                fileNames = (model.files).Split(";").ToList();
            } catch {  }

            //Gets urls for each file
            FileService fileService = new FileService(_currentUser);
            foreach (string fileName in fileNames)
            {
                try { string url = fileService.getUrl($"{_currentUser.userId}/{fileName}", "resource-storage-study-spaced").Result;
                    this.files.Add(fileName, url);
                } catch { }
                
            }
            //convert difficulty from int to visual representation
            // 3 => !!!
            for (int i = 0; i < difficulty; i++)
            {
                difficultyIndicator += "!";
            }

            
        }

        //Generates a TopicModel to be entered into database
        public TopicModel Model(string due = null)
        {
            TopicModel model = new TopicModel();

            model.topicId = (id);
            model.description = (this.description);
            //check if due date has changed and apply 
            if (due != null)
                model.dueDate = (due);
            else
                model.dueDate = (dueDate.ToString("yyyy/MM/dd"));
            model.difficulty = (difficulty.ToString());
            model.repetitions = (repetitions.ToString());
            model.subjectName = (subject.name);
            model.topicName = (name);
            model.userId = _currentUser.userId;

            //Convert resources from lists to strings seperated by ;
            model.links = (string.Join(";",links));
            model.files = (string.Join(";",files.Select(x => x.Key)));

            return model;
        }

        //Generates a random alphanumeric string 10 character long
        public static string generateId()
        {
            string charBank = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var rand = new Random();
            string itemId = "";
            for (int i = 0; i < 10; i++)
            {
                itemId += charBank[rand.Next(charBank.Length)];
            }
            return itemId;
        }

    }
}
