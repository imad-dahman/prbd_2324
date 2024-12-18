using prbd_2324_a07.Model;
using prbd_2324_a07.ViewModel;
using System.Windows;
using System.Globalization;
using PRBD_Framework;
using System.Threading.Channels;
using Azure;

namespace prbd_2324_a07;

public partial class App: ApplicationBase<User,PridContext> {

    public enum Messages
    {
        MSG_NEW_MEMBER,
        MSG_NEW_DETAIL,
        MSG_PSEUDO_CHANGED,
        MSG_MEMBER_CHANGED,
        MSG_OPERATION_CHANGED,
        MSG_DISPLAY_MEMBER,
        MSG_CLOSE_TAB,
        MSG_LOGIN,
        MSG_REFRESH_MESSAGES,
        MSG_LOGOUT,
        MSG_SIGNUP,
        NewOperation

    }

    public App() {
        var ci = new CultureInfo("fr-BE") {
            DateTimeFormat = {
                ShortDatePattern = "dd/MM/yyyy",
                DateSeparator = "/"
            }
        };
        CultureInfo.DefaultThreadCurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        TestQueries();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, PridContext>();
        });

        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, PridContext>();
        });
        Register(this, Messages.MSG_SIGNUP,()  => {
            
            NavigateTo<SignupViewModel, User, PridContext>();
        });



    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated(); 

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(1);
        Console.WriteLine("done");
    }

    protected override void OnRefreshData() {
        // TODO

       
    }

    private static void TestQueries() {
        // Un endroit pour tester vos requêtes LINQ

        var q1 = from user in Context.Users
                 where Context.Tricounts.Any(tricount => tricount.Title == "Vacances" && tricount.Creator == user.Id)
                 select user.Full_name;

        foreach (var m in q1) {
            Console.WriteLine(m);
        }
        var q2 = from m in Context.Tricounts
                 where m.Creator == 1
                 select m.Title;
        foreach (var m in q2) {
            Console.WriteLine(m);
        }
       


        var q5 = from o in Context.Operations
                 from u in Context.Users
                 from t in Context.Tricounts
                 where u.Id == o.Initiator && o.Tricount == t.Id
                 && o.Tricount == 6
                 select new {
                     Name = u.Full_name,
                     OperatinTitle = o.Title,
                     TricoTitle = t.Title
                 };
        foreach (var m in q5) {
            Console.WriteLine($"Name: {m.Name} , Operation Title : {m.OperatinTitle} , Tricount Title : {m.TricoTitle}");
        }

        var userId = 1/* ID de l'utilisateur dont vous voulez récupérer les tricounts */;
        var q6 = (from s in Context.subscriptions
                  join t in Context.Tricounts on s.TricountId equals t.Id
                  where s.User == userId
                  select t.Title)
             .Concat(
               from t in Context.Tricounts
               where t.Creator == userId  // Remplacez 'userId' par l'ID de l'utilisateur concerné
               select t.Title)
             .Distinct();

        foreach (var m in q6) {
            Console.WriteLine(m);
        }


        //var q7 = Tricount.GetTotalExpenses(6);
        //var q8 = Tricount.GetMyExpenses(6);
        //var q9 = Tricount.GetMyBalance(6);

        //Console.WriteLine(q7);
        //Console.WriteLine(q8);
        //Console.WriteLine(q9);


        var filteredMembers = from o in Context.Operations
                              where o.TricountId.TricountOperations.Any(o => o.Tricount == 4)
                              select o.Title;
        foreach (var m in filteredMembers) {
            Console.WriteLine(m);
        }
        var q3 = from m in Context.Tricounts
                 where Context.Users.Any(m2 => m2.Full_name == "Xavier" && m.Creator == m2.Id)
                 select m.Title;
        foreach (var m in q3) { Console.WriteLine(m); }


        var q9 = from m in Context.subscriptions
                 where m.ParticipantTricount.Id == 4
                 select m.UserIds.Full_name;
        foreach (var m in q9) { Console.WriteLine(m); }



    }


}