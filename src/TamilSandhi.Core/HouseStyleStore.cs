using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace TamilSandhi
{
    public sealed class HouseStyleStore
    {
        private static readonly ConcurrentDictionary<string,object> Gates=new ConcurrentDictionary<string,object>(StringComparer.OrdinalIgnoreCase);
        private readonly string path;private readonly object gate;
        public HouseStyleStore(string filename){path=Path.GetFullPath(filename);gate=Gates.GetOrAdd(path,p=>new object());}
        public HouseStyle Load(){lock(gate){return Read();}}
        private HouseStyle Read()
        {
            var value=File.Exists(path)?Json.Read<HouseStyle>(File.ReadAllText(path,Encoding.UTF8)):new HouseStyle();
            if(value==null||value.NoInsertBefore==null||value.NoInsertBefore.Exists(x=>x==null)||String.IsNullOrWhiteSpace(value.Version))throw new InvalidDataException("Invalid house style configuration.");
            return value;
        }
        public T WithCurrent<T>(Func<HouseStyle,T> action){lock(gate){return action(Read());}}
        public void Save(HouseStyle value)
        {
            lock(gate)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));string temporary=path+"."+Guid.NewGuid().ToString("N")+".tmp";
                try{File.WriteAllText(temporary,Json.Write(value),new UTF8Encoding(false));if(File.Exists(path))File.Replace(temporary,path,null);else File.Move(temporary,path);}
                finally{if(File.Exists(temporary))File.Delete(temporary);}
            }
        }
    }
    public sealed class PreparedChapter
    {
        public ChapterSnapshot Snapshot {get;set;}
        public string Error {get;set;}
    }
    public static class ScopePreparation
    {
        public static PreparedChapter Read(string project,int book,int chapter,Func<string> read)
        {
            try{string text=read();if(text==null)throw new InvalidDataException("Host returned no chapter text.");return new PreparedChapter{Snapshot=new ChapterSnapshot{Project=project,Book=book,Chapter=chapter,Text=text}};}
            catch(Exception ex){return new PreparedChapter{Error=book+" "+chapter+": "+ex.Message};}
        }
    }
}
