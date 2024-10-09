namespace AprendeMasWeb.Models
{
    public class EmailConfiguration
    {
        public required string From { get; set; }

        public required string SMTPServer { get; set; }

        public int Port { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}
