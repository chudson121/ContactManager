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
            EntryEvents = GetDiaryEvents();

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
                            new XElement("AHAMemberNumber", contact.AHAMemberNumber)
                        ));

            
            _xmlDoc.Save(EntryFileFullName);


            //XmlSerializer serializer = new XmlSerializer(typeof(Contact));
            //TextWriter textWriter = new StreamWriter(EntryFileFullName);
            //serializer.Serialize(textWriter, contact);
            //textWriter.Close();
        }

        public Contact Get(int Id)
        {
            string nodeName = "Contact";
            _log.Info(string.Format("Loading Entries - {0}", _xmlDoc.Descendants(nodeName).Count()));

            IEnumerable<Contact> entries;
            entries = from e in _xmlDoc.Descendants(nodeName)
                      let xElement = e.Element("Id")
                      where xElement != null && xElement.Value == Id.ToString()
                      let addy = e.Element("Address1")
                      select (new Contact
                      {
                          Id = Convert.ToInt32(e.Element("Id").Value),
                          MembershipDate = Convert.ToDateTime(e.Element("MembershipDate").Value).ToLocalTime(),
                          FirstName = e.Element("FirstName").Value,
                          LastName = e.Element("LastName").Value,
                          EmailAddress = e.Element("EmailAddress").Value,
                          Phone = e.Element("Phone").Value,
                          Address1 = new Address(addy.Element("Street1").Value, addy.Element("Street2").Value, addy.Element("City").Value, addy.Element("State").Value, addy.Element("Zip").Value),
                          DOB = Convert.ToDateTime(e.Element("DOB").Value),
                          BusinessPhone = e.Element("BusinessPhone").Value,
                          Fax = e.Element("Fax").Value,
                          AHAMemberNumber = e.Element("AHAMemberNumber").Value,


                      });


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
                           select (new Contact
                          {
                              Id = Convert.ToInt32(e.Element("Id").Value),
                              MembershipDate = Convert.ToDateTime(e.Element("MembershipDate").Value).ToLocalTime(),
                              FirstName = e.Element("FirstName").Value,
                              LastName = e.Element("LastName").Value,
                              EmailAddress = e.Element("EmailAddress").Value,
                              Phone = e.Element("Phone").Value,
                              Address1 = new Address(addy.Element("Street1").Value, addy.Element("Street2").Value, addy.Element("City").Value, addy.Element("State").Value, addy.Element("Zip").Value),
                              DOB = Convert.ToDateTime(e.Element("DOB").Value),
                              BusinessPhone = e.Element("BusinessPhone").Value,
                              Fax = e.Element("Fax").Value,
                              AHAMemberNumber = e.Element("AHAMemberNumber").Value,
                           });

           

            if (!String.IsNullOrEmpty(filter))
            {
                _log.Info(string.Format("Loading Entries with filter - {0}", filter));
                entries = entries.Where(p => p.LastName.StartsWith(filter));
            }
                


            return entries.ToList();
        }



        public void UpdateEntry(Contact contact)
        {
            //find original
            var items = from item in _xmlDoc.Descendants("Contacts")
                        let xElement = item.Element("Id")
                        where xElement != null && xElement.Value == contact.Id.ToString()
                        select item;

            foreach (XElement itemElement in items)
            {
                //itemElement.SetElementValue("EntryEvent", entry.EntryEvent);
                //itemElement.SetElementValue("EntryTxt", entry.EntryTxt);
                //itemElement.SetElementValue("EntryDt", entry.EntryDt);
                //itemElement.SetElementValue("UserName", entry.UserName);
            }


            //update with this one.


            _xmlDoc.Save(EntryFileFullName);
        }

        public IEnumerable<string> GetDiaryEvents()
        {
            //EntryEvents
            IEnumerable<string> entries = from e in _xmlDoc.Descendants("DiaryEntry")
                                          let xElement = e.Element("EntryEvent")
                                          where xElement != null
                                          select (xElement.Value);

            return entries.Distinct();
        }

        //public BindingList<DiaryEntry> GetDiaryEntries(string eventTypeFilter = "")
        //{

        //    string sort = string.IsNullOrEmpty(SortDir) ? "desc" : SortDir.ToLower();
        //    List<DiaryEntry> entries = new List<DiaryEntry>();
        //    switch (sort)
        //    {
        //        case "desc":
        //            entries = LoadList(eventTypeFilter).OrderByDescending(x => x.EntryDt).ToList();
        //            break;
        //        case "asc":
        //            entries = LoadList(eventTypeFilter).OrderBy(x => x.EntryDt).ToList();
        //            break;
        //        default:
        //            entries = LoadList(eventTypeFilter).OrderBy(x => x.Id).ToList();
        //            break;
        //    }

        //    var bindinglist = new BindingList<DiaryEntry>(entries);

        //    return bindinglist;
        //}

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

        //private List<DiaryEntry> LoadList(string eventTypeFilter = "")
        //{
        //    _log.Info(string.Format("Loading Entries - {0}", _xmlDoc.Descendants("DiaryEntry").Count()));

        //    IEnumerable<DiaryEntry> entries;

        //    //TODO: improve this so the filter doesnt have to be in the if statement

        //    if (string.IsNullOrEmpty(eventTypeFilter))
        //    {

        //        entries = from e in _xmlDoc.Descendants("DiaryEntry")
        //                  let id = e.Element("Id")
        //                  where id != null
        //                  let entryDt = e.Element("EntryDt")
        //                  where entryDt != null
        //                  let entryEvent = e.Element("EntryEvent")
        //                  where entryEvent != null
        //                  let entryTxt = e.Element("EntryTxt")
        //                  where entryTxt != null
        //                  let userName = e.Element("UserName")
        //                  where userName != null
        //                  select (new DiaryEntry
        //                  {
        //                      Id = Convert.ToInt32(id.Value),
        //                      EntryDt = Convert.ToDateTime(entryDt.Value).ToLocalTime(),
        //                      EntryEvent = entryEvent.Value,
        //                      EntryTxt = entryTxt.Value,
        //                      UserName = userName.Value
        //                  });

        //    }
        //    else
        //    {
        //        _log.Info(string.Format("Loading Entries with filter - {0}", eventTypeFilter));
        //        entries = from e in _xmlDoc.Descendants("DiaryEntry")
        //                  let xElement1 = e.Element("EntryEvent")
        //                  where xElement1 != null
        //                  where xElement1.Value.ToLower() == eventTypeFilter.ToLower()
        //                  let id = e.Element("Id")
        //                  where id != null
        //                  let entryDt = e.Element("EntryDt")
        //                  where entryDt != null
        //                  let entryEvent = e.Element("EntryEvent")
        //                  where entryEvent != null
        //                  let entryTxt = e.Element("EntryTxt")
        //                  where entryTxt != null
        //                  let userName = e.Element("UserName")
        //                  where userName != null
        //                  select (new DiaryEntry
        //                  {
        //                      Id = Convert.ToInt32(id.Value),
        //                      EntryDt = Convert.ToDateTime(entryDt.Value).ToLocalTime(),
        //                      EntryEvent = entryEvent.Value,
        //                      EntryTxt = entryTxt.Value,
        //                      UserName = userName.Value
        //                  });

        //    }




        //    return entries.ToList();
        //}

        private static string GetUser()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            return windowsIdentity != null ? windowsIdentity.Name : string.Empty;
        }

    }
}
