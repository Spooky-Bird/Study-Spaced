using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp.ColorSpaces;
using Spire;
using Spire.Doc;
using Spire.Presentation;
using Spire.Xls;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace ServerApp.Services
{
    public class FileService
    {
        public AmazonS3Client s3Client = new AmazonS3Client("AKIA4MTWG6LQHO536SWH", "GDDcWwvn6BN0iijtf3YGnCP/oH+mZZefZyXFYNuw", Amazon.RegionEndpoint.APSoutheast2);

        public async Task saveFile(string path, string name, string fileType)
        {
            if (!FolderExists(User.userId).Result)
                CreateUserFolder("resource-storage-study-spaced");
            try
            {
                await s3Client.PutObjectAsync(new PutObjectRequest() { BucketName = "resource-storage-study-spaced", Key = $"{User.userId}/{name}", ContentType = fileType, FilePath = path, });
                File.Delete(path);
            }
            catch (Exception e){ }
        }

        public async Task<bool> FolderExists(string folderName)
        {
            var request = new ListObjectsRequest()
            {
                BucketName = "resource-storage-study-spaced",
                Prefix = folderName
            };
            bool smae = false;
            try
            {
                smae = s3Client.ListObjectsAsync(request).Result.S3Objects.Count() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e) ;
            }
            return smae;
        }

        public async Task CreateUserFolder(string directory)
        {
            var request = new PutObjectRequest();
            request.BucketName = directory;

            request.Key = User.userId + "/";
            try
            {
            await s3Client.PutObjectAsync(request);
            }
            catch (Exception e) { } 
        }

        public async Task deleteFile(string name)
        {
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = @"resource-storage-study-spaced", Key = $"{User.userId}/{name}" });
        }

        public  async Task savePfp(string path, string name, string fileType)
        {
            string key = string.Join('.', new string[] { User.userId, name.Split('.')[1] });
                                                                         //this has been changed (key => User.userId)  \/ 
            await s3Client.PutObjectAsync(new PutObjectRequest() { BucketName = "pfp-storage-study-spaced", Key = User.userId, ContentType = fileType, FilePath = path });
            File.Delete($@"wwwroot\TempFiles\{User.userId}\pfp\{name}");
            User.pfpUrl = await getUrl($@"{key}", "pfp-storage-study-spaced");
        }

        public async Task<string> getUrl(string path, string bucket)
        {
            return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest() { BucketName = bucket, Key = path, Expires = DateTime.UtcNow.AddDays(1) });
        }

        public async Task<bool> convToPdf(string path, string fileType)
        {
			string newPath = path.Split(".")[0] + ".pdf";
            

			switch (fileType)
            {
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    Document doc = new Document();
                    doc.LoadFromFile(path);
                    doc.SaveToFile(newPath);
                    File.Delete(path);
                    return true;
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    Presentation ppt = new Presentation();
					ppt.LoadFromFile(path, Spire.Presentation.FileFormat.PDF);
					ppt.SaveToFile(newPath, Spire.Presentation.FileFormat.PDF);
                    File.Delete(path);
                    return true;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    Workbook workbook = new Workbook();
                    workbook.LoadFromFile(path);
					Worksheet sheet = workbook.Worksheets[0];
                    sheet.SaveToPdf(newPath);
                    File.Delete(path);
                    return true;
			}
            return false;   
		}

        public async Task fullBackup()
        {
            var conditions = new List<ScanCondition>();
            ListObjectsResponse resp = s3Client.ListObjectsAsync("resource-storage-study-spaced").Result;
            Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\");
            foreach(S3Object objects in resp.S3Objects)
            {
                string[] dirs = objects.Key.Split('/');
                string path = string.Join("\\", dirs[..^1]);
                Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\{path}");
                if (objects.Key[^1] != '/')
                {
                    GetObjectResponse objResp = s3Client.GetObjectAsync("resource-storage-study-spaced", objects.Key).Result;
                    path = $@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\{objects.Key.Replace("/", "\\")}";
                    objResp.WriteResponseStreamToFileAsync(path, false, new CancellationToken());

                }
            }

            resp = s3Client.ListObjectsAsync("pfp-storage-study-spaced").Result;
            Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Profile Pictures\");
            foreach (S3Object objects in resp.S3Objects)
            {
                GetObjectResponse objResp = s3Client.GetObjectAsync("pfp-storage-study-spaced", objects.Key).Result;
                string path = $@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Profile Pictures\{objects.Key.Replace("/", "\\")}";
                objResp.WriteResponseStreamToFileAsync(path, false, new CancellationToken());
            }
        }
    }
}
