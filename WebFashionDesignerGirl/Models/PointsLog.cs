namespace WebFashionDesignerGirl.Models
{
    public class PointsLog
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int Amount { get; set; } // Kaç puan geldi/gitti?
        public string Reason { get; set; } // Neden?
    }
}