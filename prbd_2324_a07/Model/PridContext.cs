using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System.Configuration;

namespace prbd_2324_a07.Model;

public class PridContext : DbContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        /*
         * SQLite
         */

        // var connectionString = ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
        // optionsBuilder.UseSqlite(connectionString);

        /*
         * SQL Server
         */

        var connectionString = ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString;
        optionsBuilder.UseSqlServer(connectionString);

        ConfigureOptions(optionsBuilder);
    }

    private static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information) // permet de visualiser les requêtes SQL générées par LINQ
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors() // attention : ralentit les requêtes
            ;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Configuration de la hiérarchie de discrétion pour la classe User
        modelBuilder.Entity<User>()
            .HasDiscriminator(m => m.Role)
            .HasValue<User>(Role.User)
            .HasValue<Administrator>(Role.Administrator);

        // Relation entre User et Tricount
        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedTricounts)
            .WithOne(t => t.CreatorId)
            .HasForeignKey(t => t.Creator);

        // Relation entre User et Operation (initiateur)
        modelBuilder.Entity<Operation>()
            .HasOne(o => o.InitiatorId)
            .WithMany(u => u.InitiatedOperations)
            .HasForeignKey(o => o.Initiator)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Relation entre Operation et Tricount
        modelBuilder.Entity<Operation>()
            .HasOne(o => o.TricountId)
            .WithMany(t => t.TricountOperations)
            .HasForeignKey(o => o.Tricount)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Repartition>()
                .HasKey(r => new { r.Operation, r.User });

        // Relation entre  Repartition  et Operation 
        modelBuilder.Entity<Repartition>()
            .HasOne(r => r.OperationId)
            .WithMany(o => o.OperationRepartitions)
            .HasForeignKey(r => r.Operation);
        // Relation entre  Repartition et User
        modelBuilder.Entity<Repartition>()
            .HasOne(r => r.UserId)
            .WithMany(u => u.UserRepartitions)
            .HasForeignKey(r => r.User);

        modelBuilder.Entity<Subscriptions>()
                .HasKey(s => new { s.User, s.TricountId });


        //// Relation entre  Subscriptions et User
        //modelBuilder.Entity<Subscriptions>()
        //    .HasOne(r => r.UserIds)
        //    .WithMany(u => u.UserSubscripitions)
        //    .HasForeignKey(r => r.User)
        //    .OnDelete(DeleteBehavior.ClientCascade);
        //// Relation entre  Sub  et Tricount
        //modelBuilder.Entity<Subscriptions>()
        //    .HasOne(s => s.ParticipantTricount)
        //    .WithMany(t => t.TricountSubscriptions)
        //    .HasForeignKey(s => s.TricountId)
        //    .OnDelete(DeleteBehavior.ClientCascade);

        // Relation entre  Subscriptions et User

        modelBuilder.Entity<User>()
    .HasMany(u => u.OwnerTricounts)
    .WithMany(t => t.Participants)
    .UsingEntity<Subscriptions>(
        right => right.HasOne(s => s.ParticipantTricount).WithMany().HasForeignKey(nameof(Subscriptions.TricountId))
        .OnDelete(DeleteBehavior.ClientCascade),
        left => left.HasOne(s => s.UserIds).WithMany().HasForeignKey(nameof(Subscriptions.User))
        .OnDelete(DeleteBehavior.ClientCascade),
        joinEntity => {
            joinEntity.HasKey(s => new { s.User, s.TricountId });
        }
        );

        // Relation entre  Repartition et User
        modelBuilder.Entity<Templates>()
            .HasOne(r => r.IdTricount)
            .WithMany(u => u.TricountTemplates)
            .HasForeignKey(r => r.Tricount);


        modelBuilder.Entity<Template_items>()
               .HasKey(r => new { r.Template, r.User });
        // Relation entre  Template_items  et Template
        modelBuilder.Entity<Template_items>()
            .HasOne(r => r.TemplatesId)
            .WithMany(o => o.Templatetemplate)
            .HasForeignKey(r => r.Template)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Relation entre  Template_items et User
        modelBuilder.Entity<Template_items>()
            .HasOne(r => r.UserIdt)
            .WithMany(u => u.UserTemplate)
            .HasForeignKey(r => r.User)
            .OnDelete(DeleteBehavior.ClientCascade);


        // Ajout de la seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        int countId = 0;
        modelBuilder.Entity<User>().HasData(
            new User {
            Id = ++ countId,Email = "boverhaegen@epfc.eu",
                Hashed_Password = SecretHasher.Hash("Password1,"), Full_name = "Boris"
            },
            new User {
                Id = ++countId, Email = "bepenelle@epfc.eu",
                Hashed_Password = SecretHasher.Hash("Password1,"), Full_name = "Benoît"
            },
            new User {
                Id = ++countId, Email = "xapigeolet@epfc.eu",
                Hashed_Password = SecretHasher.Hash("Password1,"), Full_name = "Xavier"
            },
            new User {
                Id = ++countId, Email = "mamichel@epfc.eu",
                Hashed_Password = SecretHasher.Hash("Password1,"), Full_name = "Marc"
            }
            
            );
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = ++countId, Email = "admin@epfc.eu",
                Hashed_Password = SecretHasher.Hash("Password1,"), Full_name = "Administrator"
            }
            );
          modelBuilder.Entity<Tricount>().HasData(
            new Tricount {
                Id = 1, Title = "Gers 2023", Description = "", Created_at = DateTime.Parse("10-10-2023  18:42:24"), Creator = 1
            },
            new Tricount {
                Id = 2, Title = "Resto badminton", Description = "", Created_at = DateTime.Parse("10-10-2023  19:25:10"), Creator = 1
            },
            new Tricount {
                Id = 4, Title = "Vacances", Description = "A la mer du nord", Created_at = DateTime.Parse("10-10-2023  19:31:09"), Creator = 1
            },
            new Tricount {
                Id = 5, Title = "Grosse virée", Description = "A Torremolinos", Created_at = DateTime.Parse("15-08-2023  10:00:00"), Creator = 2
            },
            new Tricount {
                Id = 6, Title = "Torhout Werchter", Description = "Memorabile", Created_at = DateTime.Parse("02-06-2023  18:30:12"), Creator = 3
            }
            );
        int count = 0;
        modelBuilder.Entity<Operation>().HasData(
            new Operation {
                Id = ++count, Title = "Colruyt", Tricount = 4, Amount = 100, Operation_date = DateTime.Parse("13-10-2023"), Initiator = 2
            },
            new Operation {
                Id = ++count, Title = "Plein essence", Tricount = 4, Amount = 75, Operation_date = DateTime.Parse("13-10-2023"), Initiator = 1
            },
            new Operation {
                Id = ++count, Title = "Grosses courses LIDL", Tricount = 4, Amount = 212.47, Operation_date = DateTime.Parse("13-10-2023"), Initiator = 3
            },
            new Operation {
                Id = ++count, Title = "Apéros", Tricount = 4, Amount = 31.897456217, Operation_date = DateTime.Parse("13-10-2023"), Initiator = 1
            },
            new Operation {
                Id = ++count, Title = "Boucherie", Tricount = 4, Amount = 25.5, Operation_date = DateTime.Parse("26-10-2023"), Initiator = 2
            },
            new Operation {
                Id = ++count, Title = "Loterie", Tricount = 4, Amount = 35, Operation_date = DateTime.Parse("26-10-2023"), Initiator = 1
            },
            new Operation {
                Id = ++count, Title = "Sangria", Tricount = 5, Amount = 42, Operation_date = DateTime.Parse("16-08-2023"), Initiator = 2
            },
            new Operation {
                Id = ++count, Title = "Jet Ski", Tricount = 5, Amount = 250, Operation_date = DateTime.Parse("17-08-2023"), Initiator = 3
            },
            new Operation {
                Id = ++count, Title = "PV parking", Tricount = 5, Amount = 15.5, Operation_date = DateTime.Parse("16-08-2023"), Initiator = 3
            },
            new Operation {
                Id = ++count, Title = "Tickets", Tricount = 6, Amount = 220, Operation_date = DateTime.Parse("08-06-2023"), Initiator = 1
            },
            new Operation {
                Id = ++count, Title = "Décathlon", Tricount = 6, Amount = 199.99, Operation_date = DateTime.Parse("01-07-2023"), Initiator = 2
            }
            );
        modelBuilder.Entity<Repartition>().HasData(
            new Repartition {
                Operation = 1,User = 1,Weight = 1
            },
            new Repartition {
                Operation = 1, User = 2, Weight = 1
            },
            new Repartition {
                Operation = 2, User = 1, Weight = 1
            },
            new Repartition {
                Operation = 2, User = 2, Weight = 1
            },
            new Repartition {
                Operation = 3, User = 1, Weight = 2
            },
            new Repartition {
                Operation = 3, User = 2, Weight = 1
            },
            new Repartition {
                Operation = 3, User = 3, Weight = 1
            },
            new Repartition {
                Operation = 4, User = 1, Weight = 1
            },
            new Repartition {
                Operation = 4, User = 2, Weight = 2
            },
            new Repartition {
                Operation = 4, User = 3, Weight = 3
            },
            new Repartition {
                Operation = 5, User = 1, Weight = 2
            },
            new Repartition {
                Operation = 5, User = 2, Weight = 1
            },
            new Repartition {
                Operation= 5, User = 3, Weight = 1
            },
            new Repartition {
                Operation = 6, User = 1, Weight = 1
            },
            new Repartition {
                Operation = 6, User = 3, Weight = 1
            },
            new Repartition {
                Operation = 7, User = 2, Weight = 1
            },
            new Repartition {
                Operation = 7, User = 3, Weight = 2
            },
            new Repartition {
                Operation = 7, User = 4, Weight = 3
            },
            new Repartition {
                Operation = 8, User = 3, Weight = 2
            },
            new Repartition {
                Operation = 8, User = 4, Weight = 1
            },
            new Repartition {
                Operation = 9, User = 2, Weight = 1
            },
            new Repartition {
                Operation = 9, User = 4, Weight = 5
            },
            new Repartition {
                Operation = 10, User = 1, Weight = 1
            },
            new Repartition {
                Operation = 10, User = 3, Weight = 1
            },
            new Repartition {
                Operation = 11, User = 2, Weight = 2
            },
            new Repartition {
                Operation = 11, User = 4, Weight = 2
            }
            );
        modelBuilder.Entity<Subscriptions>().HasData(
      new Subscriptions { User = 1, TricountId = 1 },
      new Subscriptions { User = 1, TricountId = 2 },
      new Subscriptions { User = 1, TricountId = 4 },
      new Subscriptions { User = 1, TricountId = 6 },
      new Subscriptions { User = 2, TricountId = 2 },
      new Subscriptions { User = 2, TricountId = 4 },
      new Subscriptions { User = 2, TricountId = 5 },
      new Subscriptions { User = 2, TricountId = 6 },
      new Subscriptions { User = 3, TricountId = 4 },
      new Subscriptions { User = 3, TricountId = 5 },
      new Subscriptions { User = 3, TricountId = 6 },
      new Subscriptions { User = 4, TricountId = 4 },
      new Subscriptions { User = 4, TricountId = 5 },
      new Subscriptions { User = 4, TricountId = 6 }
  );
        modelBuilder.Entity<Templates>().HasData(
            new Templates {
                Id = 1, Title = "Boris paye double", Tricount = 4
            },
            new Templates {
                Id = 2, Title = "Benoît ne paye rien", Tricount = 4
            }
            );
        modelBuilder.Entity<Template_items>().HasData(
            new Template_items {
                Template = 1, User = 1, Weight = 2
            },
            new Template_items {
                Template = 1, User = 2, Weight = 1
            },
            new Template_items {
                Template = 1, User = 3, Weight = 1
            },
            new Template_items {
                Template = 2, User = 1, Weight = 1
            },
            new Template_items {
                Template = 2, User = 3, Weight = 1
            }
            
           );
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Tricount> Tricounts => Set<Tricount>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Repartition> Repartitions => Set<Repartition>();
    public DbSet<Subscriptions> subscriptions => Set<Subscriptions>();
    public DbSet<Templates> Templates => Set<Templates>();
    public DbSet<Template_items> Template_Items => Set<Template_items>();


}