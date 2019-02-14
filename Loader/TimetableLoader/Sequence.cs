using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TimetableLoader
{
    internal class Sequence
    {
        private int nextId = 0;

        public int GetNext()
        {
            return Interlocked.Increment(ref nextId);
        }
    }
}
