namespace Turnero.Models
{
    public class Attend
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public int UserId { get; set; }
        public int WindowId { get; set; }
        public DateTime AttendanceDate { get; set; }
    }

    public class AttendDTO
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public int UserId { get; set; }
        public int WindowId { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
    public class AttendDTOCreate
    {
        public int TurnNumber { get; set; }
        public int UserId { get; set; }
        public int WindowId { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
}
