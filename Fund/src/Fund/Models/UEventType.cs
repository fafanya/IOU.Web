using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fund.Models
{
    public class UEventType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UEvent> UEvents { get; set; }

        public static int tOwn = 1;
        public static int tCommon = 2;
        public static int tPartly = 3;
    }
}
