using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_sql.Memory
{
    public class InsertResponse
    {
        public string Table { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
