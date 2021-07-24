using System;

namespace Interfaces
{
    public class CustomDTOAttribute : Attribute
    {
        public string Url { get; set; }
        public string Method { get; set; }
    }
}
