using System;
using System.Collections;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class DeserializableList<T> : IEnumerable, ICollection<T>, IList<T>, IEnumerable<T>
	{
		protected List<T> _Data;

		protected string _NextUrl;

		protected string _PreviousUrl;

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return ((ICollection<T>)_Data).IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return _Data.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return _Data[index];
			}
			set
			{
				_Data[index] = value;
			}
		}

		[Obsolete("Use IList interface on the DeserializableList object instead.", false)]
		public List<T> Data
		{
			get
			{
				return _Data;
			}
		}

		public bool HasNextPage
		{
			get
			{
				return !string.IsNullOrEmpty(NextUrl);
			}
		}

		public bool HasPreviousPage
		{
			get
			{
				return !string.IsNullOrEmpty(PreviousUrl);
			}
		}

		public string NextUrl
		{
			get
			{
				return _NextUrl;
			}
		}

		public string PreviousUrl
		{
			get
			{
				return _PreviousUrl;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator1();
		}

		public int IndexOf(T obj)
		{
			return _Data.IndexOf(obj);
		}

		public void Add(T item)
		{
			_Data.Add(item);
		}

		public void Clear()
		{
			_Data.Clear();
		}

		public bool Contains(T item)
		{
			return _Data.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_Data.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _Data.GetEnumerator();
		}

		public void Insert(int index, T item)
		{
			_Data.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return _Data.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_Data.RemoveAt(index);
		}

		private IEnumerator GetEnumerator1()
		{
			return GetEnumerator();
		}
	}
}
