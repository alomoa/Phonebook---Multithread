namespace PhonebookMultithread
{
    public interface IPhoneBook
    {
        void Add(string name, string number);
        void Clear();
        bool ContainsValue(string number);
        int Count();
        string Get(string name);
        Dictionary<string, string> GetEntries();
        void RemoveByName(string name);
        void RemoveByNumber(string number);
        void Update(string name, string newNumber);
    }

    public class PhoneBook : IPhoneBook
    {

        private Dictionary<string, string> _entries;

        private readonly IPhoneBookFileService _phoneBookService;

        public PhoneBook(IPhoneBookFileService service)
        {
            _phoneBookService = service;
            _entries = _phoneBookService.GetEntries();
        }

        public void Add(string name, string number)
        {
            lock (_entries)
            {
                if (number.Length == 11)
                {
                    _entries.Add(name, number);
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
            if (_entries.ContainsKey(name))
            {
                _entries.Remove(name);
                _phoneBookService.Write(_entries);
            }
            else
            {
                throw new ArgumentException($"{name} does not exist in the phonebook");
            }
        }

        public Dictionary<string, string> GetEntries()
        {
            lock (_entries)
            {
                return _entries;
            }
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
                        _entries.Remove(key);
                        _phoneBookService.Write(_entries);
                        deleteSuccess = true;
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
                }
            }
        }

        public void Clear()
        {
            lock (_entries)
            {
                _entries = new Dictionary<string, string>();
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
                return _entries.ContainsValue(number);
            }
        }
    }
}

