using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.Data.Entities;

namespace SchoolWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Absence> Absences { get; set; }

        public DbSet<Class> Classes { get; set; }

        public DbSet<ClassStudent> ClassStudents { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseDiscipline> CourseDisciplines { get; set; }

        public DbSet<Discipline> Disciplines { get; set; }

        public DbSet<Evaluation> Evaluations { get; set; }

        public DbSet<Gender> Genders { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
