﻿using System;

namespace JPRSC.HRIS.Models
{
    public class EarningDeduction
    {
        public DateTime AddedOn { get; set; }
        public string Code { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Description { get; set; }
        public EarningDeductionType? EarningDeductionType { get; set; }
        public int Id { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}