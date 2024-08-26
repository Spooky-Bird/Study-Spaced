///**********************************************************************
///
///Project: ServerApp
///
///Page: Home, Settings
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///Defines subject object, stores all necessary info for subject
///**********************************************************************
namespace ServerApp.Services
{
    public class Subject
    {
        public string name;
        public string colour;
        public Subject(string name, string colour)
        {
            this.name = name;
            this.colour = colour;
        }
    }
}
