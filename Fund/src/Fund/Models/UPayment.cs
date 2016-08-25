using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    public class UPayment
    {
        public int Id { get; set; }
        public double Amount { get; set; }

        public int UEventId { get; set; }
        public int UMemberId { get; set; }

        public UEvent UEvent { get; set; }
        public UMember UMember { get; set; }

        [NotMapped]
        public string MemberName { get; set; }
    }
}
