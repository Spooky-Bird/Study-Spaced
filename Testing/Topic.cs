using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Services;
using ServerApp;


namespace Testing
{
    [TestClass]
    public class TopicServiceTest
    {
        TopicService ts = GetRequiredService<ServerApp.Services.TopicService>();
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServerApp.Services.TopicService>(ServiceProvider => new TopicService(new User()));
            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            string filePath = "keys.env";

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }

        [TestMethod]
        public void getTasks()
        {
            Assert.IsNotNull(ts.GetTasks());
        }

        [TestMethod]
        public void clearTopics()
        {
            ts.clearTopics();
        }

        [TestMethod] 
        public void deleteTopic()
        {
            ts.delete("");
        }

        [TestMethod]
        public void storeTopic()
        {
            Topic topic = new Topic("", new Subject("name", "#000000"), 0, 0, "fs", new List<string>(), new List<string>(), new User());
            ts.store(topic.Model());
        }
    }
}