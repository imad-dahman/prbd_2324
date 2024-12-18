using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using PRBD_Framework;


namespace prbd_2324_a07.Model
{
    

    public class Subscriptions : EntityBase<PridContext>
    {


        [System.ComponentModel.DataAnnotations.Key]
        [Required, ForeignKey(nameof(UserIds))]
        public int User { get; set; }
        public virtual User UserIds { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        [Required, ForeignKey(nameof(ParticipantTricount))]
        public int TricountId { get; set; }
        public virtual Tricount ParticipantTricount { get; set; }

        public Subscriptions(int user , int tricount) {
            User = user;
            TricountId = tricount;
        }

        public Subscriptions() { }

    }
}
