using System;
using System.Configuration;

namespace ABankAdmin.Core.Utils
{
    public class ConstantValues
    {
        //read-only variables
        //connection string
        public static string Connection_String { get { return ConfigurationManager.ConnectionStrings["AdminDBContext"].ConnectionString; } }

        //from config
        public static string Image_Path { get { return ConfigurationManager.AppSettings["Image_Path"]; } }

        //from config
        public static string CSVFile_Path { get { return ConfigurationManager.AppSettings["CSVFile_Path"]; } }
        public static string CSVIconicFile_Path { get { return ConfigurationManager.AppSettings["CSVIconicReportFile_Path"]; } }

        public static string CSVBulkFile_Path { get { return ConfigurationManager.AppSettings["CSVBulkFile_Path"]; } }

        public static string Image_Domain { get { return ConfigurationManager.AppSettings["Image_Domain"]; } }
        public static string HardCodeKey { get { return ConfigurationManager.AppSettings["HardCodeKey"]; } }
        public static string HardCodeIV { get { return ConfigurationManager.AppSettings["HardCodeIV"]; } }
        public static string APIEndPointURL { get { return ConfigurationManager.AppSettings["APIEndPointURL"]; } }

        //pdf path
        public static string Pdf_Path { get { return ConfigurationManager.AppSettings["Pdf_Path"]; } }

        //admin pdf path
        public static string AdminPdf_Path { get { return ConfigurationManager.AppSettings["AdminPdf_Path"]; } }

        //corporate pdf path
        public static string CorporatePdf_Path { get { return ConfigurationManager.AppSettings["CorporatePdf_Path"]; } }

        //CifInfo pdf path
        public static string CifPdf_Path { get { return ConfigurationManager.AppSettings["CifPdf_Path"]; } }
        public static string IconicMailRoute { get { return ConfigurationManager.AppSettings["URLForIconicEmail"]; } }

        //constant values
        public static string ADS_Path { get { return "\\Images\\ADS\\"; } }

        public static string Utility_Path { get { return "\\Images\\Bill_Payment\\"; } }
        public static string ExcelFile_Path { get { return ConfigurationManager.AppSettings["ExcelFile_Path"]; } }

        public static string Date_Format { get { return "dd-MM-yyyy"; } }
        public static string Date_TAndUReports { get { return "MM-dd-yyyy"; } }
        public static string Date_Time_Format { get { return "dd-MM-yyyy HH:mm:ss"; } }
        public static string Date_FormatForIconic { get { return "dd-MM-yy"; } }
        public static string DateTime_Format { get { return "dd-MM-yy"; } }

        //notification type ("Information")
        public static string NotiType { get { return "Information"; } }

        //new GUID
        public static string GetGUID() { return Guid.NewGuid().ToString(); }

        //image names

        //for register path, for reset path

        //for successful and error messages

        //User Type
        public const string Retail = "Retail";

        public const string Corporate = "Corporate";

        //id of 'Administrator' Role at TBL_ROLE
        public const int AdminRoleID = 3;

        //Iconic Cron
        public const string IconicCronlog = "ICONIC_CRONLOG";
        public const string IconicCronlogForWeekday = "ICONIC_CRONLOG_WEEKDAY";
        public const string IconicReportCron_Monthly = "ICONIC_CRONLOG_MONTHLY";

        public const string IconicReportCron = "ICONIC_REPORT";
        public const string IconicReportCronForWeekday = "ICONIC_REPORT_WEEKDAY";
        public const string IconicReportCronForMonthly = "ICONIC_REPORT_MONTHLY";
    }
}