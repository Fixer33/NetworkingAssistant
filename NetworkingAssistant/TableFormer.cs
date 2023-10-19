using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingAssistant
{
    public class TableFormer
    {
        public static (TablePositions result, List<Person> ignored) GetPositions(int tableCount, List<Person> people)
        {
            List<Person> sortedPeople = people.OrderBy(i => i.Coefficient).ToList();
            Queue<Person> queue = new Queue<Person>();
            for (int i = 0; i < sortedPeople.Count; i++)
            {
                queue.Enqueue(sortedPeople[i]);
            }

            List<Person> ignored = new();
            TablePositions result = new TablePositions(tableCount);
            for (int i = 0; i < people.Count; i++)
            {
                var p = queue.Dequeue();
                if (result.AddPerson(p) == false)
                {
                    ignored.Add(p);
                }
            }

            return new(result, ignored);
        }

        public class TablePositions
        {
            public int TableCount { get; private set; }
            public List<TablesLayer> Layers { get; private set; }

            public TablePositions(int tableCount)
            {
                TableCount = tableCount;
                Layers = new();
            }

            public bool AddPerson(Person person)
            {
                for (int i = 0; i < Layers.Count; i++)
                {
                    if (Layers[i].AddPerson(person))
                    {
                        return true;
                    }
                }

                Layers.Add(new TablesLayer(TableCount));
                Layers[^1].AddPerson(person);
                return true;
            }

            public List<Person> GetTablePeople(int tableIndex)
            {
                List<Person> result = new();
                for (int i = 0; i < Layers.Count; i++)
                {
                    result.Add(Layers[i].GetPersonByTableIndex(tableIndex));
                }
                return result;
            }

            public void MoveLayers()
            {
                for (int i = 0; i < Layers.Count; i++)
                {
                    if (i == TableCount)
                    {
                        Layers[i].Randomize();
                    }
                    else
                    {
                        Layers[i].Move(i);
                    }

                }
            }
        }

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

        public class Person
        {
            public string Name { get; private set; }
            public int Coefficient { get; private set; }

            public Person(string name, int coefficient)
            {
                Name = name;
                Coefficient = coefficient;
            }

        }
    }

    [Serializable]
    public struct PersonArray
    {
        public List<SerializablePerson> People;
    }

    [Serializable]
    public struct SerializablePerson
    {
        public string Name;
        public int Coefficient;
    }
}
