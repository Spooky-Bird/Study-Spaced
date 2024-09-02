using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Services;
using ServerApp;


namespace Testing
{
    [TestClass]
    public class UserServiceTest
    {
        UserService us = GetRequiredService<ServerApp.Services.UserService>();
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServerApp.Services.UserService>(ServiceProvider => new UserService(new User()));
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
        public void loadUser()
        {
            us.LoadUser("Ts9CYgNvsY");
            Assert.AreEqual(us._currentUser.userId, "Ts9CYgNvsY");
        }

        [TestMethod]
        public void getUser()
        {
            List<UserModel> users = us.GetUsers("Test User");
            Assert.AreEqual(users.Count, 1);
        }

        [TestMethod] 
        public void deleteUser()
        {
            us._currentUser.userId = "";
            us.delete();
        }

        [TestMethod]
        public void storeUser()
        {
            us._currentUser.userId = "";
            us.store(us._currentUser.Model());
        }
    }
}