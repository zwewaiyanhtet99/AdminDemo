using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABankAdmin.Models
{
    [Table("C_CorporateTranRule")]
    public class C_CorporateTranRule
    {
        public int Id { get; set; }

        public int CorporateId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public decimal Value { get; set; }

        public bool IsDelete { get; set; }

        public string RuleCode { get; set; }
}
}