
using JsonDbDemo;
using JsonDbDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace Minder.Data
{//https://docs.microsoft.com/en-us/uwp/api/windows.data.json.jsonobject
	public class DataRpositories
	{
	    private static	ApplicationDataContainer _localSettings ;
		private static Serilog.Core.Logger _minderLogger;
		private static string _containerName = "";
		private static string _userName = "";
		public   bool isLocalDataSet = false;
		private static StorageFolder _appLocalFolder = ApplicationData.Current.LocalFolder;
		public DataRpositories( ApplicationDataContainer localSettings,string userName)
		{
			_minderLogger = LogConfiguration.GetFileLogger();
			_containerName = userName;
			_localSettings = localSettings;
			isLocalDataSet = true;
			_userName = userName;
		}
		public DataRpositories(string userName)
		{
			_containerName = userName;		
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
		public static void CreateContainer(string userName,string userPassword)
		{
			_minderLogger.Information("################## Creating Container for "+userName+" ###################");
			 if (!IsContainerExist(userName))
			 {
			  var c=  _localSettings.CreateContainer(userName, ApplicationDataCreateDisposition.Always);
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
   catch(Exception ex) { _minderLogger.Error("Error while setting default data"+ex.StackTrace); }			
		}
		
		public static void AddNewSettingValue(string settingKey,string settingValue)
		{
			if (IsSettingExist(settingKey))
			{
				UpdateSettingValue(_containerName,settingValue);
			}
			else { _localSettings.Containers[_containerName.ToString()].Values[settingKey] = settingValue.ToString(); }
		}
		public static void UpdateSettingValue(string settingKey, string settingValue)
		{
			_minderLogger.Information("UpdateSettingValue called");
			var userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
			//var rootObject = JsonObject.Parse(userDataString);
			//JsonObject rootObject = null;
			JsonObject.TryParse(userDataString,out JsonObject rootObject);
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
				value= _localSettings.Containers[_containerName.ToString()].Values[settingKey].ToString();
			}
			return value;
		}
		public static void  DeleteContainer()
		{
			_localSettings.DeleteContainer(_containerName);
		}
		public static JsonDataModel GetUserLocalData()
		{
			_minderLogger.Information("*************GetUserLocalData**********"+_containerName.ToString());
			string userDataString = string.Empty;
			if (IsContainerExist(_containerName))
			{
				//userDataString = _localSettings.Containers[_containerName.ToString()].Values["UserData"].ToString();
				//_minderLogger.Information("dksingh" + userDataString);
				userDataString=_localSettings.Values[_containerName].ToString();
				_minderLogger.Information("dksingh_new" + userDataString);
			}
			else { _minderLogger.Information(_containerName+"Not Exist"); }
	       
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
		
  
         public static void SaveLocalData(JsonDataModel userLocalAppData)
		{
			//organising data in container was not working in production need some rnd will do later
			//_localSettings.Containers[_containerName.ToString()].Values["UserData"] = JsonConvert.SerializeObject(userLocalAppData);

			//as per microsoft doc there is no limitation of data size in local setting
			//remove setting
			_localSettings.Values.Remove(_userName) ;
			//push setting
			_localSettings.Values[_userName] = JsonConvert.SerializeObject(userLocalAppData);
			

		}
	


		//#region dataManagement without container in local folder
		//public  async static Task<JsonDataModel> GetMinderLocalData(string userName)
		//{
		//	userName = userName.ToString() + "_data.json";			
		//	try
		//	{
		//		string jsonString = await FileIO.ReadTextAsync(await ApplicationData.Current.LocalFolder.GetFileAsync(userName));
		//		return JsonConvert.DeserializeObject<JsonDataModel>(jsonString);

		//	}
		//	catch (Exception ex) { _minderLogger.Error("Error while GetMinderLocalData" + ex.StackTrace); return null; }
		//}
		////data will be stored in json in applocal folder and get at start time
		//public static async Task CreateDataFile(string userName,bool isInitialData)
		//{
		//	_userName = userName??_userName;
		//	string fileName = _userName + "_data.json";

		//	await _appLocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
		//	//string jsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DB//UserData.json")));
		//	var dataString = await GetData(isInitialData);
		//   await	 WriteDataToFile(dataString,fileName);
		//}
		//private static async Task WriteDataToFile(string data, string fileName)
		//{
		//	try
		//	{
		//		if (_appLocalFolder != null)
		//		{
		//			StorageFile userJsonFile = await _appLocalFolder.GetFileAsync(fileName);
		//		//var 	file = _appLocalFolder.GetFileAsync(fileName).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

		//			if (userJsonFile != null&& userJsonFile.Name==fileName)
		//			{
		//				_minderLogger.Information("dataTo Write"+"File Atributes"+userJsonFile.Attributes+"is avai  "+userJsonFile.IsAvailable+data);
		//				//when in production mode data is there but not writting anything on json
		//				//even in debug/release mode working file
		//				//File.SetAttributes(file, FileAttributes.Normal);
		//				//var folder = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("DB").AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
		//				//file = StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data.json")).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
		//				////var file = folder.GetFileAsync("UserData.json").AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
		//				await FileIO.WriteTextAsync(userJsonFile, data);
		//				//_minderLogger.Information("after call " + "File Atributes" + userJsonFile.Attributes + data);
		//				//await FileIO.WriteTextAsync(userJsonFile, "\+"{"UserId":"daksh@gmail.com","Password":"ser_123","OppSettings":[{"SettingName":"Sitting Desk","IsSelected":true,"TimeStamp":{"CreatedAt":"","ModifiedAt":"","CreatedBy":"","ModifiedBy":"","IsActive":true,"IsDelete":false},"SettingDetails":{"BodyLocation":"T1","WidgetPosition":"Bottom Center","NotificationDevice":"Mackbook Pro","PostureSensitivity":50,"PostureDifficulty":50,"PostureSupport":50,"RotationSensitivity":0,"FullWidgetRotation":"None","IsfullWidgetPosture":false,"Opacity":50,"WidgetBackground":"None","IsDisplayTime":true,"SelectedHRSensor":"Auto On","IsDisplayHeartRate":true,"StressholdHeartRate":234}},{"SettingName":"Standing Desk","IsSelected":false,"TimeStamp":{"CreatedAt":"","ModifiedAt":"","CreatedBy":"","ModifiedBy":"","IsActive":true,"IsDelete":false},"SettingDetails":{"BodyLocation":"T1","WidgetPosition":"Bottom Center","NotificationDevice":"Mackbook Pro","PostureSensitivity":50,"PostureDifficulty":50,"PostureSupport":50,"RotationSensitivity":0,"FullWidgetRotation":"None","IsfullWidgetPosture":false,"Opacity":50,"WidgetBackground":"None","IsDisplayTime":true,"SelectedHRSensor":"Auto On","IsDisplayHeartRate":true,"StressholdHeartRate":234}},{"SettingName":"Walking","IsSelected":false,"TimeStamp":{"CreatedAt":"","ModifiedAt":"","CreatedBy":"","ModifiedBy":"","IsActive":true,"IsDelete":false},"SettingDetails":{"BodyLocation":"T1","WidgetPosition":"Bottom Center","NotificationDevice":"Mackbook Pro","PostureSensitivity":50,"PostureDifficulty":50,"PostureSupport":50,"RotationSensitivity":0,"FullWidgetRotation":"None","IsfullWidgetPosture":false,"Opacity":50,"WidgetBackground":"None","IsDisplayTime":true,"SelectedHRSensor":"Auto On","IsDisplayHeartRate":true,"StressholdHeartRate":234}},{"SettingName":"Driving","IsSelected":false,"TimeStamp":{"CreatedAt":"","ModifiedAt":"","CreatedBy":"","ModifiedBy":"","IsActive":true,"IsDelete":false},"SettingDetails":{"BodyLocation":"T1","WidgetPosition":"Bottom Center","NotificationDevice":"Mackbook Pro","PostureSensitivity":50,"PostureDifficulty":50,"PostureSupport":50,"RotationSensitivity":0,"FullWidgetRotation":"None","IsfullWidgetPosture":false,"Opacity":50,"WidgetBackground":"None","IsDisplayTime":true,"SelectedHRSensor":"Auto On","IsDisplayHeartRate":true,"StressholdHeartRate":234}},{"SettingName":"Piano Playing","IsSelected":false,"TimeStamp":{"CreatedAt":"","ModifiedAt":"","CreatedBy":"","ModifiedBy":"","IsActive":true,"IsDelete":false},"SettingDetails":{"BodyLocation":"T1","WidgetPosition":"Bottom Center","NotificationDevice":"Mackbook Pro","PostureSensitivity":50,"PostureDifficulty":50,"PostureSupport":50,"RotationSensitivity":0,"FullWidgetRotation":"None","IsfullWidgetPosture":false,"Opacity":50,"WidgetBackground":"None","IsDisplayTime":true,"SelectedHRSensor":"Auto On","IsDisplayHeartRate":true,"StressholdHeartRate":234}}],"UserSessions":null}");
		//			}
		//			else{ _minderLogger.Information("userdata not found for user"+fileName); }
		//		}

		//	}
		//	catch (Exception e)
		//	{
		//		Debug.WriteLine("Failure: " + e.Message);
		//		return;
		//	}
		//}

		//private async static Task<string> GetData(bool isInitialData)
		//{			
		//	if (isInitialData)
		//	{
		//		JsonDataModel minderDb = new JsonDataModel();
		//		//JsonObject jsonObject = new JsonObject();

		//		//var jsonArr = new JsonArray();
		//		//minderDb.OppSettings.ForEach(item =>
		//		//{
		//		//	jsonArr.Add(JsonValue.TryParse());
		//		//});
		//		//string jsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets//UserData.json")));
		//		return JsonConvert.SerializeObject(minderDb) ;
		//		//return jsonString;
		//		//return new JavaScriptSerializer().Serialize(minderDb);
		//	}
		//	else
		//	{

		//		return JsonConvert.SerializeObject(_userLocalAppData);
		//	}
		//}
		//public static void SetRoamingData(JsonDataModel userLocalAppData)
		//{
		//	//minderDb =	JsonConvert.DeserializeObject<MinderDb>(JsonConvert.SerializeObject(userLocalAppData));

		//}
		//private static JsonDataModel _userLocalAppData;
		//public async static Task StoreRoamingData(JsonDataModel userLocalAppData)
		//{
		//	_userLocalAppData = userLocalAppData;
		// await	CreateDataFile(userLocalAppData.UserId,false);
		//}
		//#endregion

	}
}
