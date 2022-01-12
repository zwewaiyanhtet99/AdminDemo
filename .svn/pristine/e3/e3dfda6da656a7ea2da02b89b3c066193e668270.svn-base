using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class Biller_FieldVM
    {
        public int ID { get; set; }

        public int Biller_Id { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string LableName { get; set; }
        public string LableNameMM { get; set; }
        public string DefaultValue { get; set; }
        public string Placeholder { get; set; }

        public Attributes Attributes { get; set; }
        public int? ParentId { get; set; }

        public bool IsOutput { get; set; }
        public bool IsInput { get; set; }
        public bool IsHidden { get; set; }
        public int? SortOrder { get; set; }
        public IList<Biller_FieldVM> Children { get; set; }
    }

    public class Attributes
    {
        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public bool? Required { get; set; }
    }

    public class AttributesConvert
    {
        public int? maximum_length { get; set; }

        public int? minimum_length { get; set; }

        public bool? required { get; set; }
    }
}