using System.Text;
using UnityEngine;

public class ElementSequence<T>
{
	public T[] elements;

	public int[] sequence;

	private int length;

	private int current;

	private int last;

	public int Length
	{
		get
		{
			return length;
		}
	}

	public ElementSequence(T[] _elements)
	{
		if (_elements.Length > 0)
		{
			InitializeSequence(_elements.Length);
			_elements.CopyTo(elements, 0);
		}
	}

	public void InitializeSequence(int _length)
	{
		last = -1;
		length = _length;
		sequence = new int[length];
		elements = new T[length];
		ResetSequence();
	}

	public void ResetSequence()
	{
		current = 0;
		for (int i = 0; i < length; i++)
		{
			sequence[i] = i;
		}
		ShuffleSequence();
	}

	public bool IsDone()
	{
		return current >= length;
	}

	public void ShuffleSequence()
	{
		for (int i = 0; i < length; i++)
		{
			int num = Random.Range(i, length);
			int num2 = sequence[i];
			sequence[i] = sequence[num];
			sequence[num] = num2;
		}
		if (last == sequence[0])
		{
			sequence[0] = sequence[length - 1];
			sequence[length - 1] = last;
		}
		last = sequence[length - 1];
	}

	public T Next()
	{
		if (IsDone())
		{
			return default(T);
		}
		return elements[sequence[current++]];
	}

	public T Current()
	{
		if (current == 0)
		{
			return default(T);
		}
		return elements[sequence[current - 1]];
	}

	public T GetNext()
	{
		if (IsDone())
		{
			ResetSequence();
		}
		return elements[sequence[current++]];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append(sequence[i].ToString() + ',');
		}
		return stringBuilder.ToString();
	}
}
