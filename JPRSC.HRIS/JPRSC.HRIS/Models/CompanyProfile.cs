using System;

namespace JPRSC.HRIS.Models
{
    public class CompanyProfile
    {
        public DateTime AddedOn { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Signatory { get; set; }
    }
}