using System;
using System.Windows;

namespace Hola
{
	public partial class MainWindow : Window
	{
		static bool isConnected = false;
		static bool receptionIsWorked = false;
		static Contact currentContact;
		static string userName;

		public MainWindow()
		{
			InitializeComponent();
			ResetInputData();
		}

		private void ConnectUser()
		{
			try
			{
				currentContact = new Contact(tbRemotePort.Text, tbLocalPort.Text, tbRemoteAddress.Text);
				if (tbUserName.Text == string.Empty)
					throw new ArgumentNullException();
				userName = tbUserName.Text;

				btnConDiscon.Content = "Disconnect";
				isConnected = true;
				btnSend.IsEnabled = true;
				tbMessage.IsEnabled = true;
				btnAddContact.IsEnabled = true;
				lbChat.Items.Add($"Hi {userName}! You joined to chat!");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
				//Send message about new connect

				//Run receive task
			}
			catch (ArgumentNullException)
			{
				ResetInputData();
				lbChat.Items.Add("Fields 'your name' and 'ip for send' must be filled!");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			catch (FormatException)
			{
				ResetInputData();
				lbChat.Items.Add("Fields 'port for send' and 'port for receive' must be filled with numbers!");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			catch (Exception ex)
			{
				ResetInputData();
				lbChat.Items.Add(ex.Message);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
		}

		private void ResetInputData()
		{
			currentContact = null;
			tbRemotePort.Text = "Enter port";
			tbLocalPort.Text = "Enter port";
			string[] comandLineArgs = Environment.GetCommandLineArgs();
			if (comandLineArgs.Length != 1)
				tbRemoteAddress.Text = comandLineArgs[1];
			userName = String.Empty;
			tbUserName.Text = "Enter name";

		}

		private void DisconnectUser()
		{
			try
			{
				//Send message about disconnect
				ResetInputData();
				btnConDiscon.Content = "Connect";
				isConnected = false;
				btnSend.IsEnabled = false;
				tbMessage.IsEnabled = false;
				btnAddContact.IsEnabled = false;
				receptionIsWorked = false;
				lbChat.Items.Add("Diconnected!");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			catch (Exception ex)
			{
				lbChat.Items.Add(ex.Message);
			}
		}

		private void btnConDiscon_Click(object sender, RoutedEventArgs e)
		{
			if (!isConnected)
				ConnectUser();
			else
				DisconnectUser();
		}
	}
}
