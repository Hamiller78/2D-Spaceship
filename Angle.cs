namespace SpaceGame.Utlities;

using Godot;
using System;

public partial class Angle : Node
{
    public float InDegrees
    {
        get => _angle;
        set
        {
            _angle = value;
            _angle = _angle % 360f;
            if (_angle < -180f)
            {
                _angle += 360f;
            }
            else if (_angle > 180f)
            {
                _angle -= 360f;
            }
        }
    }

    public float InRadians
    {
        get => _angle * (float)Math.PI / 180f;
        set => InDegrees = value * 180f / (float)Math.PI;
    }

    private float _angle = 0f;

    public Angle() {}

    public Angle(float angle)
    {
        InDegrees = angle;
    }

    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle(a.InDegrees + b.InDegrees);
    }

    public static Angle operator -(Angle a, Angle b)
    {
        return new Angle(a.InDegrees - b.InDegrees);
    }
}
