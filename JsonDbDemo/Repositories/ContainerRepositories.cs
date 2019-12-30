
using JsonDbDemo.Models;
using Newtonsoft.Json;
using System;
using Windows.Data.Json;
using Windows.Storage;

namespace JsonDbDemo.Repositories
{

	public class ContainerRepositories
	{
		private static ApplicationDataContainer _localSettings;
		private static Serilog.Core.Logger _minderLogger;
		private static string _containerName = "";
		private static string _userName = "";
		public bool isLocalDataSet = false;
		private static StorageFolder _appLocalFolder = ApplicationData.Current.LocalFolder;
		public ContainerRepositories(ApplicationDataContainer localSettings, string userName)
		{
			_minderLogger = LogConfiguration.GetFileLogger();
			_containerName = userName;
			_localSettings = localSettings;
			isLocalDataSet = true;
			_userName = userName;
		}
		#region minder local data management
		//need to study versioning from micsoft docs
		//https://docs.microsoft.com/en-us/windows/uwp/design/app-settings/store-and-retrieve-app-data#versioning-your-app-data


		#endregion
		#region not working in prod (ie deliver to client)data management in container
		//check if container exist
		public static bool IsContainerExist(string containerName)
		{
			return _localSettings.Containers.ContainsKey(containerName);
		}
		//create a new container
		public static void CreateContainer(string userName, string userPassword)
		{
			_minderLogger.Information("################## Creating Container for " + userName + " ###################");
			if (!IsContainerExist(userName))
			{
				var c = _localSettings.CreateContainer(userName, ApplicationDataCreateDisposition.Always);
				_minderLogger.Information("New Container Is" + c.Name.ToString());
				_containerName = c.Name.ToString();
				//create default app data for this user in separate container
				SetDefaultData();
				_minderLogger.Information("################## Container  Created ###################");
			}
			else
			{
				_containerName = userName;
				//key is casesensitive
				UpdateSettingValue("UserId", userName);
				UpdateSettingValue("Password", userPassword);
			}
		}
		public async static void SetDefaultData()
		{
			//data stored in json as a content can not be edited in uwp ie read only
			//so i need to push this data in application data container which is allowed to read as well as write
			//string jsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DB//UserData.json")));

			// string jsonStrings = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets//UserData.json")));
			//working fine 
			_minderLogger.Information("$$$$$$$$$$$$$$$$$$$$$$$ Setting Default data for " + _containerName + " $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
			string jsonString = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("UserData.json"));

			var rootObject = JsonObject.Parse(jsonString);

			//newContainer.Values["UserData"] = rootObject.ToString();
			try
			{
				_minderLogger.Information(jsonString);
				_localSettings.Containers[_containerName].Values["UserData"] = jsonString;
				//_localSettings.Values[_containerName] = jsonString;
				_minderLogger.Information("$$$$$$$$$$$$$$$$ Default Data Created $$$$$$$$$$$$$$$");
			}
			catch (Exception ex) { _minderLogger.Error("Error while setting default data" + ex.StackTrace); }
		}

		public static void AddNewSettingValue(string settingKey, string settingValue)
		{
			if (IsSettingExist(settingKey))
			{
				UpdateSettingValue(_containerName, settingValue);
			}
			else { _localSettings.Containers[_containerName.ToString()].Values[settingKey] = settingValue.ToString(); }
		}
		public static void UpdateSettingValue(string settingKey, string settingValue)
		{
			_minderLogger.Information("UpdateSettingValue called");
			var userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
			//var rootObject = JsonObject.Parse(userDataString);
			//JsonObject rootObject = null;
			JsonObject.TryParse(userDataString, out JsonObject rootObject);
			//settingKeyis case sensitive
			rootObject.SetNamedValue(settingKey, JsonValue.CreateStringValue(settingValue));
			var u = JsonConvert.DeserializeObject<JsonDataModel>(rootObject.Stringify());
			try
			{
				_localSettings.Containers[_containerName.ToString()].Values["UserData"] = JsonConvert.SerializeObject(u);
			}
			catch (Exception ex) { }
		}
		public static bool IsSettingExist(string settingKey)
		{
			return _localSettings.Containers[_containerName.ToString()].Values.ContainsKey(settingKey);
		}
		public static string GetSettingValue(string settingKey)
		{
			var value = "";
			if (IsSettingExist(settingKey) == true)
			{
				value = _localSettings.Containers[_containerName.ToString()].Values[settingKey].ToString();
			}
			return value;
		}
		public static void DeleteContainer()
		{
			_localSettings.DeleteContainer(_containerName);
		}
		public static JsonDataModel GetUserLocalData()
		{
			_minderLogger.Information("*************GetUserLocalData**********" + _containerName.ToString());
			string userDataString = string.Empty;
			if (IsContainerExist(_containerName))
			{
				//userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
				//_minderLogger.Information("dksingh" + userDataString);
				userDataString = _localSettings.Values[_containerName].ToString();
				_minderLogger.Information("dksingh_new" + userDataString);
			}
			else { _minderLogger.Information(_containerName + "Not Exist"); }

			var f = JsonConvert.DeserializeObject<JsonDataModel>(userDataString);
			_minderLogger.Information("dk" + f);
			return JsonConvert.DeserializeObject<JsonDataModel>(userDataString);
		}

		public static JsonObject GetUserData()
		{
			var userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
			JsonObject.TryParse(userDataString, out JsonObject rootObject);
			return rootObject;
		}

		//get current user  container data
		public static void GetContainerData()
		{
			//https://docs.microsoft.com/en-us/previous-versions/windows/apps/hh770289(v=win.10)?redirectedfrom=MSDN#code-snippet-1
			var userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
			//JsonObject.TryParse(userDataString, out JsonObject rootObject);		
			//  UserData userData1 = JsonConvert.DeserializeObject<UserData>(userDataString);			
			////finding named array data not working

			//var m = GetOppSettings();
			//JsonObject.TryParse(rootObject.GetNamedValue("Walking").ToString(), out JsonObject userData);
			////get a value from json
			//rootObject.TryGetValue("SettingName", out IJsonValue jsonValue);
			////SettingDetail settingDetail = JsonConvert.DeserializeObject<SettingDetail>(userData.GetNamedObject("SettingDetails").Stringify());

			//CreateFile("GetContainerData.txt");
			//WriteData(JsonConvert.SerializeObject(data));



		}
		#endregion

	}
}
