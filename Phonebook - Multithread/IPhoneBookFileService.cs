namespace PhonebookMultithread
{
    public interface IPhoneBookFileService
    {
        void Clear();
        Dictionary<string, string> GetEntries();
        void Write(Dictionary<string, string> entries);
    }
}