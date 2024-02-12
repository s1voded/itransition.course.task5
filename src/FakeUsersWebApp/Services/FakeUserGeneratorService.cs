﻿using Bogus;
using FakeUsersWebApp.Model;
using static Bogus.DataSets.Name;

namespace FakeUsersWebApp.Services
{
    public class FakeUserGeneratorService : IFakeUsersService
    {
        private readonly IErrorService _errorService;

        private readonly Dictionary<Gender, string[]> dictionaryMiddleNames = [];
        private readonly Dictionary<string, string[]> dictionaryPhoneFormats = new()
        {
            { "ru" , ["+7 ### ##-##-##", "8 (###) ###-##-##"] },
            { "en", ["+1 ### ### ####", "(###) ###-####"] },
            { "de", ["+49 ### ### ####", "0049 (###) #######"]}
        };

        public FakeUserGeneratorService(IErrorService errorService)
        {
            _errorService = errorService;
        }

        public IEnumerable<User> GetUsers(int countUsers, string locale, float countErrorsPerRecord, int seed)
        {
            LoadMiddleNamesFromFile(locale);
            var userFaker = InitUserFaker(locale, countErrorsPerRecord);
            return Enumerable.Range(seed, countUsers).Select(s => MakeUser(s, userFaker));
        }

        private Faker<User> InitUserFaker(string locale, float countErrorsPerRecord)
        {
            var userIds = 1;
            var userFaker = new Faker<User>(locale)
                .RuleFor(u => u.Id, f => userIds++)
                .RuleFor(u => u.Guid, f => f.Random.Guid())
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.MiddleName, GetRandomMiddleName)
                .RuleFor(u => u.Address, GetRandomAddress)
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber(GetRandomPhoneFormat(locale, f.Random)))
                .FinishWith((f, u) =>
                {
                    //var func = f.Random.ListItem(_errorService.ListErrorFunc);
                    _errorService.CreateErrors(u, countErrorsPerRecord, f);
                });
            return userFaker;
        }

        private User MakeUser(int seed, Faker<User> userFaker)
        {
            return userFaker.UseSeed(seed).Generate();
        }

        private string GetRandomPhoneFormat(string locale, Randomizer randomizer)
        {
            var defaultPhoneFormat = "###-###-####";
            if (dictionaryPhoneFormats.TryGetValue(locale, out string[]? localePhoneFormats))
            {
                return randomizer.ArrayElement(localePhoneFormats);
            }
            else return defaultPhoneFormat;
        }

        private string GetRandomMiddleName(Faker faker)
        {
            var middleName = "";
            if (dictionaryMiddleNames.TryGetValue(faker.Person.Gender, out string[]? allMiddleNames))
            {
                return faker.Random.ArrayElement(allMiddleNames);
            }
            else return middleName;
        }

        private void LoadMiddleNamesFromFile(string locale)
        {
            dictionaryMiddleNames.Clear();
            foreach (Gender gender in Enum.GetValues(typeof(Gender)))
            {
                var path = Path.Combine($"Data/{locale}_{gender}_middlename.txt");
                if (File.Exists(path))
                {
                    dictionaryMiddleNames.Add(gender, File.ReadAllLines(path));
                }
            }
        }

        private string GetRandomAddress(Faker faker)
        {
            var random = faker.Random;
            var address = faker.Address;
            return $"{random.ArrayElement([$"{address.ZipCode()} ", ""])}{address.City()} {address.StreetAddress()}{random.ArrayElement([$"-{random.Number(1, 300)}", ""])}";
        }
    }

    public interface IFakeUsersService
    {
        IEnumerable<User> GetUsers(int countUsers, string locale, float countErrorsPerRecord, int seed);
    }
}
