using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using Spire.Doc;
using Spire.Presentation;
using Spire.Xls;

namespace ServerApp.Services
{
    //Defines service class to control s3 file api
    public class FileService
    {
        //defines s3 client
        public AmazonS3Client s3Client;

        User _currentUser;

        public FileService(User user)
        {
            s3Client = new AmazonS3Client(Environment.GetEnvironmentVariable("Access"), Environment.GetEnvironmentVariable("Secret"), Amazon.RegionEndpoint.APSoutheast2);
            _currentUser = user;
        }

        //Saves a file to resource bucket
        public async Task saveFile(string path, string name, string fileType)
        {
            //Checks if user already has their own directory
            if (!FolderExists(_currentUser.userId).Result)
                //Creates new directory for user
                CreateUserFolder("resource-storage-study-spaced");
            try
            {
                //saves file to bucket
                await s3Client.PutObjectAsync(new PutObjectRequest() { BucketName = "resource-storage-study-spaced", Key = $"{_currentUser.userId}/{name}", ContentType = fileType, FilePath = path, });
                
                //Deletes file from temporary local storage
                File.Delete(path);
            }
            catch (Exception e){ }
        }

        //Checks if folder exists
        public async Task<bool> FolderExists(string folderName)
        {
            //Creates list object request
            //prefix determines folder name
            var request = new ListObjectsRequest()
            {
                BucketName = "resource-storage-study-spaced",
                Prefix = folderName
            };

            bool exists = false;
            //Lists all folders with given folder name
            try
            {
                //If any folders exists, sets to true
                exists = s3Client.ListObjectsAsync(request).Result.S3Objects.Count() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e) ;
            }
            return exists;
        }

        //Creates new folder to  store user files
        public async Task CreateUserFolder(string directory)
        {
            //Initialises new request
            var request = new PutObjectRequest();

            request.BucketName = directory;

            //key is used as define file name + path
            // / is used to signify object is a folder
            request.Key = _currentUser.userId + "/";

            //Sends put request
            try
            {
            await s3Client.PutObjectAsync(request);
            }
            catch (Exception e) { } 
        }

        //Deletes a given file from resources database
        public async Task deleteFile(string name)
        {
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = @"resource-storage-study-spaced", Key = $"{_currentUser.userId}/{name}" });
        }

        //saves users profile picture 
        public  async Task savePfp(string path, string name, string fileType)
        {
            //Saves file using the user's user id as its key
            await s3Client.PutObjectAsync(new PutObjectRequest() { BucketName = "pfp-storage-study-spaced", Key = _currentUser.userId, ContentType = fileType, FilePath = path });
            
            //Deletes pfp from temparary storage
            File.Delete($@"wwwroot\TempFiles\{_currentUser.userId}\pfp\{name}");

            //Gets link to the uploaded file to reference
            _currentUser.pfpUrl = await getUrl($@"{_currentUser.userId}", "pfp-storage-study-spaced");
        }

        //Gets a pre signed url for an item in an s3 bucket
        public async Task<string> getUrl(string path, string bucket)
        {
            try
            {
                //Checks if item exists by attempting to get its metadata
                //This is used as if item doesnt exist, sClient will return an invalid url
                var response = s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
                {
                    BucketName = bucket,
                    Key = path
                }).Result;
            }

            catch (Amazon.S3.AmazonS3Exception ex)
            {
                throw;
            }
            //gets the link for the file which will expire in 1 day
            return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest() { BucketName = bucket, Key = path, Expires = DateTime.UtcNow.AddDays(1) });
        }

        //Deletes all of a users resources
        //Run when user acct is deleted
        public async Task clearFiles()
        {
            //Deletes resource folder for user
            deleteFolder("resource-storage-study-spaced", _currentUser.userId);

            //Deletes users pfp
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = @"pfp-storage-study-spaced", Key = $"{_currentUser.userId}" });

        }

        //Deletes a folder and clears all files recursively
        public async Task deleteFolder(string bucketName, string folderName)
        {
            DeleteObjectsRequest request2 = new DeleteObjectsRequest();

            //Builds list object request
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = folderName
            };

            //Gets all items in given folder
            ListObjectsResponse response = await s3Client.ListObjectsAsync(request);
            
            //Each item in folder is added into deleteobjectsasync request
            foreach (S3Object entry in response.S3Objects)
            {

                request2.AddKey(entry.Key);
            }
            request2.BucketName = bucketName;
            
            //Request sent
            await s3Client.DeleteObjectsAsync(request2);
        }

        //Converts common office file types to pdf
        //File types include .docx, .csv, and .ppt
        public async Task<bool> convToPdf(string path, string fileType)
        {
            //replaces file extention with .pdf
			string newPath = path.Split(".")[0] + ".pdf";
            
            //Checks file type and uses Spire library to convert into a pdf
			switch (fileType)
            {
                //if .docx
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    Document doc = new Document();

                    //gets file
                    doc.LoadFromFile(path);

                    //saves pdf file to same dir
                    doc.SaveToFile(newPath);

                    //deletes og file
                    File.Delete(path);

                    //sucess
                    return true;

                //if .pptx
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    Presentation ppt = new Presentation();

                    //Gets file
					ppt.LoadFromFile(path, Spire.Presentation.FileFormat.PDF);

                    //Saves pdf file to same dir
					ppt.SaveToFile(newPath, Spire.Presentation.FileFormat.PDF);

                    //deletes og file
                    File.Delete(path);

                    //sucess
                    return true;

                //if csv
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    Workbook workbook = new Workbook();

                    //get file
                    workbook.LoadFromFile(path);

                    //get first worksheet from file
					Worksheet sheet = workbook.Worksheets[0];

                    //save worksheet to pdf in same dir
                    sheet.SaveToPdf(newPath);

                    //delete og csv
                    File.Delete(path);
                    
                    //Sucess
                    return true;
			}
            return false;   
		}

        //backs up all files in s3 database
        public async Task fullBackup()
        {
            //gets all resources
            ListObjectsResponse resp = s3Client.ListObjectsAsync("resource-storage-study-spaced").Result;

            //create new directory to store resource backup
            Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\");

            //iterates through all items
            foreach(S3Object objects in resp.S3Objects)
            {
                //gets path leading up to object
                string[] dirs = objects.Key.Split('/');
                string path = string.Join("\\", dirs[..^1]);

                //creates directory using same file structure
                Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\{path}");

                //if the item isnt a folder
                if (objects.Key[^1] != '/')
                {
                    //get the object
                    GetObjectResponse objResp = s3Client.GetObjectAsync("resource-storage-study-spaced", objects.Key).Result;

                    //save to file
                    path = $@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Resources\{objects.Key.Replace("/", "\\")}";
                    objResp.WriteResponseStreamToFileAsync(path, false, new CancellationToken());

                }
            }

            //gets all pfp files
            resp = s3Client.ListObjectsAsync("pfp-storage-study-spaced").Result;

            //creates new directory to store pfp backup
            Directory.CreateDirectory($@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Profile Pictures\");

            //iteratees through all pfps
            foreach (S3Object objects in resp.S3Objects)
            {
                //gets file
                GetObjectResponse objResp = s3Client.GetObjectAsync("pfp-storage-study-spaced", objects.Key).Result;

                //saves to path
                string path = $@"wwwroot\Backups\Files\{DateTime.UtcNow.AddHours(10).ToString("dd-MM-yyyy")}\Profile Pictures\{objects.Key.Replace("/", "\\")}";
                objResp.WriteResponseStreamToFileAsync(path, false, new CancellationToken());
            }
        }
    }
}
