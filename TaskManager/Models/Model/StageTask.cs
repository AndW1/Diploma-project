using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public partial class StageTask
    {
        public StageTask()
        {
            NodeStage = new HashSet<NodeStage>();
        }

        [Column("id")]
        public long Id { get; set; }

       // [Required]
        [BindRequired]
        [Column("id_owner")]
        public long IdOwner { get; set; }
       
       // [Required]
        [BindNever]
        [Column("name_stage")]
        [StringLength(100)]
        public string NameStage { get; set; }
       
        //[Required]
        [Required(ErrorMessage = "Поле не заполнено")]
        [Column("content_stage")]
        public string ContentStage { get; set; }
      
       // [Required]
        [BindNever]
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
      
       // [Required]
        [Required]
        [Column("date_finished", TypeName = "datetime")]
        public DateTime DateFinished { get; set; }
       
        [Column("background_image")]
        public string BackgroundImage { get; set; }
       
        [Column("stage_created")]
        public bool? StageCreated { get; set; }

        [ForeignKey("IdOwner")]
        [InverseProperty("StageTask")]
        public BoardTask IdOwnerNavigation { get; set; }
        [InverseProperty("IdOwnerNavigation")]
        public ICollection<NodeStage> NodeStage { get; set; }
    }
}
