using UnityEngine;

public class MaterialColorTweenProperty : AbstractColorTweenProperty
{
	private string _materialColorName;

	public MaterialColorTweenProperty(Color endValue, string colorName = "_Color", bool isRelative = false)
		: base(endValue, isRelative)
	{
		_materialColorName = colorName;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _materialColorName == ((MaterialColorTweenProperty)obj)._materialColorName;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = _target.GetColor(_materialColorName);
		}
		else
		{
			_startValue = _target.GetColor(_materialColorName);
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Color color = GoTweenUtils.unclampedColorLerp(_startValue, _diffValue, value);
		_target.SetColor(_materialColorName, color);
	}
}
