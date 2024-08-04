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
        public Topic(string name, Subject subject, int overdue, int difficulty, string description, List<string> files, List<string> links)
        {
            this.name = name;
            this.subject = subject;
            this.overdue = overdue;
            this.difficulty = difficulty;
            this.description = description;
            this.links = links;
            FileService fileService = new FileService();
            foreach(string fileName in files)
            {
                string url = fileService.getUrl($"{User.userId}/{fileName}", "resource-storage-study-spaced").Result;
                this.files.Add(fileName, url);
            }
            this.id = generateId();
            for (int i = 0; i < difficulty; i++)
            {
                difficultyIndicator += "!";
            }
        }
        public Topic(TopicModel model)
        {
            this.name = (model.topicName);
            this.subject = new Subject("NULL", "NULL");
            foreach (Subject subject in User.subjects)
            {
                if (subject.name == (model.subjectName))
                    this.subject = subject;
            }
            id = (model.topicId);
            dueDate = DateTime.ParseExact((model.dueDate), "yyyy/MM/dd", null);
            this.overdue = (int)(DateTime.UtcNow.AddHours(10) - dueDate).TotalDays;
            this.difficulty = int.Parse((model.difficulty));
            this.description = (model.description);
            repetitions = int.Parse((model.repetitions));
            try
            {
                this.links = (model.links).Split(";").ToList();
            } catch { this.links = new List<string>(); }
            List<string> fileNames = new List<string>();
            try
            {
                fileNames = (model.files).Split(";").ToList();
            } catch {  }
            FileService fileService = new FileService();

            foreach (string fileName in fileNames)
            {
                string url = fileService.getUrl($"{User.userId}/{fileName}", "resource-storage-study-spaced").Result;
                this.files.Add(fileName, url);
            }

            for (int i = 0; i < difficulty; i++)
            {
                difficultyIndicator += "!";
            }

            
        }
        public TopicModel Model(string due = null)
        {
            TopicModel model = new TopicModel();

            model.topicId = (id);
            model.description = (this.description);
            if (due != null)
                model.dueDate = (due);
            else
                model.dueDate = (dueDate.ToString("yyyy/MM/dd"));
            model.difficulty = (difficulty.ToString());
            model.repetitions = (repetitions.ToString());
            model.subjectName = (subject.name);
            model.topicName = (name);
            model.userId = User.userId;
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
