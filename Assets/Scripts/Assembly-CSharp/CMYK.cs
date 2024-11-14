public struct CMYK
{
	public static readonly CMYK Empty = default(CMYK);

	private float c;

	private float m;

	private float y;

	private float k;

	public float Cyan
	{
		get
		{
			return c;
		}
		set
		{
			c = value;
			c = ((c > 1f) ? 1f : ((!(c < 0f)) ? c : 0f));
		}
	}

	public float Magenta
	{
		get
		{
			return m;
		}
		set
		{
			m = value;
			m = ((m > 1f) ? 1f : ((!(m < 0f)) ? m : 0f));
		}
	}

	public float Yellow
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
			y = ((y > 1f) ? 1f : ((!(y < 0f)) ? y : 0f));
		}
	}

	public float Black
	{
		get
		{
			return k;
		}
		set
		{
			k = value;
			k = ((k > 1f) ? 1f : ((!(k < 0f)) ? k : 0f));
		}
	}

	public CMYK(float c, float m, float y, float k)
	{
		this.c = c;
		this.m = m;
		this.y = y;
		this.k = k;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return this == (CMYK)obj;
	}

	public override int GetHashCode()
	{
		return Cyan.GetHashCode() ^ Magenta.GetHashCode() ^ Yellow.GetHashCode() ^ Black.GetHashCode();
	}

	public static bool operator ==(CMYK item1, CMYK item2)
	{
		return item1.Cyan == item2.Cyan && item1.Magenta == item2.Magenta && item1.Yellow == item2.Yellow && item1.Black == item2.Black;
	}

	public static bool operator !=(CMYK item1, CMYK item2)
	{
		return item1.Cyan != item2.Cyan || item1.Magenta != item2.Magenta || item1.Yellow != item2.Yellow || item1.Black != item2.Black;
	}
}
