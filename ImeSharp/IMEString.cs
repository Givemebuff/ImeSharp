using System;
using System.Collections;
using System.Collections.Generic;

namespace ImeSharp
{
    public unsafe struct IMEString : IEnumerable<char>
    {
        internal const int IMECharBufferSize = 64;

        public static readonly IMEString Empty = new IMEString((List<char>)null);

        internal struct Enumerator : IEnumerator<char>
        {
            private IMEString _imeString;
            private char _currentCharacter;
            private int _currentIndex;

            public Enumerator(IMEString imeString)
            {
                _imeString = imeString;
                _currentCharacter = '\0';
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                int size = _imeString.Count;

                _currentIndex++;

                if (_currentIndex == size)
                    return false;

                fixed (char* ptr = _imeString.buffer)
                {
                    _currentCharacter = *(ptr + _currentIndex);
                }

                return true;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose()
            {
            }

            public char Current { get { return _currentCharacter; } }
            object IEnumerator.Current { get { return Current; } }
        }

        public int Count { get { return _size; } }

        public char this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException("index");

                fixed (char* ptr = buffer)
                {
                    return *(ptr + index);
                }
            }
        }

        private int _size;

        fixed char buffer[IMECharBufferSize];

        public IMEString(string characters)
        {
            if (string.IsNullOrEmpty(characters))
            {
                _size = 0;
                return;
            }

            _size = characters.Length;
            if (_size > IMECharBufferSize)
                _size = IMECharBufferSize - 1;

            fixed (char* _ptr = buffer)
            {
                char* ptr = _ptr;
                for (var i = 0; i < _size; i++)
                {
                    *ptr = characters[i];
                    ptr++;
                }
            }
        }

        public IMEString(List<char> characters)
        {
            if (characters == null || characters.Count == 0)
            {
                _size = 0;
                return;
            }

            _size = characters.Count;
            if (_size > IMECharBufferSize)
                _size = IMECharBufferSize - 1;

            fixed (char* _ptr = buffer)
            {
                char* ptr = _ptr;
                for (var i = 0; i < _size; i++)
                {
                    *ptr = characters[i];
                    ptr++;
                }
            }
        }

        public IMEString(char[] characters, int count)
        {
            if (characters == null || count <= 0)
            {
                _size = 0;
                return;
            }

            _size = count;
            if (_size > IMECharBufferSize)
                _size = IMECharBufferSize - 1;

            if (_size > characters.Length)
                _size = characters.Length;

            fixed (char* _ptr = buffer)
            {
                char* ptr = _ptr;
                for (var i = 0; i < _size; i++)
                {
                    *ptr = characters[i];
                    ptr++;
                }
            }
        }

        public IMEString(IntPtr bStrPtr)
        {
            if (bStrPtr == IntPtr.Zero)
            {
                _size = 0;
                return;
            }

            var ptrSrc = (char*)bStrPtr;

            int i = 0;

            fixed (char* _ptr = buffer)
            {
                char* ptr = _ptr;

                while (ptrSrc[i] != '\0')
                {
                    *ptr = ptrSrc[i];
                    i++;
                    ptr++;
                }
            }

            _size = i;
        }

        public override string ToString()
        {
            fixed (char* ptr = buffer)
            {
                return new string(ptr, 0, _size);
            }
        }

        public IntPtr ToIntPtr()
        {
            fixed (char* ptr = buffer)
            {
                return (IntPtr)ptr;
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
