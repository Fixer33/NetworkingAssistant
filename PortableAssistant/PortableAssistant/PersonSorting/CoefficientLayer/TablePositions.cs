using PortableAssistant.Models;
using System.Collections.Generic;

namespace PortableAssistant.PersonSorting.CoefficientLayer
{
    public class TablePositions
    {
        public int TableCount { get; private set; }
        public List<TablesLayer> Layers { get; private set; }

        public TablePositions(int tableCount)
        {
            TableCount = tableCount;
            Layers = new List<TablesLayer>();
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
            Layers[Layers.Count - 1].AddPerson(person);
            return true;
        }

        public List<Person> GetTablePeople(int tableIndex)
        {
            List<Person> result = new List<Person>();
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
}
