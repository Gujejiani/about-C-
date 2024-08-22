


using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data


{
    public class DataContextEF: DbContext {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config) {
            _config = config;
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserSalary> UserSalary { get; set; }

        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured){
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"), options=>{
                    options.EnableRetryOnFailure();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // to tell EF to use the schema name TutorialAppSchema
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            // to tell EF to use the table name Users
            modelBuilder.Entity<User>().ToTable("Users")
            .HasKey(u => u.UserId);


            // to tell EF to know primary key for UserSalary and UserJobInfo
            modelBuilder.Entity<UserSalary>()
                        .HasKey(u => u.UserId);
            
            // to tell EF to know primary key for UserJobInfo
            modelBuilder.Entity<UserJobInfo>()
                        .HasKey(u => u.UserId);
            
        }

    }
}