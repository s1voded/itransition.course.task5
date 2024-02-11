using Bogus;
using FakeUsersWebApp.Model;

namespace FakeUsersWebApp.Services
{
    public class FakeUserGeneratorService : IUsersService
    {
        private readonly Dictionary<string, string[]> phoneFormats = new()
        {
            { "ru", ["8 (###) ###-##-##", "+7 ### ##-##-##"] }
        };

        public IEnumerable<User> GetUsers()
        {
            var seed = 100500;
            var locale = "ru";

            var userIds = 0;
            var userFaker = new Faker<User>(locale)
                .RuleFor(u => u.Id, f => userIds++)
                .RuleFor(u => u.Guid, f => f.Random.Guid())
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber(GetRandomPhoneFormat(locale, f.Random)))
                .FinishWith((f, u) => 
                {
                    //u.Address = GetRandomAddress(f);
                });

            User MakeUser(int seed)
            {
                return userFaker.UseSeed(seed).Generate();
            }

            return Enumerable.Range(seed, 20).Select(MakeUser);
        }

        private string GetRandomPhoneFormat(string locale, Randomizer randomizer)
        {
            var defaultPhoneFormat = "###-###-####";
            if (phoneFormats.TryGetValue(locale, out string[] localePhoneFormats))
            {
                return randomizer.ArrayElement(localePhoneFormats);
            }
            else return defaultPhoneFormat;
        }

        /*private string GetRandomAddress(Faker faker)
        {
            return faker.Random.ArrayElement(
            [
                $"{faker.Address.ZipCode()} {faker.Address.City()} {faker.Address.StreetName()} {faker.Address.BuildingNumber()}-{faker.Random.Number(1, 300)}",
                $"{faker.Address.ZipCode()} {faker.Address.City()} {faker.Address.StreetName()} {faker.Address.BuildingNumber()}",
                $"{faker.Address.ZipCode()} {faker.Address.City()} {faker.Address.StreetAddress()}",
                $"{faker.Address.City()} {faker.Address.StreetName()} {faker.Address.BuildingNumber()}-{faker.Random.Number(1, 300)}",
                $"{faker.Address.City()} {faker.Address.StreetName()} {faker.Address.BuildingNumber()}",
                $"{faker.Address.City()} {faker.Address.StreetName()} {faker.Address.StreetAddress()}",
            ]);
        }*/
    }

    public interface IUsersService
    {
        IEnumerable<User> GetUsers();
    }
}
