using System.Windows;

namespace Hola
{
	public partial class EnterNameWindow : Window
	{
		public string UserName { get; private set; }

		public EnterNameWindow(string name)
		{
			InitializeComponent();
			tbName.Text = name;
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			UserName = tbName.Text;
			DialogResult = true;
		}

		private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
	}
}
