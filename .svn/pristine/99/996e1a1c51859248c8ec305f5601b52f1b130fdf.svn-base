using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_Menu")]
    public class C_Menu
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        [Required]
        [StringLength(35)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string MenuType { get; set; }

        [Required]
        [StringLength(30)]
        public string ControllerName { get; set; }

        [Required]
        [StringLength(30)]
        public string ActionName { get; set; }

        [StringLength(20)]
        public string Icon { get; set; }

        public bool HasChild { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedUserId { get; set; }

        public bool IsDelete { get; set; }
    }
}