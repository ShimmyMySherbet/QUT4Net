namespace QUT4Net.Models
{
    public class UnitOutline
    {
        public string UnitCode { get; set; }
        public int CreditPoints { get; set; }
        public string[] Prerequisites { get; set; }
        public string[] Equivalents { get; set; }
        public string[] Antirequisites { get; set; }

        public string CoordinatorName { get; set; }
        public string CoordinatorEmail { get; set; }

        public int CSPStudentContribution { get; set; }
        public int DomesticTuitionUnitFee { get; set; }
        public int InternationalUnitFee { get; set; }
    }
}