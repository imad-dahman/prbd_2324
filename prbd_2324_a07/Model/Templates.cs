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
    public class Templates : EntityBase<PridContext>
    {

        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Tricount { get; set; }
        [ForeignKey(nameof(Tricount))]

        public virtual Tricount IdTricount { get; set; }
        [InverseProperty(nameof(Template_items.TemplatesId))]
        public virtual ICollection<Template_items> Templatetemplate { get; set; } = new HashSet<Template_items>();
        public Templates(int id, string title, int tricount) {
            Id = id;
            Title = title;
            Tricount = tricount;
        }
        public Templates() { }

    }
}
