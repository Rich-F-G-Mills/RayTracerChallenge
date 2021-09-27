namespace FSRayTracer.Geometries

module Cube =

    open System
    open System.Numerics
    open FSRayTracer


    let private checkAxis (origin, direction) =
        let tMinNumerator = -1.0f - origin
        let tMaxNumerator = 1.0f - origin

        let tmin, tmax =
            if MathF.Abs(direction) <= Single.Epsilon then
                (tMinNumerator * Single.PositiveInfinity, tMaxNumerator * Single.PositiveInfinity)
            else
                (tMinNumerator / direction, tMaxNumerator / direction)

        if tmin > tmax then (tmax, tmin) else (tmin, tmax)


    let internal normalAt (touchPoint: Vector4) =
        let maxC =
            Max3 (MathF.Abs(touchPoint.X), MathF.Abs(touchPoint.Y), MathF.Abs(touchPoint.Z))

        if maxC = MathF.Abs(touchPoint.X) then
            Vector4((if touchPoint.X > 0.0f then 1.0f else -1.0f), 0.0f, 0.0f, 0.0f)

        else if maxC = MathF.Abs(touchPoint.Y) then
            Vector4(0.0f, (if touchPoint.Y > 0.0f then 1.0f else -1.0f), 0.0f, 0.0f)

        else
            Vector4(0.0f, 0.0f, (if touchPoint.Z > 0.0f then 1.0f else -1.0f), 0.0f)            


    let private intersector (ray: Ray) =
        let tToIntersectionPoint (t: float32) inside =
            let touchPoint =
                ray.Origin + t * ray.Direction

            let normal = normalAt (touchPoint)

            { Distance = t
              Location = touchPoint
              Normal = if inside then -normal else normal
              Inside = Some inside }

        let xtMin, xtMax = checkAxis(ray.Origin.X, ray.Direction.X)
        let ytMin, ytMax = checkAxis(ray.Origin.Y, ray.Direction.Y)
        let ztMin, ztMax = checkAxis(ray.Origin.Z, ray.Direction.Z)

        let tMin = Max3(xtMin, ytMin, ztMin)
        let tMax = Min3(xtMax, ytMax, ztMax)

        if tMin > tMax then
            List.empty

        else
            [tToIntersectionPoint tMin false
             tToIntersectionPoint (MathF.Max(tMin + Single.Epsilon, tMax)) true]            

    /// <summary>
    /// Represents a cube centered at the origin and extending from -1.0 to +1.0 along each axis.
    /// </summary>
    let geometry =
        Geometry ("Cube", Intersector intersector)

