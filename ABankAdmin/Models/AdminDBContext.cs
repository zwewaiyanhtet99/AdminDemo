using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace ABankAdmin.Models
{
    public class AdminDBContext : IdentityDbContext<AdminUser>
    {
        public AdminDBContext()
            : base("name=AdminDBContext")
        {
            //this.Database.CommandTimeout = 600;
            Database.SetInitializer<AdminDBContext>(null);
        }

        public static AdminDBContext Create()
        {
            return new AdminDBContext();
        }

        public DbSet<Ver_sion> Versions { get; set; }     
        public DbSet<User> _Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Rule_> Rules { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        //public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Township> Townships { get; set; }
        public DbSet<FinacleLogin> FinacleLogins { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        //public DbSet<RemittanceRate> RemittanceRates { get; set; }
        public DbSet<ADS> ADSs { get; set; }
        public DbSet<ACC_Service> ACC_Services { get; set; }
        public DbSet<ACC_Service_Desc> ACC_Service_Descs { get; set; }
        //public DbSet<Tran_Log> Tran_Log { get; set; }
        public DbSet<UltType> UltTypes { get; set; }
        public DbSet<Office_Account> Office_Account { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> _Roles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Utilities> Utilities { get; set; }
        public DbSet<UtilitiesDetail> UtilitiesDetails { get; set; }
        
        //For maker checker
        public DbSet<ReqUser> ReqUsers { get; set; }
        public DbSet<ReqAcc> ReqAccs { get; set; }
        public DbSet<ReqChange> ReqChanges {get;set;}
        public DbSet<Login> Logins { get; set; }
        //public DbSet<Tran_Log> Tran_Logs { get; set; }
        public DbSet<FeedBack> Feedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Fingerprint> Fingerprints { get; set; }

        //public DbSet<FireBase> FireBases { get; set; }
        public DbSet<COMMENT> COMMENTs { get; set; }
        public DbSet<ContactUs> Contactus { get; set; }
        public DbSet<SMS_Allow> SMS_Allows { get; set; }
        public DbSet<SMS_Body> SMS_Bodys { get; set; }
        public DbSet<OtherBank> OtherBanks { get; set; }
        public DbSet<OtherBranch> OtherBranches { get; set; }
        public DbSet<IB_HowToUse> IB_HowToUses { get; set; }
        public DbSet<Api_Log> Api_Logs { get; set; }
        public DbSet<IB_Menu> IB_Menus { get; set; }
        public DbSet<Other_Bank_Recon> Other_Bank_Recons { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<ServiceManagement> ServiceManagements { get; set; }   
        public DbSet<CronLog> CronLogs { get; set; }
        public DbSet<Schedule_Tranfer> ScheduleTransfers { get; set; }
        public DbSet<ScheduleTransferLog> ScheduleTransferLogs { get; set; }
        public DbSet<OtherBankTranLog> OtherBankTranLogs { get; set; }
        public DbSet<OtherBankBeneficiary> OtherBankBeneficiarys { get; set; }
        public DbSet<Biller> Billers { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<CronLog_Iconic> CronLogs_Iconic { get; set; }

        #region Corporate

        //public DbSet<C_Country> Countries { get; set; }		
        //public DbSet<ApproveRule> ApproveRules { get; set; }
        //public DbSet<C_State> States { get; set; }
        //public DbSet<C_Position> Positions { get; set; }
        //public DbSet<Department> Departments { get; set; }
        //public DbSet<BulkFileRecord> BulkFileRecords { get; set; }
        //public virtual DbSet<CorporateRole> CorporateRoles { get; set; }
        //public virtual DbSet<CorporateRoleMenu> CorporateRoleMenus { get; set; }

        //req
        public virtual DbSet<C_Req_Corporate> C_Req_Corporates { get; set; }
        public virtual DbSet<C_Req_Changes> C_Req_Changes { get; set; }
        public virtual DbSet<C_Req_User> C_Req_Users { get; set; }
        public virtual DbSet<C_Req_UserInAccount> C_Req_UserInAccounts { get; set; }
        public virtual DbSet<C_Req_UserInRole> C_Req_UserInRoles { get; set; }
        public virtual DbSet<C_Req_UserTranLimit> C_Req_UserTranLimits { get; set; }
        public virtual DbSet<C_Req_MenuPermission> C_Req_MenuPermissions { get; set; }

        //def
        public virtual DbSet<C_Corporate> C_Corporates { get; set; }
        public virtual DbSet<C_UserInAccount> C_UserInAccounts { get; set; }
        public virtual DbSet<C_UserInRole> C_UserInRoles { get; set; }
        public virtual DbSet<C_Menu> C_Menus { get; set; }
        //public virtual DbSet<C_MenuInRole> C_MenuInRoles { get; set; }
        public virtual DbSet<C_MenuPermission> C_MenuPermissions { get; set; }
        public virtual DbSet<C_Role> C_Role { get; set; }
        public virtual DbSet<C_UserTranLimit> C_UserTranLimits { get; set; }
        public virtual DbSet<C_CorporateTranRule> C_CorporateTranRules { get; set; }
        public virtual DbSet<C_Country> C_Countries { get; set; }
        public virtual DbSet<C_Department> C_Departments { get; set; }
        public virtual DbSet<C_Position> C_Positions { get; set; }
        public virtual DbSet<C_State> C_States { get; set; }
        public virtual DbSet<C_Bulk_File_Record> C_Bulk_File_Records { get; set; }
        //public DbSet<C_Approve_Rule> C_Approve_Rules { get; set; }
        public DbSet<C_ApproverTranRule> C_ApproverTranRules { get; set; }
        public DbSet<C_ApproverTranRuleDetail> C_ApproverTranRuleDetails { get; set; }
        public virtual DbSet<C_BulkPaymentFileUpload> C_BulkPaymentFileUploads { get; set; }
        public virtual DbSet<C_MakerTranLog> C_MakerTranLogs { get; set; }
        public DbSet<C_Req_ApproverTranRule> C_Req_ApproverTranRules { get; set; }
        public DbSet<C_Req_ApproverTranRuleDetail> C_Req_ApproverTranRuleDetails { get; set; }
        #endregion

        #region Biller

        public virtual DbSet<TBL_Biller> TBL_Billers { get; set; }
        public virtual DbSet<TBL_Biller_Field> TBL_Biller_Fields { get; set; }

        #endregion Biller
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Branch>().Property(e => e.LATITUDE).HasPrecision(10, 8);
            modelBuilder.Entity<Branch>().Property(e => e.LONGITUDE).HasPrecision(11, 8);//to save decimal places 8 (not default 2 floating points)
            modelBuilder.Entity<Ver_sion>().Property(e => e.VersionNo).HasPrecision(18, 0);
            //branch relationship
            //modelBuilder.Entity<City>()
            //    .HasKey(c => c.Code);

            //modelBuilder.Entity<Branch>()
            //    .HasRequired(b => b._city)
            //    .WithMany(c => c.Branches)
            //    .HasForeignKey(b => b.CITY);
            modelBuilder.Entity<TBL_Biller_Field>()
                .HasRequired(b => b.TBL_Biller)
                .WithMany(c => c.TBL_Biller_Fields)
                .HasForeignKey(b => b.Biller_Id);

            modelBuilder.Entity<TBL_Biller_Field>()
                    .HasOptional(u => u.Parent)
                    .WithMany(u => u.Children)
                    .HasForeignKey(u => u.ParentId);
            //.WillCascadeOnDelete(true);
        }

        public class MyConfiguration : DbConfiguration
        {
            public MyConfiguration()
            {
                SetExecutionStrategy(
                    "System.Data.SqlClient",
                    () => new SqlAzureExecutionStrategy(3, TimeSpan.FromSeconds(5)));
            }
        }

        public System.Data.Entity.DbSet<ABankAdmin.ViewModels.ReqApvTranRuleAndDetailVM> ReqApvTranRuleAndDetailVMs { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.Models.UserTypeModel> UserTypeModels { get; set; }
        //public System.Data.Entity.DbSet<ABankAdmin.Models.UserTypeModelForTier> TierModels { get; set; }
        public System.Data.Entity.DbSet<ABankAdmin.Models.CIFInfoModel> CIFInfoModels { get; set; }
        public System.Data.Entity.DbSet<ABankAdmin.Models.Account_Info> Acct_InfoModels { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.ViewModels.CIFInfoVM> CIFInfoVMs { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.Models.IconicBookingModel> IconicBookingModels { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.Models.IconicBlacklistModel> IconicBlacklistModels { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.Models.RM_TL_Info> RM_TL_Info { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.Models.RM_Info> RM_Info { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.ViewModels.CIFInfoVMForDeactivator> CIFInfoVMForDeactivators { get; set; }
        public System.Data.Entity.DbSet<ABankAdmin.Models.Privilege> Privilege { get; set; }
        public System.Data.Entity.DbSet<ABankAdmin.Models.Privilege_Usage> PrivilegeUsage { get; set; }

        public System.Data.Entity.DbSet<ABankAdmin.ViewModels.PrivilegeVM> PrivilegeVMs { get; set; }
        public System.Data.Entity.DbSet<ABankAdmin.Models.CIFInfoModelForTemp> CIFInfoForTemp { get; set; }
    }

    public class AdminLogDBContext : IdentityDbContext<AdminUser>
    {
        public AdminLogDBContext() : base("name=AdminLogDBContext")
        {
            //this.Database.CommandTimeout = 600;
            Database.SetInitializer<AdminLogDBContext>(null);
        }

        public static AdminLogDBContext Create()
        {
            return new AdminLogDBContext();
        }

        public DbSet<Api_Log> Api_Logs { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }

      


    }


}