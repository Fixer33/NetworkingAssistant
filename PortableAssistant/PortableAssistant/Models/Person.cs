using System;

namespace PortableAssistant.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string PublicName { get; set; }
        public string Description { get; set; }
        public int TalkingCoef { get; set; }
        public bool IncludeInForming { get; set; }
    }
}