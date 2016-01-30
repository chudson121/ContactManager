using ContactManager.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ContactManager.Controller
{
    class ContactController
    {
        private readonly ILog _log;

        private static XDocument _xmlDoc;

        public string EntryFileFullName { get; set; }

        public IEnumerable<string> EntryEvents { get; private set; }

        public ContactController(ILog log, string pathToEntries, string sortDir = "desc")
        {

            if (log == null)
                throw new ArgumentNullException("log", "ILog is null");

            _log = log;
            _xmlDoc = GetEntryFile(pathToEntries);
            //SortDir = sortDir;
            //EntryEvents = GetDiaryEvents();

        }

        public void Add(Contact contact)
        {
            List<Contact> items = GetAll();
            int max = items.Count == 0 ? 0 : items.Max(i => i.Id);

            int newId = ++max;

            var xElement = _xmlDoc.Element("Contacts");
            if (xElement != null)
                xElement.Add(
                    new XElement("Contact", new XElement("Id", newId.ToString(CultureInfo.InvariantCulture)),
            new XElement("MembershipDate", contact.MembershipDate),
                             new XElement("FirstName", contact.FirstName),
                              new XElement("LastName", contact.LastName),
                              new XElement("EmailAddress", contact.EmailAddress),
                              new XElement("Phone", contact.Phone),
                              new XElement("Address1",
                                            new XElement("Street1", contact.Address1.Street1),
                                            new XElement("Street2", contact.Address1.Street2),
                                            new XElement("City", contact.Address1.City),
                                            new XElement("State", contact.Address1.State),
                                            new XElement("Zip", contact.Address1.Zip)
                                            ),
                            new XElement("DOB", contact.DOB),
                            new XElement("BusinessPhone", contact.BusinessPhone),
                            new XElement("Fax", contact.Fax),
                            new XElement("AHAMemberNumber", contact.AHAMemberNumber),
                            new XElement("EntryAdded", DateTime.UtcNow),
                            new XElement("EntryUpdated", DateTime.UtcNow),
                            new XElement("IsActive", contact.IsActive)

                            

                        ));


            _xmlDoc.Save(EntryFileFullName);
        }

        public Contact Get(int Id)
        {
            string nodeName = "Contact";

            _log.Info(string.Format("Loading Entries - {0}", _xmlDoc.Descendants(nodeName).Count()));

            IEnumerable<Contact> entries;
            entries = from e in _xmlDoc.Descendants(nodeName)
                      let xElement = e.Element("Id")
                      where xElement != null && xElement.Value == Id.ToString()
                      select (new Contact(e));

            return entries.FirstOrDefault();


        }

        public void Delete(Contact contact)
        {
            _xmlDoc.Root.Elements().Where(e => e.Attribute("Id").Value.Equals(contact.Id.ToString())).Select(e => e).Single().Remove();
            _xmlDoc.Save(EntryFileFullName);
        }

        public List<Contact> GetAll(string filter = "")
        {
            string nodeName = "Contact";
            _log.Info(string.Format("Loading Entries - {0}", _xmlDoc.Descendants(nodeName).Count()));

            IEnumerable<Contact> entries;

            //TODO: Add back address
            entries = from e in _xmlDoc.Descendants(nodeName)
                      let addy = e.Element("Address1")
                      select (new Contact(e));



            if (!String.IsNullOrEmpty(filter))
            {
                _log.Info(string.Format("Loading Entries with filter - {0}", filter));
                entries = entries.Where(p => p.LastName.StartsWith(filter));
            }



            return entries.ToList();
        }
        
        public void UpdateEntry(Contact contact)
        {
            string nodeName = "Contact";
            //find original
            var xmlElement = (from item in _xmlDoc.Descendants(nodeName)
                              let xElement = item.Element("Id")
                              where xElement != null && xElement.Value == contact.Id.ToString()
                              select item).Single();
            var oldContact = new Contact(xmlElement);

            xmlElement.ReplaceWith(
                  new XElement("Contact", new XElement("Id", contact.Id),
          new XElement("MembershipDate", contact.MembershipDate),
                           new XElement("FirstName", contact.FirstName),
                            new XElement("LastName", contact.LastName),
                            new XElement("EmailAddress", contact.EmailAddress),
                            new XElement("Phone", contact.Phone),
                            new XElement("Address1",
                                          new XElement("Street1", contact.Address1.Street1),
                                          new XElement("Street2", contact.Address1.Street2),
                                          new XElement("City", contact.Address1.City),
                                          new XElement("State", contact.Address1.State),
                                          new XElement("Zip", contact.Address1.Zip)
                                          ),
                          new XElement("DOB", contact.DOB),
                          new XElement("BusinessPhone", contact.BusinessPhone),
                          new XElement("Fax", contact.Fax),
                          new XElement("AHAMemberNumber", contact.AHAMemberNumber),
                          new XElement("EntryAdded", contact.EntryAdded),
                          new XElement("EntryUpdated", DateTime.UtcNow),
                        new XElement("IsActive", contact.IsActive)

                      ));

            //update with this one.
            _xmlDoc.Save(EntryFileFullName);
        }
        
        public void ArchiveFile()
        {
            string datepart = String.Format("{0:yyyy-MM-dd_hh-mm-ss}", DateTime.Now.ToUniversalTime());

            string newFileName = string.Format("{0}-{1}", Path.GetFileNameWithoutExtension(EntryFileFullName), datepart);

            string outFileName = EntryFileFullName.Replace(Path.GetFileNameWithoutExtension(EntryFileFullName), newFileName);

            try
            {
                _log.Info(string.Format("Moving current File - {0} to new archive File - {1}", EntryFileFullName, outFileName));
                File.Move(EntryFileFullName, outFileName);
            }
            catch (Exception)
            {
                _log.Error(string.Format("ERROR - Moving current File - {0} to new archive File - {1}", EntryFileFullName, outFileName));
                throw;
            }


            //generate new file
            _xmlDoc = GetEntryFile(EntryFileFullName);
        }

        private XDocument GetEntryFile(string pathToEntries)
        {
            _log.Info(string.Format("Configuring Entries File {0}.", pathToEntries));

            EntryFileFullName = string.IsNullOrEmpty(pathToEntries)
                ? Application.ExecutablePath.Replace(Assembly.GetExecutingAssembly().GetName().Name + ".EXE", "") +
                  "Entries.xml"
                : pathToEntries;

            _log.Info(string.Format("Checking Entries File at {0}.", EntryFileFullName));
            //check if path exists
            var fi = new FileInfo(Path.GetFullPath(EntryFileFullName));
            if (fi.Exists)
            {
                _log.Info(string.Format("File already Exists {0}.", EntryFileFullName));
                return XDocument.Load(EntryFileFullName);
            }

            // Determine whether the directory exists.
            string path = Path.GetDirectoryName(EntryFileFullName);
            if (string.IsNullOrEmpty(path))
            {
                _log.Error(string.Format("Problem creating Path for the Entries file {0}.", path));
                MessageBox.Show(string.Format("Problem creating Path for the Entries file. {0}", EntryFileFullName));
            }
            else //path isnt blank
            {
                if (Directory.Exists(path))
                {
                    _log.Info(string.Format("Path already exists - {0}", path));
                }
                else
                {
                    // Try to create the directory.
                    try
                    {
                        Directory.CreateDirectory(path);
                        _log.Info(string.Format("The directory was created successfully at {0}", path));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(string.Format("Problem creating Path for the Entries file. {0}", EntryFileFullName));
                        throw;
                    }


                }
            }

            CreateFile(EntryFileFullName);
            return XDocument.Load(EntryFileFullName);
        }

        private void CreateFile(string filePath)
        {
            _log.Info(string.Format("Creating File - {0}", filePath));
            new XDocument(new XElement("Contacts")).Save(filePath);
        }


        private static string GetUser()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            return windowsIdentity != null ? windowsIdentity.Name : string.Empty;
        }

    }
}
