using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models.Crypto;

namespace TaskManager.Models
{
    public partial class UserApp
    {
        public UserApp()
        {
            BoardTask = new HashSet<BoardTask>();
        }


        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("id_role")]
        public int IdRole { get; set; }
        [Column("id_java")]
        public long? IdJava { get; set; }
        [Required]
        [Column("first_name")]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [Column("upassword")]
        [StringLength(100)]
        public string Upassword { get; set; }
      
        [Column("date_java", TypeName = "datetime")]
        public DateTime? DateJava { get; set; }
        [Required]
        [Column("date_app", TypeName = "datetime")]
        public DateTime DateApp { get; set; }
        [Column("group_status")]
        public bool? GroupStatus { get; set; }


        [Column("emailconfirmed")]
        public bool EmailConfirmed { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        [ForeignKey("IdRole")]
        [InverseProperty("UserApp")]
        public UserRole IdRoleNavigation { get; set; }
        [InverseProperty("IdOwnerNavigation")]
        public ICollection<BoardTask> BoardTask { get; set; }
    }
}
