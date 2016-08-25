using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    public class UDebt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }

        public int UGroupId { get; set; }
        public UGroup UGroup { get; set; }

        public int LenderId { get; set; }
        public UMember Lender { get; set; }

        public int DebtorId { get; set; }
        public UMember Debtor { get; set; }

        [NotMapped]
        public string LenderName { get; set; }
        [NotMapped]
        public string DebtorName { get; set; }
    }
}
