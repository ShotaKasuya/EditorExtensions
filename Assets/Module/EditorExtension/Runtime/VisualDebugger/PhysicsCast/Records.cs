using System;
using UnityEngine;

namespace Module.EditorExtension.Runtime.VisualDebugger.PhysicsCast;

internal readonly record struct RayRecord(CommonRecord CommonRecord);

internal readonly record struct SphereRecord(float Radius, CommonRecord CommonRecord);

internal readonly struct CommonRecord : IEquatable<CommonRecord>
{
    public Ray Ray { get; }
    public float CheckDistance { get; }
    public bool IsHit { get; }

    public CommonRecord(Ray ray, float checkDistance, bool isHit)
    {
        Ray = ray;
        CheckDistance = checkDistance;
        IsHit = isHit;
    }

    public bool Equals(CommonRecord other)
    {
        return Equal(Ray, other.Ray) && CheckDistance.Equals(other.CheckDistance) && IsHit == other.IsHit;
    }

    private bool Equal(Ray rhs, Ray lhs)
    {
        return rhs.origin.Equals(lhs.origin) && rhs.direction.Equals(lhs.direction);
    }

    public override bool Equals(object obj)
    {
        return obj is CommonRecord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ray.direction, Ray.origin, CheckDistance, IsHit);
    }
}