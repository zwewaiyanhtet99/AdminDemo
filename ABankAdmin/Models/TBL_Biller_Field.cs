using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ABankAdmin.Models
{
    [Table("TBL_Biller_Field")]
    public class TBL_Biller_Field
    {
        [Key]
        public int ID { get; set; }

        public int Biller_Id { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string LableName { get; set; }
        public string LableNameMM { get; set; }
        public string DefaultValue { get; set; }
        public string Placeholder { get; set; }
        public string Attributes { get; set; }
        public int? ParentId { get; set; }
        public bool IsOutput { get; set; }
        public bool IsInput { get; set; }
        public bool IsHidden { get; set; }
        public int? SortOrder { get; set; }
        public virtual TBL_Biller TBL_Biller { get; set; }
        public virtual TBL_Biller_Field Parent { get; set; }
        public virtual ICollection<TBL_Biller_Field> Children { get; set; }
    }
}