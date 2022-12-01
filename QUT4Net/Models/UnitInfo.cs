namespace QUT4Net.Models
{
    public class UnitInfo
    {
        public string UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public string TeachingPeriod { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public string[] AttendanceModes { get; set; }
        public string OutlineURL { get; set; }
    }
}