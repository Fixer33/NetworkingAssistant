using PortableAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PortableAssistant.Services
{
    public class PersonsDataStore : IDataStore<Person>
    {
        private const string SAVE_KEY = "persons_save_data";

        readonly PersonList persons;

        public PersonsDataStore()
        {
            if (Application.Current.Properties.TryGetValue(SAVE_KEY, out var saved))
            {
                persons = PersonList.Get(saved);
            }
            else
            {
                persons = new PersonList();
                Application.Current.Properties.Add(SAVE_KEY, persons.Serialize());
            }
        }

        public async Task<bool> AddItemAsync(Person item)
        {
            persons.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Person item)
        {
            var oldItem = persons.Persons.Where((Person arg) => arg.Name == item.Name).FirstOrDefault();
            persons.Remove(oldItem);
            persons.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string name)
        {
            var oldItem = persons.Persons.Where((Person arg) => arg.Name == name).FirstOrDefault();
            persons.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Person> GetItemAsync(string name)
        {
            return await Task.FromResult(persons.Persons.FirstOrDefault(s => s.Name == name));
        }

        public async Task<IEnumerable<Person>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(persons.Persons);
        }
    }
}