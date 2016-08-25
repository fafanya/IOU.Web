using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    public class UMember
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UUserId { get; set; }
        public UUser UUser { get; set; }

        public int? UGroupId { get; set; }
        public UGroup UGroup { get; set; }

        [InverseProperty("Lender")]
        public List<UDebt> LenderUDebts { get; set; }

        [InverseProperty("Debtor")]
        public List<UDebt> DebtorUDebts { get; set; }

        public List<UPayment> UPayments { get; set; }
        public List<UBill> UBills { get; set; }

        [NotMapped]
        public double Balance { get; set; }
    }
}
