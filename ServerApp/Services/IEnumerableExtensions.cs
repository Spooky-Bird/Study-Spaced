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
///Extends functionality of enuermable functions
///Used to include index of item in foreach loop
///**********************************************************************
namespace ServerApp.Services
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));
    }

}
