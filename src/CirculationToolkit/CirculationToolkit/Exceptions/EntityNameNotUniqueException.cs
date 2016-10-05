using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Exceptions
{
    class EntityNameNotUniquException : Exception
    {
        public EntityNameNotUniquException()
        {
        }

        public EntityNameNotUniquException(string message)
        : base(message)
        {
        }

        public EntityNameNotUniquException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
