using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hola
{
	public class Contact : INotifyPropertyChanged
	{
		public Contact(string remotePort, string localPort, string remoteAddress)
		{
			RemotePort = Int32.Parse(remotePort);
			LocalPort = Int32.Parse(localPort);
			RemoteAddress = remoteAddress;
		}

		private string name;
		private int remotePort;
		private int localPort;
		private string remoteAddress;

		public int Id { get; set; }

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

		public int RemotePort
		{
			get { return remotePort; }
			set
			{
				remotePort = value;
				OnPropertyChanged("RemotePort");
			}
		}

		public int LocalPort
		{
			get { return localPort; }
			set
			{
				localPort = value;
				OnPropertyChanged("LocalPort");
			}
		}

		public string RemoteAddress
		{
			get { return remoteAddress; }
			set
			{
				remoteAddress = value;
				OnPropertyChanged("RemoteAddress");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "") 
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}
}
