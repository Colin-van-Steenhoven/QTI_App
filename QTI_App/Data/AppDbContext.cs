using Microsoft.EntityFrameworkCore;
using QTI_App.Data.Seeders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Usb;
using Windows.System;
using Windows.UI;

namespace E4_The_Big_Three.Data
{
    class AppDbContext : DbContext
    {
        public DbSet<Answer> answers { get; set; }
        public DbSet<Question> questions { get; set; }
        public DbSet<QuestionTag> questionTags { get; set; }
        public DbSet<Tag> tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseMySql(
               ConfigurationManager.ConnectionStrings["QTI_App"].ConnectionString,
               ServerVersion.Parse("5.7.33-winx64"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionTagConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
        }
    }


}
