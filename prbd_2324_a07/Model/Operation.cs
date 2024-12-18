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

    public class Operation : EntityBase<PridContext>
    {
    [Required]
    [Key] public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public int Tricount { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public DateTime Operation_date { get; set; } = DateTime.Now;
    
    [Required]
    public int Initiator {  get; set; }
    [ForeignKey(nameof(Initiator))]
    public virtual User InitiatorId { get; set; }
    [ForeignKey(nameof(Tricount))]
    public virtual Tricount TricountId { get; set; }

    [InverseProperty(nameof(Repartition.OperationId))]
    public virtual ICollection<Repartition> OperationRepartitions { get; set; } = new HashSet<Repartition>();

    public Operation(int id, string title, int tricount, double amount, DateTime operation_date , int initiator) {
        Id = id;
        Title = title;
        Tricount = tricount;
        Amount = amount;
        Operation_date = operation_date;
        Initiator = initiator;
       
    }
    public Operation() { }


    public static IQueryable<Operation> GetAll() {
        return Context.Operations
                       .OrderByDescending(o => o.Operation_date)
                       .ThenBy(o => o.Title);
    }
    public static IQueryable<Operation> GetAlll() {
        return Context.Operations
                       .OrderByDescending(o => o.InitiatorId.Full_name);
    }
    





}

