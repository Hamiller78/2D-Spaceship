namespace SpaceGame.Utlities;

using Godot;
using System;

/// <summary>
/// Represents an angle saving the value internally in degrees.
/// Contains help functions to find the shortest path between two angles.pathes between angles.
/// IMPORTANT: The Godot engine has the y axis pointing down, so angles are measured clockwise.
/// </summary>
public partial class Angle
{
    public float InDegrees
    {
        get => _angle;
        set
        {
            _angle = value;
            _angle = _angle % 360f;
            if (_angle < 0f)
            {
                _angle += 360f;
            }
            else if (_angle >= 360f)
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

    public Angle() { }

    public Angle(float angle)
    {
        InDegrees = angle;
    }

    public static Angle FromMathCoordSystem(float angle)
    {
        return new Angle(-angle);
    }

    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle(a.InDegrees + b.InDegrees);
    }

    public static Angle operator -(Angle a, Angle b)
    {
        return new Angle(a.InDegrees - b.InDegrees);
    }

    public int GetTurnDirection(Angle targetAngle, Angle minAngle, Angle maxAngle)
    {
        if (!IsBetween(minAngle, maxAngle))
        {
            GD.PrintErr("Angle is not between min and max angle");
            GD.PrintErr($"Angle: {InDegrees}, Min: {minAngle.InDegrees}, Max: {maxAngle.InDegrees}");
            return 0;
        }

        var angleToMin = GetCounterClockwiseDifference(minAngle);
        var angleToMax = GetClockwiseDifference(maxAngle);
        var clockwiseDifference = GetClockwiseDifference(targetAngle);
        var counterClockwiseDifference = GetCounterClockwiseDifference(targetAngle);

        if (clockwiseDifference <= counterClockwiseDifference)
        {
            if (clockwiseDifference <= angleToMax)
            {
                return 1;
            }
            else if (counterClockwiseDifference <= angleToMin)
            {
                return -1;
            }
        }

        if (counterClockwiseDifference < clockwiseDifference)
        {
            if (counterClockwiseDifference <= angleToMin)
            {
                return -1;
            }
            else if (clockwiseDifference <= angleToMax)
            {
                return 1;
            }
        }

        return 0;
    }

    public bool IsBetween(Angle minAngle, Angle maxAngle)
    {
        var angleMinToThis = GetCounterClockwiseDifference(minAngle);
        var angleThisToMax = GetClockwiseDifference(maxAngle);

        return angleMinToThis + angleThisToMax <= 360f;
    }

    public float GetClockwiseDifference(Angle otherAngle)
    {
        if (InDegrees < otherAngle.InDegrees)
        {
            return Math.Abs(otherAngle.InDegrees - InDegrees);
        }
        else
        {
            return Math.Abs(otherAngle.InDegrees - InDegrees + 360f);
        }
    }

    public float GetCounterClockwiseDifference(Angle otherAngle)
    {
        if (InDegrees < otherAngle.InDegrees)
        {
            return Math.Abs(otherAngle.InDegrees - InDegrees - 360f);
        }
        else
        {
            return Math.Abs(otherAngle.InDegrees - InDegrees);
        }
    }
}