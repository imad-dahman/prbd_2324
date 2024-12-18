using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.Model
{
    public class Template_items:EntityBase<PridContext>
    {
        [Required, ForeignKey(nameof(TemplatesId))]
        public int Template { get; set; }
        [Required]
        public int Weight { get; set; }

        [Required, ForeignKey(nameof(UserIdt))]

        public int User { get; set; }

        public virtual Templates TemplatesId { get; set; }
        public virtual User UserIdt { get; set; }


        public Template_items(int template, int user, int weight) {
            Template = template;
            User = user;
            Weight = weight;
        }
        public Template_items() { }
    }
}
