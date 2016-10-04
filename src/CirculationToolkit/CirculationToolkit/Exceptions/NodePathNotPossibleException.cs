using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CirculationToolkit.Exceptions
{
    public class NodePathNotPossibleException : Exception
    {
        public NodePathNotPossibleException()
        {
        }

        public NodePathNotPossibleException(string message)
        : base(message)
        {
        }

        public NodePathNotPossibleException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
