using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Services;
using ServerApp;


namespace Testing
{
    [TestClass]
    public class UserTest
    {
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServerApp.Services.User>();
            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }

        [TestMethod]
        public void Model()
        {
            ServerApp.Services.User user = GetRequiredService<ServerApp.Services.User>();
            UserModel model = user.Model();
            Assert.AreEqual(model.userId, user.userId);
        }

        [TestMethod]
        public void generateId()
        {
            ServerApp.Services.User user = GetRequiredService<ServerApp.Services.User>();
            user.generateId();
            Assert.IsNotNull(user.userId);
        }

        [TestMethod]
        public void clear()
        {
            var user = GetRequiredService<ServerApp.Services.User>();
            user.clear();
            Assert.AreEqual("", user.userId);
        }

    }
}