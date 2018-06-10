using System;

namespace JPRSC.HRIS.Models
{
    public class Bank
    {
        public string AccountNumber { get; set; }
        public DateTime AddedOn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string BatchNumber { get; set; }
        public string BranchCode { get; set; }
        public string Code { get; set; }
        public string CompanyCode { get; set; }
        public string ContactPerson { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public string OtherBankInfo { get; set; }
        public string Position { get; set; }
    }
}