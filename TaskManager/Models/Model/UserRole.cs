using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            UserApp = new HashSet<UserApp>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Column("id_role")]
        public int IdRole { get; set; }

        [InverseProperty("IdRoleNavigation")]
        public ICollection<UserApp> UserApp { get; set; }
    }
}
