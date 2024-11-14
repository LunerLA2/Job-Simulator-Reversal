public struct CIEXYZ
{
	public static readonly CIEXYZ Empty = default(CIEXYZ);

	public static readonly CIEXYZ D65 = new CIEXYZ(0.9505f, 1f, 1.089f);

	private float x;

	private float y;

	private float z;

	public float X
	{
		get
		{
			return x;
		}
		set
		{
			x = ((value > 0.9505f) ? 0.9505f : ((!(value < 0f)) ? value : 0f));
		}
	}

	public float Y
	{
		get
		{
			return y;
		}
		set
		{
			y = ((value > 1f) ? 1f : ((!(value < 0f)) ? value : 0f));
		}
	}

	public float Z
	{
		get
		{
			return z;
		}
		set
		{
			z = ((value > 1.089f) ? 1.089f : ((!(value < 0f)) ? value : 0f));
		}
	}

	public CIEXYZ(float x, float y, float z)
	{
		this.x = ((x > 0.9505f) ? 0.9505f : ((!(x < 0f)) ? x : 0f));
		this.y = ((y > 1f) ? 1f : ((!(y < 0f)) ? y : 0f));
		this.z = ((z > 1.089f) ? 1.089f : ((!(z < 0f)) ? z : 0f));
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return this == (CIEXYZ)obj;
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
	}

	public static bool operator ==(CIEXYZ item1, CIEXYZ item2)
	{
		return item1.X == item2.X && item1.Y == item2.Y && item1.Z == item2.Z;
	}

	public static bool operator !=(CIEXYZ item1, CIEXYZ item2)
	{
		return item1.X != item2.X || item1.Y != item2.Y || item1.Z != item2.Z;
	}
}
