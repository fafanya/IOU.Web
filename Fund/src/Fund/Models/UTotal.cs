using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    [NotMapped]
    public class UTotal
    {
        public double Amount { get; set; }

        public string Text
        {
            get
            {
                return DebtorName + " -> " + LenderName + ": " + Amount;
            }
        }

        public string DebtorName { get; set; }
        public string LenderName { get; set; }
    }
}
