namespace PhonebookMultithread
{
    public class PhoneBookFileService : IPhoneBookFileService
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "phonebook.txt");
        private object lockObject;
        public PhoneBookFileService()
        {

        }

        public void Clear()
        {
            lock (lockObject)
            {
                File.Delete(path);
            }
        }

        public Dictionary<string, string> GetEntries()
        {
            try
            {
                string[] entries;
                lock(lockObject)
                {
                    entries = File.ReadAllLines(path);
                }
                
                Dictionary<string, string> entriesDict = new Dictionary<string, string>();

                foreach (var entry in entries)
                {
                    var split = entry.Split(" ");
                    entriesDict.Add(split[0], split[1]);
                }

                return entriesDict;
            }
            catch
            {
                return new Dictionary<string, string>();
            }

        }

        public void Write(IDictionary<string, string> entries)
        {
            var entityWriteData = entries.Select(entry => $"{entry.Key} {entry.Value}").ToArray();
            lock(lockObject)
            {
                File.WriteAllLines(path, entityWriteData);
            }
        }
    }
}
