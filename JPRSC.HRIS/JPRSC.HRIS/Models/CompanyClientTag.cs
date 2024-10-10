namespace JPRSC.HRIS.Models
{
    public class CompanyClientTag
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public Client Client { get; set; }
        public Company Company { get; set; }
    }
}
