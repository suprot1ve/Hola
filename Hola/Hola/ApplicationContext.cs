using System.Data.Entity;

namespace Hola
{
	class ApplicationContext : DbContext
	{
		public ApplicationContext() : base("DefaultConnection")
		{
		}
		public DbSet<Contact> Contacts { get; set; }
	}
}
