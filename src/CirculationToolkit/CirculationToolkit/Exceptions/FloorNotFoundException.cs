using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Exceptions
{
    class FloorNotFoundException : Exception
    {
        public FloorNotFoundException()
        {
        }

        public FloorNotFoundException(string message)
        : base(message)
        {
        }

        public FloorNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
