using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Model
{
    public class Contacts : ICollection
    {
        public string CollectionName;
        private ArrayList conArray = new ArrayList();


        public Contact this[int index]
        {
            get { return (Contact)conArray[index]; }
        }

        public void CopyTo(Array a, int index)
        {
            conArray.CopyTo(a, index);
        }
        public int Count
        {
            get { return conArray.Count; }
        }
        public object SyncRoot
        {
            get { return this; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public IEnumerator GetEnumerator()
        {
            return conArray.GetEnumerator();
        }

        public void Add(Contact newContact)
        {
            conArray.Add(newContact);
        }
    }
}
