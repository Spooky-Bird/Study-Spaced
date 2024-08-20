using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ServerApp.Services;
using Syncfusion.Blazor;
namespace ServerApp
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();

            builder.Services.AddSyncfusionBlazor();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM2NTMxNUAzMjM2MmUzMDJlMzBXazR1cUtWVjZtQXF6VEJJaSs3R3VTZndVVXpCSkxVMFpvWllqaFlXSExvPQ==");
            builder.Services.AddScoped<HttpClient>();

            string filePath = "keys.env";
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }


            builder.Services.AddScoped<Env>();	
			builder.Services.AddScoped<User>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TopicService>();
			builder.Services.AddScoped<FileService>();
			builder.Services.AddScoped<UserModel>();
			builder.Services.AddScoped<TopicModel>();
			builder.Services.AddScoped<TaskController>();

   //         builder.Services.AddCors(options =>
			//{
			//	options.AddPolicy(name: MyAllowSpecificOrigins,
			//					  policy =>
			////					  {
			////						  policy.WithOrigins("https://wpszys0ba0.execute-api.ap-southeast-2.amazonaws.com/UserAPI");
			////					  });
			////});

   //         builder.Services.AddCors(options =>
   //         {
   //             options.AddPolicy("NewPolicy", builder =>
   //             builder.AllowAnyOrigin()
   //                 .AllowAnyMethod()
   //                 .AllowAnyHeader());
   //         });


            builder.Services.AddBlazoredLocalStorage();

			

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();
            //app.UseCors("NewPolicy");
            app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");




			app.Run();
		}
	}
}