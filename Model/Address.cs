using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContactManager.Model
{
    public class Address
    {
        // The XmlAttribute instructs the XmlSerializer to serialize the 
        // Name field as an XML attribute instead of an XML element (the 
        // default behavior).
        //[XmlAttribute]
        public string Street1;
        // Setting the IsNullable property to false instructs the 
        // XmlSerializer that the XML attribute will not appear if 
        // the City field is set to a null reference.
        [XmlElementAttribute(IsNullable = false)]
        public string Street2;
        public string City;
        public string State;
        public string Zip;

        public Address() { }
        public Address(string street1, string street2, string city, string state, string zip)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            State = state;
            Zip = zip;

        }

    }
}
