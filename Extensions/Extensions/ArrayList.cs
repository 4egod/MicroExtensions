#if !MF && !TINYCLR
//namespace Extensions
//{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ArrayList : IList, ICollection, IEnumerable
    {
        private List<object> list;

        public ArrayList()
        {
            this.list = new List<object>();
        }

        public int Add(object value)
        {
            this.list.Add(value);
            return this.list.Count;
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public bool Contains(object value)
        {
            return this.list.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.list.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.list.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.list.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            object[] objects = new object[this.list.Count];
            this.list.CopyTo(objects, index);
            array = objects;
        }

        public virtual Array ToArray(Type type)
        {
            Array array = Array.CreateInstance(type, this.list.Count);
            Array.Copy(this.list.ToArray(), 0, array, 0, this.list.Count);
            return array;
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return this.list; }
        }

        public IEnumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
//}
#endif
