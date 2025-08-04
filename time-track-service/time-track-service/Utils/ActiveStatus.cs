using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace time_track_service.Utils
{
    public class ActiveStatus
    {
        public const int Inactive = 0;
        public const int Active = 1;
        public const int Pending = 2;
        public const int Deleted = 3;

        protected ActiveStatus() { }
    }
}
