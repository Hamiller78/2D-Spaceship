namespace SpaceGame.Utlities;

using Godot;
using System;

public class NavigationManager
{
    public enum TurnDirection
    {
        None = 0,
        Clockwise = 1,
        CounterClockwise = -1
    }

    public static Angle GetGlobalAngleToTarget(Vector2 source, Vector2 target, float deltaGlobalRotation)
    {
        var angleInRads = source.AngleToPoint(target);
        var angle = new Angle() { InRadians = angleInRads } - new Angle(deltaGlobalRotation);
        return angle;
    }

    public static Angle GetAngleToTarget(Vector2 source, Vector2 target)
    {
        var angleInRads = source.AngleToPoint(target);
        return new Angle() { InRadians = angleInRads };
    }

    public static float GetNewRotation(
        float currentRotationDegrees,
        float targetRotationDegrees,
        float rotationSpeed,
        float minRotation,
        float maxRotation,
        double delta)
    {
        var currentAngle = new Angle(currentRotationDegrees);
        var targetAngle = new Angle(targetRotationDegrees);

        var maxDeltaDegrees = rotationSpeed * (float)delta;

        // Calculates shortest allowed path to target rotation
        var turnDirection = GetTurnDirection(currentAngle, targetAngle, minRotation, maxRotation);
        if (turnDirection == TurnDirection.None)
        {
            return currentRotationDegrees;
        }

        float absDegreesToTarget = 0f;
        if (turnDirection == TurnDirection.Clockwise)
        {
            absDegreesToTarget = GetClockwiseAbsDifference(currentAngle, targetAngle);
        }
        else if (turnDirection == TurnDirection.CounterClockwise)
        {
            absDegreesToTarget = GetCounterClockwiseAbsDifference(currentAngle, targetAngle);
        }

        if (absDegreesToTarget < maxDeltaDegrees)
        {
            return targetRotationDegrees;
        }

        return currentRotationDegrees + (maxDeltaDegrees * (int)turnDirection);
    }

    private static TurnDirection GetTurnDirection(Angle angle, Angle targetAngle, float minRotation, float maxRoation)
    {
        var shortPathDirection = GetShortestPathDirection(angle, targetAngle);
        var longPathDirection
            = shortPathDirection == TurnDirection.Clockwise
                ? TurnDirection.CounterClockwise
                : TurnDirection.Clockwise;

        if (IsPathFree(angle, targetAngle, shortPathDirection, minRotation, maxRoation))
        {
            return shortPathDirection;
        }
        else if (IsPathFree(angle, targetAngle, longPathDirection, minRotation, maxRoation))
        {
            return longPathDirection;
        }
        else
        {
            return TurnDirection.None;
        }
    }

    private static bool IsPathFree(
        Angle angle,
        Angle targetAngle,
        TurnDirection turnDirection,
        float minAngle,
        float maxAngle)
    {
        if (minAngle == 0f && maxAngle == 360f)
        {
            return true;
        }

        switch (turnDirection)
        {
            case TurnDirection.None:
                return true;
            case TurnDirection.Clockwise:
                return IsBetween(targetAngle, angle.InDegrees, maxAngle);
            case TurnDirection.CounterClockwise:
                return IsBetween(targetAngle, minAngle, angle.InDegrees);
            default:
                return false;
        }
    }

    private static bool IsBetween(Angle angle, float minRoation, float maxRotation)
    {
        var minAngle = new Angle(minRoation);
        var maxAngle = new Angle(maxRotation);

        var angleMinToThis = GetCounterClockwiseAbsDifference(angle, minAngle);
        var angleThisToMax = GetClockwiseAbsDifference(angle, maxAngle);

        return angleMinToThis + angleThisToMax <= 360f;
    }

    private static TurnDirection GetShortestPathDirection(Angle angle, Angle targetAngle)
    {
        var clockwiseDifference = GetClockwiseAbsDifference(angle, targetAngle);
        var counterClockwiseDifference = GetCounterClockwiseAbsDifference(angle, targetAngle);

        if (clockwiseDifference <= counterClockwiseDifference)
        {
            return TurnDirection.Clockwise;
        }
        else
        {
            return TurnDirection.CounterClockwise;
        }
    }

    private static float GetClockwiseAbsDifference(Angle angle, Angle otherAngle)
    {
        var diffAngle = otherAngle - angle;
        return diffAngle.InDegrees;
    }

    private static float GetCounterClockwiseAbsDifference(Angle angle, Angle otherAngle)
    {
        var diffAngle = angle - otherAngle;
        return diffAngle.InDegrees;
    }
}