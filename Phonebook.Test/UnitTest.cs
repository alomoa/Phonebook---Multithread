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
        public void PhoneBook_ShouldAdd100PhoneNumbers()
        {
            //Arranage & Act
            var numbers = new List<string>();
            Parallel.For(0, 100, (index) =>
            {
                var number = (10000000000 + index).ToString();
                lock (numbers)
                {
                    numbers.Add(number);
                }
                phonebook.Add(number, number);
            });

            //Assert
            Assert.That(phonebook.Count(), Is.EqualTo(100));
            Assert.That(numbers.Count(), Is.EqualTo(100));

            foreach (var number in numbers)
            {
                Assert.True(phonebook.ContainsValue(number));
            }
        }

        [Test]
        public void PhoneBook_ShouldRemoveAllNumbers()
        {
            //Arranage & Act
            Parallel.For(0, 100, (index) =>
            {
                var number = (10000000000 + index).ToString();

                phonebook.Add(number, number);
                phonebook.RemoveByNumber(number);
            });

            //Assert
            Assert.That(phonebook.Count(), Is.EqualTo(0));    

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

            //Assert
            mockService.Verify(m => m.Write(phonebook.GetEntries()), Times.Once);
            //Check that steve is added to the dictionary
            //replace the arg in write with It
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
            Assert.Throws<ArgumentException>(() => phonebook.Add(name, number));
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
            //Arrange
            var name = "Lenny";
            var number = "07856647222";

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
    }
}