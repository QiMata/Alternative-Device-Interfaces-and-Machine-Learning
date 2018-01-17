using System;
using System.Collections.Generic;
using System.Text;

namespace QiMata.AlternativeInterfaces.Services.Models
{
    class Person
    {
        public Guid PersonId { get; set; }
        public Guid[] PersistedFaceIds { get; set; }
        public string Name { get; set; }
        public string UserData { get; set; }
    }
}
