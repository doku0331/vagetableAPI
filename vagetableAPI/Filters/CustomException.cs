using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vagetableAPI.Filters
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {

        }
    }
}