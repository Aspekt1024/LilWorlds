using System.Collections.Generic;
using System.Linq;

namespace Aspekt.Spheres
{
    public class FixedSizeList<T>
    {
        private readonly T[] list;
        private int nextIndex;
        
        public T[] Items => list;
        public int NextIndex => nextIndex;
        
        public FixedSizeList(int size)
        {
            list = new T[size];
        }

        public void Add(T item)
        {
            list[nextIndex] = item;
            nextIndex++;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}