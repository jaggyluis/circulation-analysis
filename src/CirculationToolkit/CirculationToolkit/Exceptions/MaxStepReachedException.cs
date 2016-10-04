using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Exceptions
{
    class MaxStepReachedException : Exception
    {
        public MaxStepReachedException()
        {
        }

        public MaxStepReachedException(string message)
        : base(message)
        {
        }

        public MaxStepReachedException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
