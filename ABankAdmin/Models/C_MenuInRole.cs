using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_MenuInRole")]
    public class C_MenuInRole
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }
    }
}