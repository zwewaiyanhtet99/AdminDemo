using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_UserTranLimit")]
    public class C_UserTranLimit
    {
        public int Id { get; set; }

        public int CorporateId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(20)]
        public string RuleCode { get; set; }

        public decimal Value { get; set; }

        public bool IsDelete { get; set; }

        [ForeignKey("CorporateId")]
        public virtual C_Corporate VrCorporate { get; set; }
    }
}