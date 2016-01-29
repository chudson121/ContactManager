using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContactManager.Model
{
    [Serializable()]
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public Address Address1 { get; set; }
        public DateTime MembershipDate { get; set; }

        public DateTime DOB { get; set; }
        public string BusinessPhone { get; set; }
        public string Fax { get; set; }
        public string AHAMemberNumber { get; set; }


    }
}
