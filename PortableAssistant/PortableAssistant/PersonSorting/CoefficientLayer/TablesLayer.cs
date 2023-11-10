using PortableAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortableAssistant.PersonSorting.CoefficientLayer
{
    public class TablesLayer
    {
        public int TableCount { get; private set; }
        public Person[] Persons { get; private set; }

        private Random _random;

        public TablesLayer(int tableCount)
        {
            TableCount = tableCount;
            Persons = new Person[tableCount];
            _random = new Random();
        }

        public bool AddPerson(Person person)
        {
            for (int i = 0; i < Persons.Length; i++)
            {
                if (Persons[i] == null)
                {
                    Persons[i] = person;
                    return true;
                }
            }

            return false;
        }

        public Person GetPersonByTableIndex(int index)
        {
            return Persons[index];
        }

        public void Move(int step)
        {
            Person[] newArray = new Person[Persons.Length];
            for (int i = 0; i < Persons.Length; i++)
            {
                newArray[(i + step) % Persons.Length] = Persons[i];
            }
            Persons = newArray;
        }

        public void Randomize()
        {
            List<Person> list = Persons.ToList();
            while (list.Count > 0)
            {
                int index = _random.Next(0, list.Count);
                Persons[list.Count - 1] = list[index];
                list.RemoveAt(index);
            }
        }
    }
}
