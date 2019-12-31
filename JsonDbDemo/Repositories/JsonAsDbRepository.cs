using JsonDbDemo.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace JsonDbDemo.Repositories
{
	public class JsonAsDbRepository
	{
		private static ApplicationDataContainer _localSettings;
		private static Serilog.Core.Logger _logger;
		private static string _containerName = "";
		private static string _userName = "stephan";
		public bool isLocalDataSet = false;
		private static StorageFolder _appLocalFolder = ApplicationData.Current.LocalFolder;
		public JsonAsDbRepository(ApplicationDataContainer localSettings, string userName)
		{
			_logger = LogConfiguration.GetFileLogger();
			_containerName = userName;
			_localSettings = localSettings;
			isLocalDataSet = true;
			_userName = userName;
		}
		#region "Json as Database"
		public static async Task<Result> CreateJsonFile()
		{
			string FileName = _userName+ "_data.json";
			Result result = new Result();

			result.IsDbCreated = false;
			result.CreatedFileName = FileName;
			
			
			if (await isFilePresent(FileName) == false)
			{
				StorageFolder localFolder = ApplicationData.Current.LocalFolder;
				await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
				//write data in file
				try
				{
					_logger.Information("Creating DB For user " + FileName);
					
					StorageFile myFile = await localFolder.GetFileAsync(FileName);

					//string initialJsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///JsonInitialDb//UserData.json")));
					string initialJsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///JsonInitialDb//UserData.json")));
					await FileIO.WriteTextAsync(myFile, initialJsonString);
					result.IsDbCreated = true;
					_logger.Information("DB Created!!!!!!!!");

				}
				catch (Exception e)
				{
					_logger.Error("Failure: " + e.Message);
					result.IsDbCreated = false;
				}
			}
			else {
				_logger.Information(FileName + "is already there ");
			}
			return result;
		}
		public async static Task<Result> UpdateJsonFile(JsonDataModel jsonDataModel)
		{
			string FileName = _userName + "_data.json";
			Result result = new Result();

			result.IsDbCreated = false;
			result.CreatedFileName = FileName;
			_logger.Information(FileName + "is already there updating new content");
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
			_logger.Information("Updating  DB of user " + FileName);

			StorageFile myFile = await localFolder.GetFileAsync(FileName);
			string initialJsonString = JsonConvert.SerializeObject(jsonDataModel);
			//_logger.Information("data after serilization"+initialJsonString);
			await FileIO.WriteTextAsync(myFile, initialJsonString);
			result.IsDbCreated = true;
			_logger.Information("DB Updated!!!!!!!!");
			return result;
		}

			public async static Task<bool> isFilePresent(string fileName)
			{
				var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
				return item != null;
			}
		public class Result
		{
			public bool IsDbCreated { get; set; } = false;
			public string CreatedFileName { get; set; }
		}
		public async static Task<JsonDataModel> GetUserSavedData(string userName="admin")
		{
			_logger.Information("*************GetUserSavedData**********" + userName);
			string userDataString = string.Empty;

			//userDataString = _localSettings.Values[userName].ToString();

			userDataString = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalFolder.GetFileAsync(userName + "_data.json"));
			_logger.Information("saved data was::" + userDataString);


			//var f = JsonConvert.DeserializeObject<JsonDataModel>(userDataString);
			//_logger.Information("fetched data after convert" + f);
			return JsonConvert.DeserializeObject<JsonDataModel>(userDataString);
		}
		public async static Task<string> GetUserData(string userName = "admin")
		{
			_logger.Information("*************GetUserSavedData**********" + userName);
			string userDataString = string.Empty;

			//userDataString = _localSettings.Values[userName].ToString();

			userDataString = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalFolder.GetFileAsync(userName + "_data.json"));
			//_logger.Information("saved data was::" + userDataString);
			return userDataString;

			
		}
		#endregion
	}
}
