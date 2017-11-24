using System;

namespace Plus.HabboHotel.Rooms.PathFinding
{
    sealed class MinHeap<T> where T : IComparable<T>
    {
        private int count;
        private int capacity;
        private T temp;
        private T mheap;
        private T[] array;
        private T[] tempArray;

        public int Count
        {
            get { return this.count; }
        }

        public MinHeap() : this(16) { }

        public MinHeap(int capacity)
        {
            this.count = 0;
            this.capacity = capacity;
            array = new T[capacity];
        }

        public void BuildHead()
        {
            int position;
            for (position = (this.count - 1) >> 1; position >= 0; position--)
            {
                this.MinHeapify(position);
            }
        }

        public void Add(T item)
        {
            this.count++;
            if (this.count > this.capacity)
            {
                DoubleArray();
            }
            this.array[this.count - 1] = item;
            int position = this.count - 1;

            int parentPosition = ((position - 1) >> 1);

            while (position > 0 && array[parentPosition].CompareTo(array[position]) > 0)
            {
                temp = this.array[position];
                this.array[position] = this.array[parentPosition];
                this.array[parentPosition] = temp;
                position = parentPosition;
                parentPosition = ((position - 1) >> 1);
            }
        }

        private void DoubleArray()
        {
            this.capacity <<= 1;
            tempArray = new T[this.capacity];
            CopyArray(this.array, tempArray);
            this.array = tempArray;
        }

        private static void CopyArray(T[] source, T[] destination)
        {
            int index;
            for (index = 0; index < source.Length; index++)
            {
                destination[index] = source[index];
            }
        }

        public T ExtractFirst()
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            temp = this.array[0];
            this.array[0] = this.array[this.count - 1];
            this.count--;
            this.MinHeapify(0);
            return temp;
        }

        private void MinHeapify(int position)
        {
            do
            {
                int left = ((position << 1) + 1);
                int right = left + 1;
                int minPosition;

                if (left < count && array[left].CompareTo(array[position]) < 0)
                {
                    minPosition = left;
                }
                else
                {
                    minPosition = position;
                }

                if (right < count && array[right].CompareTo(array[minPosition]) < 0)
                {
                    minPosition = right;
                }

                if (minPosition != position)
                {
                    mheap = this.array[position];
                    this.array[position] = this.array[minPosition];
                    this.array[minPosition] = mheap;
                    position = minPosition;
                }
                else
                {
                    return;
                }

            } while (true);
        }
    }
}
