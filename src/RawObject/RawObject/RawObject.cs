using System;
using System.IO;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace VVVV.ROD

{
    public class RawObject : IDisposable
    {
        public Dictionary<string, Stream> Fields = new Dictionary<string, Stream>();
        public string Name = "";
        public Stopwatch Age = new Stopwatch();
        public string Debug = "";
        public bool Remove = false;
        public RodWrap Container;

        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                List<Stream> streamlist = this.Fields.Values.ToList();
                for (int i = 0; i < streamlist.Count; i++)
                {
                    streamlist[i].Dispose();
                }
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
        public void DisposeKey(string key)
        {
            this.Fields[key].Dispose();
            this.Fields.Remove(key);
        }

        public RawObject(RodWrap container)
        {
            this.Age.Reset();
            this.Age.Start();
            this.Container = container;
        }

        public RawObject DeepCopy(RodWrap container)
        {
            RawObject NewObject = new RawObject(container);
            NewObject.Name = this.Name;
            NewObject.Debug = this.Debug;

            foreach(KeyValuePair<string, Stream> kvp in this.Fields)
            {
                Stream newstream = new MemoryStream();
                kvp.Value.Position = 0;
                kvp.Value.CopyTo(newstream);
                kvp.Value.Position = 0;
                newstream.Position = 0;
                NewObject.Fields.Add(kvp.Key, newstream);
            }
            return NewObject;
        }
        public RawObject DeepCopy(RodWrap container, string name)
        {
            RawObject NewObject = new RawObject(container);
            NewObject.Name = name;
            NewObject.Debug = this.Debug;

            foreach (KeyValuePair<string, Stream> kvp in this.Fields)
            {
                Stream newstream = new MemoryStream();
                kvp.Value.Position = 0;
                kvp.Value.CopyTo(newstream);
                kvp.Value.Position = 0;
                newstream.Position = 0;
                NewObject.Fields.Add(kvp.Key, newstream);
            }
            return NewObject;
        }
    }
    public class RodWrap
    {
        public Dictionary<string, RawObject> Objects = new Dictionary<string, RawObject>();
        public List<string> RemoveList = new List<string>();

        public RodWrap() { }

        public void RemoveObject(string k)
        {
            this.Objects[k].Remove = true;
        }
        public void RemoveTagged()
        {
            foreach (string k in this.RemoveList)
            {
                this.Objects[k].Dispose();
                this.Objects.Remove(k);
            }
            this.RemoveList.Clear();
        }
        public void Clear()
        {
            foreach (KeyValuePair<string, RawObject> kvp in this.Objects) kvp.Value.Dispose();
            this.Objects.Clear();
        }
    }
}
