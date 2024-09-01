using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Services;
using ServerApp;


namespace Testing
{
    [TestClass]
    public class TaskControllerTest
    {
        TaskController tc = GetRequiredService<ServerApp.Services.TaskController>();

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServerApp.Services.TaskController>(ServiceProvider => new TaskController(new User()));
            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }

        [TestMethod]
        public void confirmEdit()
        {
            try
            {
                tc.currentTask.difficulty = 100;
                tc.currentTask.description = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                tc.currentTask.name = "XXXXXX";
                tc.confirmEdit();
            } catch { Assert.Fail(); }
        }

        [TestMethod]
        public void taskSubmitted()
        {
            try
            {
                tc.currentTask.difficulty = 100;
                tc.currentTask.description = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                tc.currentTask.name = "XXXXXX";
                tc.taskSubmitted();
            }
            catch { Assert.Fail(); }
        }

        [TestMethod]
        public void calculateInterval()
        {
            int interval = tc.calculateInterval(100, 5);
            if(interval < tc._currentUser.intervals[^1].delay - tc._currentUser.deviation || interval > tc._currentUser.intervals[^1].delay + tc._currentUser.deviation)
            {
                Assert.Fail();
            }

            int interval2 = tc.calculateInterval(0, 0);
            if (interval2 < tc._currentUser.intervals[0].delay - tc._currentUser.deviation || interval2 > tc._currentUser.intervals[0].delay + tc._currentUser.deviation)
            {
                Assert.Fail();
            }
        }
    }
}