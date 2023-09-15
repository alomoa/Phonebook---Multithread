using System.Collections.Concurrent;

namespace PhonebookMultithread
{
    public interface IPhoneBook
    {
        void Add(string name, string number);
        void Clear();
        bool ContainsValue(string number);
        int Count();
        string Get(string name);
        IDictionary<string, string> GetEntries();
        void RemoveByName(string name);
        void RemoveByNumber(string number);
        void Update(string name, string newNumber);
    }

    public class PhoneBook : IPhoneBook
    {
        private ConcurrentDictionary<string, string> _entries;
        private readonly IPhoneBookFileService _phoneBookService;

        public PhoneBook(IPhoneBookFileService service)
        {
            _phoneBookService = service;
            var entries = _phoneBookService.GetEntries();
            _entries = new ConcurrentDictionary<string, string>(entries);
        }

        public void Add(string name, string number)
        {

            if (number.Length == 11)
            {
                var success = _entries.TryAdd(name, number);

                if (success)
                {
                    _phoneBookService.Write(_entries);
                }
                else
                {
                    throw new OperationCanceledException("Name already exists");
                }

            }
            else
            {
                throw new ArgumentException("The length of the number provided is incorrect");
            }

        }

        public string Get(string name)
        {
            string? result;
            var success = _entries.TryGetValue(name, out result);
            if (success)
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"{name} name does not exist");
            }

        }

        public void RemoveByName(string name)
        {
            var success = _entries.TryRemove(name, out _);
            if (success == false)
            {
                throw new ArgumentException($"{name} does not exist in the phonebook");
            }
        }

        public IDictionary<string, string> GetEntries()
        {
            return new ConcurrentDictionary<string, string>(_entries);
        }

        public void RemoveByNumber(string number)
        {
            var foundKey = FindKeyByValue(number);
            if (!String.IsNullOrEmpty(foundKey))
            {
                var deleteSuccess = _entries.TryRemove(foundKey, out _);
                if (deleteSuccess)
                {
                    _phoneBookService.Write(_entries);
                }
                else
                {
                    throw new ArgumentException($"{number} does not exist in phonebook");
                }
            }
        }

        public string FindKeyByValue(string value)
        {
            var result = "";
            var keys = _entries.Keys;

            foreach (var key in keys)
            {
                var success = _entries.TryGetValue(key, out var stringValue);

                if (success && stringValue == value)
                {
                    result = key;
                    break;
                }
            }

            return result;
        }

        public void Update(string name, string newNumber)
        {
            var valueSuccess = _entries.TryGetValue(name, out var currentValue);

            if(valueSuccess)
            {
                var updateSuccess = _entries.TryUpdate(name, newNumber, currentValue);

                if (updateSuccess)
                {
                    _phoneBookService.Write(_entries);
                }
                else 
                {
                    throw new ArgumentException($"Failed to update {name}");
                }
            }
        }

        public void Clear()
        {

            _entries.Clear();
            _phoneBookService.Clear();

        }

        public int Count()
        {
            return (int)_entries.Count;
        }

        public bool ContainsValue(string number)
        {
            return _entries.Values.Contains(number);

        }
    }
}

