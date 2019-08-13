using System.Windows;

namespace Hola
{
	public partial class ContactWindow : Window
	{
		public Contact Contact { get; private set; }

		public ContactWindow(Contact c)
		{
			InitializeComponent();
			Contact = c;
			DataContext = Contact;
		}

		private void OK_Click(object sender, RoutedEventArgs e) => DataContext = true;
	}
}
