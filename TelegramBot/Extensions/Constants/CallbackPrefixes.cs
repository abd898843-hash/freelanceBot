using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TelegramBot.Constants
{
    
    public enum ReportType { Daily, Weekly, FollowUp }

  
    public static class CallbackPrefixes
    {
        public const string ReportProject = "report_project";
        public const string ReportType = "report_type";
        public const string BackToReports = "back_to_reports";
    }
}
