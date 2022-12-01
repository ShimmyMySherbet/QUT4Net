namespace QUT4Net.Models
{
    public class QUTUnit
    {
        public UnitInfo Info { get; set; }
        public UnitOutline Outline { get; set; }

        public QUTUnit(UnitInfo info, UnitOutline outline)
        {
            Info = info;
            Outline = outline;
        }
    }
}