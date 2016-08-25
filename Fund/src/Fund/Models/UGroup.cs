using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fund.Models
{
    public class UGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public string UUserId { get; set; }
        public UUser UUser { get; set; }

        public List<UMember> UMembers { get; set; }
        public List<UEvent> UEvents { get; set; }
        public List<UDebt> UDebts { get; set; }
        [NotMapped]
        public List<UTotal> UTotals { get; set; }
    }
}
