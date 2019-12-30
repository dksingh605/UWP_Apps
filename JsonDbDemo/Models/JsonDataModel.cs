
using System.Collections.Generic;


namespace JsonDbDemo.Models
{
	public class JsonDataModel
	{
		public string UserId { get; set; } = "dk";
		public string Password { get; set; } = "pass";
		public List<Setting> Settings { get; set; } = new List<Setting>();
	}
	public class TimeStamp
	{
		public string CreatedAt { get; set; }
		public string ModifiedAt { get; set; }
		public string CreatedBy { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public bool IsDelete { get; set; }
	}

	public class SettingDetails
	{
		public string BodyLocation { get; set; }
		public string WidgetPosition { get; set; }
		public string NotificationDevice { get; set; }
		public int PostureSensitivity { get; set; }
		public int PostureDifficulty { get; set; }
		public int PostureSupport { get; set; }
		public int RotationSensitivity { get; set; }
		public string FullWidgetRotation { get; set; }
		public bool IsfullWidgetPosture { get; set; }
		public int Opacity { get; set; }
		public string WidgetBackground { get; set; }
		public bool IsDisplayTime { get; set; }
		public string SelectedHRSensor { get; set; }
		public bool IsDisplayHeartRate { get; set; }
		public int StressholdHeartRate { get; set; }
	}

	public class Setting
	{
		public string SettingName { get; set; }
		public bool IsSelected { get; set; }
		public TimeStamp TimeStamp { get; set; }
		public SettingDetails SettingDetails { get; set; } = new SettingDetails();
	}
	
}
