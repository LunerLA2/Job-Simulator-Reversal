public struct CIELab
{
	public static readonly CIELab Empty = default(CIELab);

	private float l;

	private float a;

	private float b;

	public float L
	{
		get
		{
			return l;
		}
		set
		{
			l = value;
		}
	}

	public float A
	{
		get
		{
			return a;
		}
		set
		{
			a = value;
		}
	}

	public float B
	{
		get
		{
			return b;
		}
		set
		{
			b = value;
		}
	}

	public CIELab(float l, float a, float b)
	{
		this.l = l;
		this.a = a;
		this.b = b;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return this == (CIELab)obj;
	}

	public override int GetHashCode()
	{
		return L.GetHashCode() ^ a.GetHashCode() ^ b.GetHashCode();
	}

	public static bool operator ==(CIELab item1, CIELab item2)
	{
		return item1.L == item2.L && item1.A == item2.A && item1.B == item2.B;
	}

	public static bool operator !=(CIELab item1, CIELab item2)
	{
		return item1.L != item2.L || item1.A != item2.A || item1.B != item2.B;
	}
}
