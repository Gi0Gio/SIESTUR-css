namespace Turnero.Models
{
    public class Window
    {
        public int Id { get; set; }
        public string WindowName { get; set; }
        public int UserId { get; set; }
    }

    public class WindowDTO
    {
        public int Id { get; set; }
        public string WindowName { get; set; }
        public int UserId { get; set; }
    }
    public class WindowDTOCreate
    {
        public string WindowName { get; set; }
        // UserId is not needed, it will be set initially 0 and then updated by the user
    }
    public class WindowUpdateUserDto
    {
        public int UserId { get; set; }
    }
    public class UserWindowRegisterDTO
    {
        public int UserId { get; set; }
        public int WindowId { get; set; }
    }
}
