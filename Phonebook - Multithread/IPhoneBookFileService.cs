namespace PhonebookMultithread
{
    public interface IPhoneBookFileService
    {
        void Clear();
        Dictionary<string, string> GetEntries();
        void Write(IDictionary<string, string> entries);
    }
}