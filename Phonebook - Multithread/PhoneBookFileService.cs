namespace PhonebookMultithread
{
    public class PhoneBookFileService : IPhoneBookFileService
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "phonebook.txt");
        public PhoneBookFileService()
        {

        }

        public void Clear()
        {
            File.Delete(path);
        }

        public Dictionary<string, string> GetEntries()
        {
            try
            {
                var entries = File.ReadAllLines(path);
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

        public void Write(Dictionary<string, string> entries)
        {
            var entityWriteData = entries.Select(entry => $"{entry.Key} {entry.Value}").ToArray();
            File.WriteAllLines(path, entityWriteData);
        }
    }
}
