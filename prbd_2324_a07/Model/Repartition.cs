using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;


namespace prbd_2324_a07.Model;

    public class Repartition : EntityBase<PridContext>
{
    [Required]
    public int Weight { get; set; }
    [Key]
    [Required , ForeignKey(nameof(OperationId))]
    public int Operation { get; set; }
    public virtual Operation OperationId { get; set; }
    [Key]
    [Required , ForeignKey(nameof(UserId))]
    public int User { get; set; } // Foreign key for User
    public virtual User UserId { get; set; }

    public Repartition( int operation, int user , int weight) {
        Operation = operation;
        User = user;
        Weight = weight;
    }
    public Repartition() { }





}

