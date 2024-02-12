using Bogus;
using Bogus.DataSets;
using FakeUsersWebApp.Model;
using System.Reflection;
using System.Text;

namespace FakeUsersWebApp.Services
{
    public class ErrorGeneratorService: IErrorService
    {
        private List<Func<StringBuilder, Faker, bool>> _listErrorFunctions;

        public ErrorGeneratorService()
        {
            _listErrorFunctions = [SwapChar, DeleteChar, AddChar];
        }

        public bool SwapChar(StringBuilder word, Faker faker)
        {
            if (word.Length <= 1) return false;

            var index1 = faker.Random.Number(0, word.Length - 1);
            int index2 = index1 < word.Length - 1 ? index1 + 1 : index1 - 1;
            (word[index1], word[index2]) = (word[index2], word[index1]);

            return true;
        }

        public bool DeleteChar(StringBuilder word, Faker faker)
        {
            if (word.Length <= 1) return false;

            word.Remove(faker.Random.Number(0, word.Length - 1), 1);

            return true;
        }

        public bool AddChar(StringBuilder word, Faker faker)
        {
            word.Insert(faker.Random.Number(0, word.Length - 1), faker.Lorem.Letter().First());//only letter!

            return true;
        }

        public void CreateErrors<T>(T objectForErrors, float countErrors, Faker faker)
        {
            var totalErrors = GetTotalCountErrors(countErrors, faker.Random);
            var valueBufferForErrors = new StringBuilder();

            while (totalErrors > 0)
            {          
                var createErrorResult = CreateErrorInProperty(objectForErrors,valueBufferForErrors, faker);
                if (createErrorResult) totalErrors--;
            }
        }

        private bool CreateErrorInProperty<T>(T objectForErrors, StringBuilder valueBuffer, Faker faker)
        {
            var randomProperty = faker.Random.ArrayElement(GetAllStringProperties(objectForErrors).ToArray());

            valueBuffer.Append(randomProperty.GetValue(objectForErrors));
            var errorFunction = faker.Random.ListItem(_listErrorFunctions);
            var createErrorResult = errorFunction(valueBuffer, faker);
            randomProperty.SetValue(objectForErrors, valueBuffer.ToString());
            valueBuffer.Clear();

            return createErrorResult;
        }

        private IEnumerable<PropertyInfo> GetAllStringProperties<T>(T objecctForErrors)
        {
            return objecctForErrors.GetType().GetProperties().Where(prop => prop.PropertyType == typeof(string)).Where(prop => prop.GetValue(objecctForErrors)?.ToString()?.Length > 0);
        }

        private int GetTotalCountErrors(float countErrors, Randomizer randomizer)
        {
            var whole = (int)Math.Truncate(countErrors);
            if (randomizer.Bool(countErrors - whole)) whole++;
            return whole;
        }
    }

    public interface IErrorService
    {
        public void CreateErrors<T>(T objectForErrors, float countErrors, Faker faker);
    }
}
