
namespace RedPoint.ReefStatus.Common
{
	using System;
	using System.Collections;
	using System.Collections.ObjectModel;

 #if PocketPC || Mono
	
    public class ListCollectionView : IEnumerable
    {
        private IList list;

        public class ListCollectionEnumerator : IEnumerator
        {
            private IList list;
            private Predicate<object> filter;
            private int currentIndex;

            public ListCollectionEnumerator(IList list, Predicate<object> filter)
            {
                this.list = list;
                this.filter = filter;
                Reset();
            }

            #region IEnumerator Members

            public object Current
            {
                get
                {
                    if (currentIndex == -1)
                    {
                        this.MoveNext();
                    }

                    if(list.Count <= currentIndex)
                    {
                        return null;
                    }
                    object item = list[currentIndex];
                    if (filter(item))
                        return item;

                    return null;
                }
            }

            public bool MoveNext()
            {
                currentIndex++;

                if (list.Count <= currentIndex)
                {
                    return false;
                }

                while (!filter(list[currentIndex]))
                {
                    currentIndex++;
                    if(list.Count <=  currentIndex)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void Reset()
            {
                currentIndex = -1;
            }

            #endregion
        }

        public ListCollectionView(IList list)
        {
            this.list = list;
        }

        public Predicate<Object> Filter { get; set; }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (object item in list)
                {
                    if (Filter(item))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new ListCollectionEnumerator(list, this.Filter);
        }

        #endregion
    }
#endif
}

