using Moq;
using PhonebookMultithread;

namespace Phonebook.Test
{
    public class UnitTest
    {
        Mock<IPhoneBookFileService> mockService;
        PhoneBook phonebook;

        [SetUp]
        public void Setup()
        {
            var dic = new Dictionary<string, string>();
            mockService = new Mock<IPhoneBookFileService>();
            mockService.Setup(m => m.GetEntries()).Returns(dic);

            phonebook = new PhoneBook(mockService.Object);
        }

        [Test]
        public void AddsEntryToPhoneBook()
        {
            //Arrange
            var name = "dave";
            var number = "07567642122";

            //Act
            phonebook.Add(name, number);
            var testNumber = phonebook.Get(name);

            //Assert
            Assert.That(testNumber, Is.EqualTo(number));
        }

        [Test]
        public void AddsEntryToFile()
        {
            //Arrange
            var name = "steve";
            var number = "07823647222";

            //Act
            phonebook.Add(name, number);
            var phonebookDictionary = phonebook.GetEntries();
            //Assert
            Assert.That(phonebookDictionary, Contains.Key(name));
            Assert.That(phonebookDictionary, Contains.Value(number));
            mockService.Verify(m => m.Write(phonebook.GetEntries()), Times.Once);
        }

        [Test]
        public void DoesNotAddNumberOfIncorrectLength()
        {
            //Arrange & Act
            var name = "Alom";
            var incorrectLengthNumber = "0432423123";

            //Assert
            Assert.Throws<ArgumentException>(() => phonebook.Add(name, incorrectLengthNumber));
        }

        [Test]
        public void DoesNotAddEntryIfOneAlreadyExists()
        {
            //Arrange
            var name = "Henry";
            var number = "07654532561";

            //Act
            phonebook.Add(name, number);

            //Assert
            Assert.Throws<OperationCanceledException>(() => phonebook.Add(name, number));
        }

        [Test]
        public void LoadsDataFromTextFile()
        {
            mockService.Verify(m => m.GetEntries(), Times.Once);
        }

        [Test]
        public void DeletesEntryFromPhoneBookByName()
        {
            //Arrange
            var name = "Lenny";
            var number = "07856647222";

            //Act
            phonebook.Add(name, number);
            phonebook.RemoveByName(name);

            //Assert
            Assert.Throws<ArgumentException>(() => phonebook.Get(name));

        }

        [Test]
        public void DeletesEntryFromPhoneBookByNumber()
        {
            Mock<PhoneBook> phonebookMock = new Mock<PhoneBook>(mockService.Object);
            //Arrange
            var name = "Lenny";
            var number = "07856647222";
            phonebookMock.Setup(m => m.FindKeyByValue(number)).Returns(name);
            phonebook = phonebookMock.Object;

            //Act
            phonebook.Add(name, number);
            phonebook.RemoveByNumber(number);

            //Assert
            Assert.Throws<ArgumentException>(() => phonebook.Get(name));
        }

        [Test]
        public void DeletesEntryFromFile()
        {
            //Arrange
            var name = "Lenny";
            var number = "07856647222";

            //Act
            phonebook.Add(name, number);
            phonebook.RemoveByName(name);
            PhoneBookFileService fileService = new PhoneBookFileService();
            var entries = fileService.GetEntries();


            //Assert
            Assert.IsFalse(entries.ContainsKey(name));
        }

        [Test]
        public void GetsPhonebookEntry()
        {
            //Arrange
            var name = "Lenny";
            var number = "07856647222";

            //Act
            phonebook.Add(name, number);

            //Assert
            Assert.That(phonebook.Get(name), Is.EqualTo(number));
        }

        [Test]
        public void UpdatesPhonebookEntryWithNewNumber()
        {
            //Arrange
            var name = "Lenny";
            var number = "07856647222";
            var newNumber = "07634253642";


            //Act
            phonebook.Add(name, number);
            phonebook.Update(name, newNumber);

            //Assert
            Assert.That(phonebook.Get(name), Is.EqualTo(newNumber));
        }

        [Test]
        public void RetrievesKeyUsingValue()
        {
            //Arrange
            var name = "dave";
            var number = "07567642122";

            //Act
            phonebook.Add(name, number);
            var key = phonebook.FindKeyByValue(number);

            //Assert
            Assert.That(key, Is.EqualTo(name));
        }

        [Test]
        public void DoesNotRetrieveKeyUsingValue()
        {
            //Arrange
            var number = "07567642122";

            //Act
            var key = phonebook.FindKeyByValue(number);

            //Assert
            Assert.That(String.IsNullOrEmpty(key), Is.EqualTo(true));
        }
    }
}