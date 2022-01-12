using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class HolidayVM
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Holiday Name")]
        public string HolidayName { get; set; }
        [Required]
        [Display(Name = "Holiday Date")]
        public string HolidayDate { get; set; }
    }

    public class HolidayList
    {
        public List<Holidays> Holidays { get; set; } = new List<Holidays>();
        public FullCalendarModel FullCalendar { get; set; } = new FullCalendarModel();
    }

    public class Holidays
    {
        public int ID { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
        public int FinancialYear { get; set; }
    }

    public class FullCalendarModel
    {
        public List<EventList> EventLists { get; set; } = new List<EventList>();
        public ValidRange validRange { get; set; } = new ValidRange();
    }

    public class EventList
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public int groupId { get; set; }
        public string url { get; set; }
        public HolidayextendedProps extendedProps { get; set; } = new HolidayextendedProps();
    }

    public class ValidRange
    {
        public string start { get; set; }
        public string end { get; set; }
        public string initialdate { get; set; }
    }

    public class HolidayextendedProps
    {
        public int ID { get; set; }
        public int FinancialYear { get; set; }
    }

    public class HolidayCommonRequestModel
    {
        public int FinancialYear { get; set; }
    }
}