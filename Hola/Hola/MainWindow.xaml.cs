using System;
using System.Data.Entity;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hola
{
	public partial class MainWindow : Window
	{
		static bool isConnected = false;
		static bool receptionIsWorked = false;
		static Contact currentContact;
		static string userName;
		ApplicationContext db;

		public MainWindow()
		{
			InitializeComponent();
			ResetInputData();

			db = new ApplicationContext();
			db.Contacts.Load();
			DataContext = db.Contacts.Local.ToBindingList();
		}

		private void ConnectUser()
		{
			try
			{
				if (tbUserName.Text == string.Empty || tbRemoteAddress.Text == string.Empty)
					throw new ArgumentNullException();
				currentContact = new Contact(tbRemotePort.Text, tbLocalPort.Text, tbRemoteAddress.Text);
				userName = tbUserName.Text;

				btnConDiscon.Content = "Disconnect";
				isConnected = true;
				btnSend.IsEnabled = true;
				tbMessage.IsEnabled = true;
				lbChat.Items.Add($"Hi {userName}! You joined to chat!");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
				SendMessage("joined to chat");

				Task receiveTask = new Task(ReceiveMessages);
				receiveTask.Start();
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
				SendMessage("leaved to chat");
				ResetInputData();
				btnConDiscon.Content = "Connect";
				isConnected = false;
				btnSend.IsEnabled = false;
				tbMessage.IsEnabled = false;
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

		private void SendMessage(string msg)
		{
			UdpClient sender = null;
			try
			{
				sender = new UdpClient();
				byte[] buffer = Encoding.Unicode.GetBytes($"{userName}: {msg}");
				sender.Send(buffer, buffer.Length, currentContact.RemoteAddress, currentContact.RemotePort);
				if (tbMessage.Text != "")
					lbChat.Items.Add($"You: {tbMessage.Text}");
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			catch (Exception ex)
			{
				lbChat.Items.Add(ex.Message);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			finally
			{
				if (sender != null)
					sender.Close();
			}
		}

		private delegate void ReceiveMessagesOutHandler(string msg);

		private void ReciveMessagesOut(string msg)
		{
			if (Dispatcher.Thread == Thread.CurrentThread)
			{
				lbChat.Items.Add(msg);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
			else
			{
				Dispatcher.Invoke(new ReceiveMessagesOutHandler(ReciveMessagesOut), new object[] { msg });
			}
		}

		private void ReceiveMessages()
		{
			ReciveMessagesOut("Receiver messages activated!");
			UdpClient receiver = null;
			receptionIsWorked = true;
			try
			{
				receiver = new UdpClient(currentContact.LocalPort);
				IPEndPoint remoteIp = null;

				while (receptionIsWorked)
				{
					byte[] buffer = receiver.Receive(ref remoteIp);
					string msg = Encoding.Unicode.GetString(buffer);
					ReciveMessagesOut(msg);
				}
			}
			catch (Exception ex)
			{
				ReciveMessagesOut(ex.Message);
			}
			finally
			{
				if (receiver != null)
					receiver.Close();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			receptionIsWorked = false;
		}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			if (tbMessage.Text != "")
				SendMessage(tbMessage.Text);
			tbMessage.Text = String.Empty;
		}

		private void tbMessage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SendMessage(tbMessage.Text);
				tbMessage.Text = String.Empty;
			}
		}

		private void btnAddContact_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Contact contact;
				ContactWindow contactWindow;
				if (currentContact != null)
					contactWindow = new ContactWindow(currentContact);
				else
					contactWindow = new ContactWindow(new Contact());
				if (contactWindow.ShowDialog() == true)
				{
					contact = contactWindow.Contact;
					db.Contacts.Add(contact);
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				ResetInputData();
				lbChat.Items.Add(ex.Message);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (lbContacts.SelectedItem == null) return;
				Contact contact = lbContacts.SelectedItem as Contact;

				ContactWindow contactWindow = new ContactWindow(new Contact
				{
					Id = contact.Id,
					Name = contact.Name,
					RemotePort = contact.RemotePort,
					LocalPort = contact.LocalPort,
					RemoteAddress = contact.RemoteAddress
				});

				if (contactWindow.ShowDialog() == true)
				{
					contact = db.Contacts.Find(contactWindow.Contact.Id);
					if (contact != null)
					{
						contact.Name = contactWindow.Contact.Name;
						contact.RemotePort = contactWindow.Contact.RemotePort;
						contact.LocalPort = contactWindow.Contact.LocalPort;
						contact.RemoteAddress = contactWindow.Contact.RemoteAddress;
						db.Entry(contact).State = EntityState.Modified;
						db.SaveChanges();
					}
				}
			}
			catch (Exception ex)
			{
				ResetInputData();
				lbChat.Items.Add(ex.Message);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (lbContacts.SelectedItem == null) return;
				Contact contact = lbContacts.SelectedItem as Contact;
				db.Contacts.Remove(contact);
				db.SaveChanges();
			}
			catch (Exception ex)
			{
				ResetInputData();
				lbChat.Items.Add(ex.Message);
				lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
			}
		}

		private void lbContacts_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var listBox = sender as ListBox;
			if (null == listBox)
			{
				return;
			}

			var point = e.GetPosition((UIElement)sender);

			VisualTreeHelper.HitTest(listBox, null, (hitTestResult) =>
			{
				var uiElement = hitTestResult.VisualHit as UIElement;

				while (null != uiElement)
				{
					var listBoxItem = uiElement as ListBoxItem;
					if (null != listBoxItem)
					{
						listBoxItem.IsSelected = true;
						return HitTestResultBehavior.Stop;
					}

					uiElement = VisualTreeHelper.GetParent(uiElement) as UIElement;
				}

				return HitTestResultBehavior.Continue;
			}, new PointHitTestParameters(point));
		}
	}
}
