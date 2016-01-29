using ContactManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContactManager.Controller
{
    class ContactController
    {

        public void Add(string filename)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(Contact));
            TextWriter tw = new StreamWriter(filename);

            //serializer.Serialize(tw, );
            tw.Close();


        }

        public void Get(int Id)
        {



        }

        public void Delete()
        {

        }

        public List<Contact> ListAll()
        {

            return null;

        }

    }
}
