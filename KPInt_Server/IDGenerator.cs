using System;
using System.Collections.Generic;

namespace KPInt_Server
{
    class IDGenerator
    {
        private readonly Random random = new Random();
        private readonly HashSet<int> used = new HashSet<int>();

        public int GetID()
        {
            while (true)
            {
                var id = random.Next();
                if (!used.Contains(id))
                {
                    used.Add(id);
                    return id;
                }
            }
        }
    }
}
