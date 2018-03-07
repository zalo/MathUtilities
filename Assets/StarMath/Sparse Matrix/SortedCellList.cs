using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarMathLib.Sparse_Matrix
{
    internal class SortedCellList : IEnumerable<KeyValuePair<int, SparseCell>>
    {

        internal int LastIndex { get; private set; }
        internal int FirstIndex { get; private set; }

        internal int Count { get; private set; }

        List<int> IndexKeys { get; } = new List<int>();

        List<SparseCell> Cells { get; } = new List<SparseCell>();


        internal void Add(int index, SparseCell cell)
        {
            if (Count == 0)
            {
                IndexKeys.Add(index);
                FirstIndex = LastIndex = index;
                Count = 1;
                Cells.Add(cell);
            }
            else if (index > LastIndex)
            {
                IndexKeys.Add(index);
                Cells.Add(cell);
                LastIndex = index;
                Count++;
            }
            else if (index < FirstIndex)
            {
                IndexKeys.Insert(0, index);
                Cells.Insert(0, cell);
                FirstIndex = index;
                Count++;
            }
            else //inserting time at some intermediate value
            {
                var upper = Count - 1;
                var lower = 0;
                var i = lower + (upper - lower) / 2;
                while (true)
                {
                    if (index > IndexKeys[i])
                    {
                        if (lower == i)
                        {
                            i++;
                            break;
                        }
                        lower = i;
                    }
                    else if (index > IndexKeys[i - 1]) break;
                    else upper = i;
                    i = lower + (upper - lower) / 2;
                }
                IndexKeys.Insert(i, index);
                Cells.Insert(i, cell);
                Count++;
            }
        }

        internal KeyValuePair<int, SparseCell> Pop()
        {
            var top = new KeyValuePair<int, SparseCell>(IndexKeys[0], Cells[0]);
            IndexKeys.RemoveAt(0);
            Cells.RemoveAt(0);
            Count--;
            if (Count>0) FirstIndex = IndexKeys[0];
            return top;
        }

        public IEnumerator<KeyValuePair<int, SparseCell>> GetEnumerator()
        {
            return new SortedCellEnumerator(IndexKeys.ToArray(), Cells.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool PositionIfExists(int searchIndex, out int position)
        {
            if (Count == 0 || searchIndex < FirstIndex)
            {
                position = 0;
                return false;
            }
            else if (searchIndex > LastIndex)
            {
                position = Count;
                return false;
            }
            else if (searchIndex == FirstIndex)
            {
                position = 0;
                return true;
            }
            else if (searchIndex == LastIndex)
            {
                position = Count - 1;
                return true;
            }
            else //inserting time at some intermediate value
            {
                var upper = Count-1;
                var lower = 0;
                while (true)
                {
                  var  i = lower + (upper - lower) / 2;
                    if (searchIndex == IndexKeys[i])
                    {
                        position = i;
                        return true;
                    }
                    if (searchIndex > IndexKeys[i])
                    {
                        if (lower == i)
                        {
                            position = i+1;
                            return false;
                        }
                        lower = i;
                    }
                    else if (searchIndex > IndexKeys[i - 1])
                    {
                        position = i;
                        return false;
                    }
                    else upper = i;
                }
            }
        }

        internal SparseCell this[int position] => Cells[position];

        internal void Insert(int position, int newColIndex, CholeskyLCell newCell)
        {
            if (position == 0) FirstIndex = newColIndex;
            if (position == Count) LastIndex = newColIndex;
            IndexKeys.Insert(position, newColIndex);
            Cells.Insert(position, newCell);
            Count++;
        }
    }

    class SortedCellEnumerator : IEnumerator<KeyValuePair<int, SparseCell>>
    {
        private readonly SparseCell[] cells;
        private readonly int[] indices;

        int _position = -1;
        private readonly int _length;
        internal SortedCellEnumerator(int[] timeKeys, SparseCell[] parameterValues)
        {
            indices = timeKeys;
            cells = parameterValues;
            _length = timeKeys.GetLength(0);
        }

        public bool MoveNext()
        {
            _position++;
            return _position < _length;
        }

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        object IEnumerator.Current => Current;

        public KeyValuePair<int, SparseCell> Current => new KeyValuePair<int, SparseCell>(indices[_position], cells[_position]);
    }

}