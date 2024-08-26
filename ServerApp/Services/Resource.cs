///**********************************************************************
///
///Project: ServerApp
///
///Page: Home
///Folder: Services
///
///Author: Simon Wunderlich
///
///Description
///Stores information for a file to be downloaded from s3
///**********************************************************************
namespace ServerApp.Services
{
    public class Resource
    {
        public string icon;
        public string name;
        public Resource(string icon, string name)
        {
            this.icon = icon;
            this.name = name;
        }
    }
}
