using JsonDbDemo.Models;
using JsonDbDemo.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JsonDbDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
		
		
		private async void btn_getJsonData_Click(object sender, RoutedEventArgs e)
		{
			var f = await JsonAsDbRepository.GetUserData("stephan");
		
			Debug.WriteLine(f.ToString());
			this.ShowMessage(f.ToString());
		}

		private async void btn_saveInLocalGet_Click(object sender, RoutedEventArgs e)
		{
			App.userLocalAppData= await JsonAsDbRepository.GetUserSavedData("stephan");
			ShowMessage(App.userLocalAppData.UserId);
		}

		private async void ShowMessage(string data)
		{
			var dialog = new MessageDialog(data);
			dialog.Title = "Saved Json Data";
			await dialog.ShowAsync();
		}

		private async void btn_saveNewData_Click(object sender, RoutedEventArgs e)
		{
			string newText = txtUserId.Text.ToString() ?? "stephan";
			App.userLocalAppData.UserId = newText;
			await JsonAsDbRepository.UpdateJsonFile(App.userLocalAppData);

			btn_saveInLocalGet.Content = "Get New Data";
			btn_getJsonData.Content = "Get New Json Data";
		}
	}
}
