using PortableAssistant.Models;
using System.Collections.Generic;
using System.Linq;

namespace PortableAssistant.PersonSorting.CoefficientLayer
{
    public class CoeffTableFormer
    {
        public static (TablePositions result, List<Person> ignored) GetPositions(int tableCount, List<Person> people)
        {
            List<Person> sortedPeople = people.OrderBy(i => i.TalkingCoef).ToList();
            Queue<Person> queue = new Queue<Person>();
            for (int i = 0; i < sortedPeople.Count; i++)
            {
                queue.Enqueue(sortedPeople[i]);
            }

            List<Person> ignored = new List<Person>();
            TablePositions result = new TablePositions(tableCount);
            for (int i = 0; i < people.Count; i++)
            {
                var p = queue.Dequeue();
                if (result.AddPerson(p) == false)
                {
                    ignored.Add(p);
                }
            }

            return (result, ignored);
        }
    }
}
