using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
    Pretty much copy-paste from http://stackoverflow.com/questions/2200241/in-c-sharp-how-do-i-define-my-own-exceptions
    */

namespace Game.Exceptions
{
    [Serializable]
    public class IllegalMoveException : Exception
    {
        public IllegalMoveException()
        { }

        public IllegalMoveException(string message)
            : base(message)
        { }

        public IllegalMoveException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected IllegalMoveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
