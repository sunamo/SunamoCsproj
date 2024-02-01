using SunamoCollectionWithoutDuplicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj._sunamo
{
    internal class CAG
    {
        public static List<T> GetDuplicities<T>(List<T> clipboardL)
        {
            List<T> alreadyProcessed;
            return GetDuplicities<T>(clipboardL, out alreadyProcessed);
        }

        internal static List<T> GetDuplicities<T>(List<T> clipboardL, out List<T> alreadyProcessed)
        {
            alreadyProcessed = new List<T>(clipboardL.Count);
            CollectionWithoutDuplicates<T> duplicated = new CollectionWithoutDuplicates<T>();
            foreach (var item in clipboardL)
            {
                if (alreadyProcessed.Contains(item))
                {
                    duplicated.Add(item);
                }
                else
                {
                    alreadyProcessed.Add(item);
                }
            }
            return duplicated.c;
        }
    }
}
