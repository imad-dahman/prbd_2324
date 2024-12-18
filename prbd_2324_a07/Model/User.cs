using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace prbd_2324_a07.Model;

public enum Role {
    User = 0,
    Administrator = 1 
}
public class User : EntityBase<PridContext>
{
    [Required]
    [Key] public int Id { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Hashed_Password { get; set; }
    [Required]
    public string Full_name { get; set; }
    [Required]
    public Role Role { get; set; } = Role.User;

    [InverseProperty(nameof(Tricount.CreatorId))]
    public virtual ICollection<Tricount> CreatedTricounts { get; set; } = new HashSet<Tricount>();
    public virtual ICollection<Tricount> OwnerTricounts { get; set; } = new HashSet<Tricount>();


    [InverseProperty(nameof(Operation.InitiatorId))]
    public virtual ICollection<Operation> InitiatedOperations { get; set; } = new HashSet<Operation>();
    [InverseProperty(nameof(Repartition.UserId))]
    public virtual ICollection<Repartition> UserRepartitions { get; set; } = new HashSet<Repartition>();

    [InverseProperty(nameof(Template_items.UserIdt))]
    public virtual ICollection<Template_items> UserTemplate { get; set; } = new HashSet<Template_items>();
   

    //public int UserId { get; set; }

    public User(int id, string email, string hashed_password,string full_name) {
        Id = id;
        Email = email;
        Hashed_Password = hashed_password;
        Full_name = full_name;
    }
    public User() {
     Role = Role.User;
    }
    public static User GetUserByEmail(string email) {
        var user = Context.Users.FirstOrDefault(u => u.Email == email);
        return user;
    }
    public static void InsertUser(string fullName, string email, string password) {
            var user = new User {
                Full_name = fullName,
                Email = email,
                Hashed_Password = password
            };
            Context.Users.Add(user);
            Context.SaveChanges();

    }
    public static List<User> GetAllUserFullNames() {
        using (var context = new PridContext()) {
            var fullNames = context.Users.Select(u => u).ToList();
            return fullNames;
        }
    }

    public static User GetUserById(int iduser) {
        var user = Context.Users
                          .Where(u => u.Id == iduser)
                          .SingleOrDefault();

        return user;
    }




    public override string ToString() {
        return $"User{{Id={Id},Email ={Email} , Password={Hashed_Password},FullName={Full_name}}}";
    }
}