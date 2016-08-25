using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    public class UEvent
    {
        public int Id { get; set; }
        public int? UGroupId { get; set; }
        public int UEventTypeId { get; set; }
        public string Name { get; set; }

        public UGroup UGroup { get; set; }
        public UEventType UEventType { get; set; }

        public List<UBill> UBills { get; set; }
        public List<UPayment> UPayments { get; set; }
        public List<UMember> UMembers { get; set; }

        [NotMapped]
        public List<UTotal> UTotals { get; set; }
    }
}
