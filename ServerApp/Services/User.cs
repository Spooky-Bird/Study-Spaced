using System.Reflection;
using System.Security.Cryptography;

//Stores current users information

namespace ServerApp.Services
{
    public class User
    {
        public string userId = "";
        public string pfpUrl = "'defaultPfp.png'";
        public string username;
        public string email;
        public string password;
        public int deviation = 7;
        public string privateKey;
        public string publicKey;
        public List<Subject> subjects = new List<Subject>();
        public List<Topic> topics = new List<Topic>();
        public List<Topic> allTopics = new List<Topic>();
        public List<Interval> intervals = new List<Interval>() { new Interval(1, 20), new Interval(7, 15), new Interval(14, 10), new Interval(28, 5) };

        public User()
        {
        }

        //Generates a UserModel object to be stored in the user database
        public UserModel Model()
        {
            UserModel Model = new UserModel();
            Model.username = username;
            Model.password = (password);
            Model.email = (email);
            if(userId == "")
			{
                generateId();
			}
            Model.userId = userId;

            Model.deviation = (deviation.ToString());

            //Converts List of class Subject into two strings representing names and colours
            //Each string is delimited by semicolons
			Model.subjectNames = (string.Join(";", subjects.Select(x => x.name)));
            Model.subjectColours = (string.Join(";", subjects.Select(x => x.colour)));

            //Converts List of class Interval into two strings representing dely and duration
            //Each string is delimited by semicolons
            Model.delays = (string.Join(";", intervals.Select(x => x.delay.ToString())));
            Model.intervals = (string.Join(";", intervals.Select(x => x.duration.ToString())));
            return Model;
        }

        //Generates a random alphanumeric string 10 character long
        public void generateId()
        {
			string charBank = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			var rand = new Random();
			for (int i = 0; i < 10; i++)
			{
				userId += charBank[rand.Next(charBank.Length)];
			}
		}

        //Converts information from a UserModel retrieved from the user database to the User object
        public void Generate(UserModel uModel)
        {
			userId = uModel.userId;
			username = (uModel.username);
			email =    (uModel.email);
            password = (uModel.password);
            deviation = int.Parse((uModel.deviation));
            FileService fileService = new FileService(this);
            if (pfpUrl == "'defaultPfp.png'")
                try { pfpUrl = fileService.getUrl(userId, "pfp-storage-study-spaced").Result; } catch { }
            subjects = new List<Subject>();
            //Generates new Subject objects from the provided strings for name and colour
			for (int i = 0; i < (uModel.subjectNames).Split(";").Count(); i++)
			{
                if((uModel.subjectNames).Split(";")[i] != "")
				    subjects.Add(new Subject((uModel.subjectNames).Split(";")[i], (uModel.subjectColours).Split(";")[i]));
			}

            intervals = new List<Interval>();
            //Generates new interval objects from the provided strings for delay and duration
            for (int i = 0; i < (uModel.intervals).Split(";").Count(); i++)
            {
                intervals.Add(new Interval(Int32.Parse((uModel.delays).Split(";")[i]), Int32.Parse((uModel.intervals).Split(";")[i])));
            }
        }

        //Wipes the User object of all information when user logs out
        public void clear()
        {
            userId = "";
            username = "";
            password = "";
            email = "";
			pfpUrl = "'defaultPfp.png'";
			subjects = new List<Subject>();
            topics = new List<Topic>();
            allTopics = new List<Topic>();
            intervals = new List<Interval>();
        }
    }
}
