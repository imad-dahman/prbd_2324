using Azure;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.Model;

    

  public class Tricount : EntityBase<PridContext> {
    [Required]
    [Key] public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public DateTime Created_at { get; set; } = DateTime.Now;
    [Required]
    public virtual User CreatorId { get; set; }
    [Required]

    [ForeignKey(nameof(Creator))]
    public int Creator { get; set; }

    public virtual ICollection<User> Participants { get; set; } = new HashSet<User>();

    [InverseProperty(nameof(Operation.TricountId))]
    public virtual ICollection<Operation> TricountOperations { get; set; } = new HashSet<Operation>();


    [InverseProperty(nameof(Templates.IdTricount))]
    public virtual ICollection<Templates> TricountTemplates { get; set; } = new HashSet<Templates>();

    public Tricount(int id, string title, string description, int creator, DateTime? created_at = null) {
        Id = id;
        Title = title;
        Description = description;
        Created_at = created_at ?? DateTime.Now;
        Creator = creator;
    }

    public Tricount() { }

    public static IQueryable<Tricount> GetAll() {
        return Context.Tricounts.OrderByDescending(t => t.TricountOperations.Any() ? t.TricountOperations.Max(o => o.Operation_date) :
                            t.Created_at);
    }

    public static IQueryable<Tricount> GetTricountsByUser(int userId) {
        return Context.Tricounts.OrderByDescending(t => t.TricountOperations.Any() ? t.TricountOperations.Max(o => o.Operation_date) :
                            t.Created_at).Where(t=>t.Participants.Any(p=>p.Id==userId));
    }

    public  Double GetTotalExpenses() {
        var totalAmount = (from tricount in Context.Tricounts
                           where tricount.Id == this.Id
                           select Math.Round(tricount.TricountOperations.Sum(op => op.Amount), 2))
                    .FirstOrDefault();

        return totalAmount;
    }
    public static Tricount GetById(int id) {
        return Context.Tricounts.Find(id);
    }
    public   Double GetMyExpenses() {
        Double somme = 0;
        foreach (var operation in this.TricountOperations) {
            var sommeWeightQuery = (from repartition in operation.OperationRepartitions
                                    select  repartition.Weight).Sum();

            var weightQuery = (from repartition in operation.OperationRepartitions
                               where repartition.User == App.CurrentUser.Id
                               select repartition.Weight).FirstOrDefault();

                Double sommeWeight = sommeWeightQuery;
                Double weight = weightQuery;
                Double amount = operation.Amount;

                Double resultat = amount * weight / sommeWeight;
                somme += resultat;
            
        }

        return Math.Round(somme,2);
    }

    public  Double GetMyBalance() {
        var totalAmount = 0.0;
                totalAmount = (from operation in this.TricountOperations
                                where operation.Initiator == App.CurrentUser.Id
                                select operation.Amount).Sum();
            
        

        return Math.Round(totalAmount - GetMyExpenses(),2);
    }

    public  Double GetBalanceByUser(int idUser) {
        var totalAmount = 0.0;
        totalAmount = (from operation in this.TricountOperations
                       where operation.Initiator == idUser
                       select operation.Amount).Sum();



        return Math.Round(totalAmount - GetUserExpenses(idUser), 2);
    }

    public Double GetUserExpenses(int idUser) {
        Double somme = 0;
        foreach (var operation in this.TricountOperations) {
            var sommeWeightQuery = (from repartition in operation.OperationRepartitions
                                    select repartition.Weight).Sum();

            var weightQuery = (from repartition in operation.OperationRepartitions
                               where repartition.User == idUser
                               select repartition.Weight).FirstOrDefault();

            Double sommeWeight = sommeWeightQuery;
            Double weight = weightQuery;
            Double amount = operation.Amount;

            Double resultat = amount * weight / sommeWeight;
            somme += resultat;

        }

        return Math.Round(somme, 2);
    }
    public int GetNumberOfOperationsForUser(int userId) {
        var userOperations = new HashSet<int>();

        var participantOperations = TricountOperations
            .Where(to => to.OperationRepartitions.Any(or => or.UserId.Id == userId))
            .Select(to => to.Id);

        foreach (var operationId in participantOperations) {
            userOperations.Add(operationId);
        }

        var initiatorOperations = TricountOperations
            .Where(to => to.Initiator == userId)
            .Select(to => to.Id);

        foreach (var operationId in initiatorOperations) {
            userOperations.Add(operationId);
        }

        return userOperations.Count;
    }
    public static IQueryable<Tricount> GetFiltered(string Filter) {
        var filtered = from t in Context.Tricounts
                       where
                       t.Title.Contains(Filter) 
                       ||
                       t.Description.Contains(Filter)
                       ||
                       t.CreatorId.Full_name.Contains(Filter)
                       ||
                       t.Participants.Any(p=>p.Full_name == Filter)
                       ||
                       t.TricountOperations.Any(o=>o.Title.StartsWith(Filter))
                       ||
                       t.Participants.Any(p => p.Email.Contains(Filter))
                       orderby t.TricountOperations.Any() ?
                            t.TricountOperations.Max(o => o.Operation_date) :
                            t.Created_at descending
                       select t;

        return filtered;
    }

    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Title))
            AddError(nameof(Title), "required");
        else if (Title.Length < 3)
            AddError(nameof(Title), "length must be >= 3");
        //else
        //    // On ne vérifie l'unicité du pseudo que si l'entité est en mode détaché ou ajouté, car
        //    // dans ces cas-là, il s'agit d'un nouveau membre.
        if (App.CurrentUser != null && this != null) {
            if ((IsDetached || IsAdded || IsModified) && Context.Tricounts.Where(t => t.Creator == App.CurrentUser.Id).Any(t =>
               t.Title==Title && t.Id !=Id))
            AddError(nameof(Title), "Title already exists");
        if (!string.IsNullOrEmpty(Description) && Description.Length <3)
            AddError(nameof(Description), "length must be >= 3");
        }


        return !HasErrors;
    }

    public Operation getRecentOperation() {

        return this.TricountOperations.OrderBy(to => to.Operation_date).FirstOrDefault();

    }

    public Operation getLastOperation() {
        if (this.TricountOperations.Count()>0)
        return this.TricountOperations.OrderBy(to => to.Operation_date).Last();
        else
            return null;   
    }
    public void Delete() {
        Participants.Clear();
        TricountOperations.Clear();
        TricountTemplates.Clear();
        // Supprime le membre lui-même
        Context.Tricounts.Remove(this);
        Context.SaveChanges();
    }
    public override string ToString() {
        return $"<Member: Pseudo={Title}, " +
            $"#Creator={Creator}, " +
            $"#Description={Description}, " +
            $"#Created_At={Created_at}, " +
            $"#CreatorId={CreatorId}>";
    }
  


}



