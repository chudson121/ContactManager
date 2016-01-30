using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ContactManager.Model
{

    public class Contact
    {
        public int Id { get; set; }
        public DateTime EntryAdded { get; set; }
        public DateTime EntryUpdated { get; set; }
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

        public bool IsActive { get; set; }

        public Contact() { }
        public Contact(System.Xml.Linq.XElement e)
        {
            XElement addy = e.Element("Address1");
            Id = Convert.ToInt32(e.Element("Id").Value);
            MembershipDate = Convert.ToDateTime(e.Element("MembershipDate").Value).ToLocalTime();
            FirstName = e.Element("FirstName").Value;
            LastName = e.Element("LastName").Value;
            EmailAddress = e.Element("EmailAddress").Value;
            Phone = e.Element("Phone").Value;
            Address1 = new Address(addy.Element("Street1").Value, addy.Element("Street2").Value, addy.Element("City").Value, addy.Element("State").Value, addy.Element("Zip").Value);
            DOB = Convert.ToDateTime(e.Element("DOB").Value);
            BusinessPhone = e.Element("BusinessPhone").Value;
            Fax = e.Element("Fax").Value;
            AHAMemberNumber = e.Element("AHAMemberNumber").Value;
            EntryAdded = Convert.ToDateTime(e.Element("EntryAdded").Value);
            EntryUpdated = Convert.ToDateTime(e.Element("EntryUpdated").Value);
            IsActive = Convert.ToBoolean(e.Element("IsActive").Value);


        }
    }
}
