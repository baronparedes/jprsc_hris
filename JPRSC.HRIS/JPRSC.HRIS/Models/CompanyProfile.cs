using System;
using System.Collections.Generic;

namespace JPRSC.HRIS.Models
{
    public class CompanyProfile
    {
        public DateTime AddedOn { get; set; }
        public string Address { get; set; }
        public string BOI { get; set; }
        public DateTime? DateIssued { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DTI { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }
        public string PagIbig { get; set; }
        public string PERAA { get; set; }
        public string PhilHealth { get; set; }
        public string Phone { get; set; }
        public string PlaceIssued { get; set; }
        public string Registration { get; set; }
        public string SEC { get; set; }
        public string SSS { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public string VAT { get; set; }
        public string ZipCode { get; set; }
    }
}