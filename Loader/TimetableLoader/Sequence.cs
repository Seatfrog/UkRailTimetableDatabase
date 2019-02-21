using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TimetableLoader
{
    internal class Sequence
    {
        private long nextId = 0;

        public long GetNext()
        {
            return Interlocked.Increment(ref nextId);
        }
    }
}
