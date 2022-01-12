using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("C_Menu")]
    public class C_Req_Menu
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ControllerName { get; set; }

        [StringLength(50)]
        public string ActionName { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }

        public bool HasChild { get; set; }

        public bool IsSideMenuBar { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedUserId { get; set; }

        public bool IsDelete { get; set; }
    }
}