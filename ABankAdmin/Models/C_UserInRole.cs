using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_UserInRole")]
    public class C_UserInRole
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("RoleId")]
        public virtual C_Role VrCRole { get; set;}
}
}