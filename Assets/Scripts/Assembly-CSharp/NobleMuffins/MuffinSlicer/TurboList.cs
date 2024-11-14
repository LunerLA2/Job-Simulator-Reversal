using System;

namespace NobleMuffins.MuffinSlicer
{
	public class TurboList<T>
	{
		private T[] content;

		private int capacity;

		private int nextFigure;

		public int Count
		{
			get
			{
				return nextFigure;
			}
			set
			{
				nextFigure = value;
			}
		}

		public T[] array
		{
			get
			{
				return content;
			}
		}

		public T this[int i]
		{
			get
			{
				return content[i];
			}
		}

		public TurboList(T[] copySource)
		{
			capacity = copySource.Length;
			content = new T[copySource.Length];
			Array.Copy(copySource, content, capacity);
			nextFigure = 0;
		}

		public TurboList(int _capacity)
		{
			capacity = _capacity;
			content = new T[capacity];
			nextFigure = 0;
		}

		public T[] ToArray()
		{
			T[] array = new T[nextFigure];
			Array.Copy(content, array, nextFigure);
			return array;
		}

		public void EnsureCapacity(int i)
		{
			if (i > capacity)
			{
				T[] destinationArray = new T[i];
				Array.Copy(content, destinationArray, capacity);
				content = destinationArray;
				capacity = i;
			}
		}

		public void AddArray(T[] source)
		{
			if (source.Length + nextFigure > capacity)
			{
				int num = capacity * 3 / 2 + source.Length;
				T[] destinationArray = new T[num];
				Array.Copy(content, destinationArray, capacity);
				content = destinationArray;
				capacity = num;
			}
			Array.Copy(source, 0, content, nextFigure, source.Length);
			nextFigure += source.Length;
		}
	}
}
