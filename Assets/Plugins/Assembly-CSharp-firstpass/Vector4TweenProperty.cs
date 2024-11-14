using System;
using UnityEngine;

public class Vector4TweenProperty : AbstractTweenProperty, IGenericProperty
{
	private Action<Vector4> _setter;

	protected Vector4 _originalEndValue;

	protected Vector4 _startValue;

	protected Vector4 _endValue;

	protected Vector4 _diffValue;

	public string propertyName { get; private set; }

	public Vector4TweenProperty(string propertyName, Vector4 endValue, bool isRelative = false)
		: base(isRelative)
	{
		this.propertyName = propertyName;
		_originalEndValue = endValue;
	}

	public override bool validateTarget(object target)
	{
		_setter = GoTweenUtils.setterForProperty<Action<Vector4>>(target, propertyName);
		return _setter != null;
	}

	public override void prepareForUse()
	{
		Func<Vector4> func = GoTweenUtils.getterForProperty<Func<Vector4>>(_ownerTween.target, propertyName);
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_endValue = _startValue;
			_endValue = func();
		}
		else
		{
			_startValue = func();
		}
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _endValue;
		}
		else
		{
			_diffValue = _endValue - _startValue;
		}
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector4 obj = GoTweenUtils.unclampedVector4Lerp(_startValue, _diffValue, value);
		_setter(obj);
	}
}
