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
            lock (_entries)
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
                    throw new ArgumentException("The length of the number provided is incorrect");
                }
            }
        }

        public string Get(string name)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey(name))
                {
                    return _entries[name];
                }
                else
                {
                    throw new ArgumentException($"{name} name does not exist");
                }
            }
        }

        public void RemoveByName(string name)
        {
            lock (_entries)
            {
                if (_entries.ContainsKey(name))
                {
                    _entries.Remove(name, out _);
                    _phoneBookService.Write(_entries);
                }
                else
                {
                    throw new ArgumentException($"{name} does not exist in the phonebook");
                }
            }
        }

        public IDictionary<string, string> GetEntries()
        {
            return new ConcurrentDictionary<string, string>(_entries);
        }

        public void RemoveByNumber(string number)
        {
            lock (_entries)
            {
            var keys = _entries.Keys;
            var deleteSuccess = false;
            foreach (var key in keys)
            {
                if (_entries[key] == number)
                {
                    deleteSuccess = _entries.Remove(key, out _);
                    if (deleteSuccess)
                    {
                        _phoneBookService.Write(_entries);
                    }
                    break;
                }
            }
            if (!deleteSuccess)
            {
                throw new ArgumentException($"{number} does not exist in phonebook");
            }
        }
        }

        public void Update(string name, string newNumber)
        {
            lock (_entries)
            {
            if (_entries.ContainsKey(name))
            {
                _entries[name] = newNumber;
                    _phoneBookService.Write(_entries);
                }
            }
        }

        public void Clear()
        {
            lock (_entries)
            {
            _entries.Clear();
            _phoneBookService.Clear();
        }
        }

        public int Count()
        {
            return (int)_entries.Count;
        }

        public bool ContainsValue(string number)
        {
            lock (_entries)
            {
                return _entries.Values.Contains(number);
            }
        }
    }
}

