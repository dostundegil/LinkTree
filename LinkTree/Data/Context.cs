using LinkTree.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkTree.Data
{	//CONTEXT SINIFI
	public class Context : IdentityDbContext<AppUser,AppRole,int>
	{
		//CONNECTİON STRİNG
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("connectionstrgin");
		}

        public DbSet<Link> Links { get; set; }
    }
}
