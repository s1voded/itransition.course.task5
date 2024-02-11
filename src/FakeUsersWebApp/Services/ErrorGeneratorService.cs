using Bogus;
using FakeUsersWebApp.Model;
using System.Text;

namespace FakeUsersWebApp.Services
{
    public class ErrorGeneratorService: IErrorService
    {
        public List<Func<StringBuilder, Faker, bool>> ListErrorFunc { get; set; }

        public ErrorGeneratorService()
        {
            ListErrorFunc = [SwapChar, DeleteChar, AddChar];
        }

        public bool SwapChar(StringBuilder word, Faker faker)
        {
            if (word.Length == 1) return false;

            var index1 = faker.Random.Number(0, word.Length - 1);
            int index2 = index1 < word.Length - 1 ? index1 + 1 : index1 - 1;
            (word[index1], word[index2]) = (word[index2], word[index1]);

            return true;
        }

        public bool DeleteChar(StringBuilder word, Faker faker)
        {
            if (word.Length == 1) return false;

            word.Remove(faker.Random.Number(0, word.Length - 1), 1);
            return true;
        }

        public bool AddChar(StringBuilder word, Faker faker)
        {
            word.Insert(faker.Random.Number(0, word.Length - 1), faker.Lorem.Letter().First());//only letter!
            return true;
        }
    }

    public interface IErrorService
    {
        List<Func<StringBuilder, Faker, bool>> ListErrorFunc { get; set; }
    }
}
