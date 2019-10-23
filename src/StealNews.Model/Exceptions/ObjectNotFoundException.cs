using System;

namespace StealNews.Model.Exceptions
{
    public class ObjectNotFoundException : ApplicationException
    {
        public ObjectNotFoundException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
