using Moq;
using PhonebookMultithread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Test
{
    public class MultithreadingUnitTest
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
    }
}
