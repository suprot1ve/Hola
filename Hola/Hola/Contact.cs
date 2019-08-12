using System;
using System.Net;

namespace Hola
{
	public class Contact
	{
		public Contact(string remotePort, string localPort, string remoteAddress)
		{
			RemotePort = Int32.Parse(remotePort);
			LocalPort = Int32.Parse(localPort);
			RemoteAddress = IPAddress.Parse(remoteAddress);
		}

		protected internal int RemotePort { get; private set; }
		protected internal int LocalPort { get; private set; }
		protected internal IPAddress RemoteAddress { get; private set; }
	}
}
