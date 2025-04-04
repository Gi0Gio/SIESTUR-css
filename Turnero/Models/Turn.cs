namespace Turnero.Models
{
    public class Turn
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public DateTime TurnDate { get; set; }
    }

    public class TurnDTO
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public DateTime TurnDate { get; set; }
    }
    public class TurnDTOCreate
    {
        public int TurnNumber { get; set; }
    }
}
