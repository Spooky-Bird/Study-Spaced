using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Services;
using ServerApp;


namespace Testing
{
    [TestClass]
    public class FileServiceTest
    {
        FileService fs = GetRequiredService<ServerApp.Services.FileService>();

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddScoped<ServerApp.Services.FileService>(ServiceProvider => new FileService(new User()));
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
        public void saveFile()
        {
            try
            {
                fs.saveFile(@"defaultPfp.png", "Test Image", "image/png");
            } catch { Assert.Fail(); }
        }

        [TestMethod]
        public void getFile()
        {

            string url = fs.getUrl("Test.png", "test-study-spaced").Result;
            Uri uriResult;
            //uses Uri.TryCreate to attempt to generate a uri from the link
            //returns true if sucessful, false if not
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void deleteFile()
        {
            try
            {
                fs.deleteFile("Test.png");
            } catch { Assert.Fail();  }
        }

        [TestMethod]
        public void pdfConv() {
            File.WriteAllText("Test.docx", "Test");
            Assert.IsTrue(fs.convToPdf("Test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document").Result);
        }
    }
}