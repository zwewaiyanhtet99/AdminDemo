using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class CopUserRegVM
    {
        #region Main

        public int Id { get; set; }

        public string USERID { get; set; }
        public int? CorpID { get; set; }
        public string CorporateID { get; set; }

        public string CompanyName { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }

        public string Address { get; set; }

        public int? PositionId { get; set; }

        public int? DepartmentId { get; set; }

        public int CorporateUserId { get; set; }

        public string CIFID { get; set; }

        #endregion

        #region Transaction

        public decimal MakerTranLimit { get; set; }

        public decimal CheckerTranLimit { get; set; }

        public decimal MakerBulkTranLimit { get; set; }

        public decimal CheckerBulkTranLimit { get; set; }

        #endregion

        #region Role

        public bool IsMaker { get; set; }

        public bool IsViewer { get; set; }

        public bool IsApprover { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsTransaction { get; set; }
        public List<int> Userroles { get; set; }
        public bool IsAdminMaker { get; set; }
        public bool IsAdminApprover { get; set; }

        #endregion

        #region Other
        //public DateTime RequestedDate { get; set; }
        //public DateTime CheckedDate { get; set; }
        //public string CheckedReason { get; set; }
        //public byte STATUS { get; set; }
        //public string Position { get; set; }
        //public string Department { get; set; }
        //public Boolean ISLOCK_FLAG { get; set; }
        //public Boolean ISTRANLOCK_FLAG { get; set; }

        #endregion
    }
}