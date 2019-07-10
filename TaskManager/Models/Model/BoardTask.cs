using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class BoardTask
    {
        public BoardTask()
        {
            StageTask = new HashSet<StageTask>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("id_owner")]
        public long  IdOwner { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        [Column("name_task")]
        [StringLength(100)]
        public string NameTask { get; set; }
        [Required(ErrorMessage = "Поле не заполнено")]
        [Column("content_task")]
        public string ContentTask { get; set; }
        [Required]
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Required]
        [Column("date_finished", TypeName = "datetime")]
        public DateTime DateFinished { get; set; }
        [Column("background_image")]
        public string BackgroundImage { get; set; }
        [Column("task_created")]
        public bool? TaskCreated { get; set; }

        [ForeignKey("IdOwner")]
        [InverseProperty("BoardTask")]
        public UserApp IdOwnerNavigation { get; set; }
        [InverseProperty("IdOwnerNavigation")]
        public ICollection<StageTask> StageTask { get; set; }
    }
}
