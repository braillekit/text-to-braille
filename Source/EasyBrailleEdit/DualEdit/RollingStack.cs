using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.DualEdit
{
    public class RollingStack<T> : LinkedList<T>
    {

        private int _maxSize;

        public int MaxSize
        {
            get => _maxSize;
            set
            {
                _maxSize = value;
                if (_maxSize <= 0)
                    throw new InvalidOperationException("MaxSize 屬性必須為大於零的正整數!");

                GuardSize();
            }
        }
        

        public RollingStack(int maxSize)
        {
            MaxSize = maxSize;
        }

        public void Push(T instance)
        {
            AddFirst(instance);
            GuardSize();
        }

        private void GuardSize()
        {
            while (Count > _maxSize)
            {
                RemoveLast();
            }                
        }

        public T Pop()
        {
            if (First != null)
            {
                T instance = First.Value;
                RemoveFirst();
                return instance;
            }
            return default(T);
        }

        public T Peek()
        {
            if (First != null)
                return First.Value;
            return default(T);
        }
    }
}
