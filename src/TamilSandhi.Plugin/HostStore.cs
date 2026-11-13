using System;
using Paratext.PluginInterfaces;

namespace TamilSandhi.Plugin
{
    internal sealed class HostStore : IChapterStore
    {
        private readonly IProject project;
        private readonly IPluginObject owner;
        private IWriteLock held;
        public HostStore(IProject p,IPluginObject obj){project=p;owner=obj;}
        public string ProjectId {get{return project.ID;}}
        public string Read(int b,int c){return project.GetUSFM(b,c);}
        public bool CanEdit(int b,int c){return !project.IsResource && project.CanEdit(owner,b,c);}
        public IDisposable Lock(int b,int c)
        {
            if(held!=null)throw new InvalidOperationException("A write transaction is already active.");
            var requested=project.RequestWriteLock(owner,delegate(IWriteLock revoked){revoked.Dispose();held=null;},b,c);
            if(requested==null)return null;
            held=requested;return new LockHandle(this,requested);
        }
        public void Write(int b,int c,string text)
        {
            if(held==null)throw new InvalidOperationException("The host write lock was released.");
            project.PutUSFM(held,text,b);
        }
        private sealed class LockHandle:IDisposable
        {
            private HostStore store;private IWriteLock held;
            public LockHandle(HostStore s,IWriteLock h){store=s;held=h;}
            public void Dispose(){if(store.held==held){store.held=null;held.Dispose();}}
        }
    }
}
