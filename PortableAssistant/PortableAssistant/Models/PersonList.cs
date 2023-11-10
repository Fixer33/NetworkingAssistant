using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortableAssistant.Models
{
    [Serializable]
    internal class PersonList
    {
        public List<Person> Persons => _list;

        [JsonProperty] private List<Person> _list;

        public PersonList()
        {
            _list = new List<Person>();
        }

        public void Add(Person person)
        {
            _list.Add(person);
        }

        public void Remove(Person person)
        {
            _list.Remove(person);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static PersonList Get(object from = null)
        {
            PersonList result = new PersonList();

            if (from == null)
                return result;

            if (from is string json)
            {
                try
                {
                    result = JsonConvert.DeserializeObject<PersonList>(json);
                }
                catch
                {
                    
                }
            }

            return result;
        }
    }
}
