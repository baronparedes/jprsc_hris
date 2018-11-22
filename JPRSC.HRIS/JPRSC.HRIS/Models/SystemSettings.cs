namespace JPRSC.HRIS.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public decimal? MinimumDeductionOfContribution { get; set; }
        public decimal? MinimumNetPay { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public string TestEmailAddress { get; set; }
        public bool? EnableSendingEmails { get; set; }
    }
}