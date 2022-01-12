using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class MenuVM
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string MenuType { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}