using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public partial class NodeStage
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("id_owner")]
        public long IdOwner { get; set; }
        [Required]
        [Column("content_node")]
        public string ContentNode { get; set; }
        [Required]
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("date_finished", TypeName = "datetime")]
        public DateTime DateFinished { get; set; }
        [Column("path_file")]
        public string BackgroundImage { get; set; }
        [Column("node_created")]
        public bool? NodeCreated { get; set; }

        [ForeignKey("IdOwner")]
        [InverseProperty("NodeStage")]
        public StageTask IdOwnerNavigation { get; set; }
    }
}
