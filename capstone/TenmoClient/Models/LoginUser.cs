namespace TenmoClient.Models
{
    /// <summary>
    /// Model to provide login parameters
    /// </summary>
    public class User
    {
        internal static object Identity;

        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
    }

    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
