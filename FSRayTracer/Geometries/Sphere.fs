
namespace FSRayTracer.Geometries

module Sphere =

    open System
    open System.Numerics
    open FSRayTracer

    let internal normalAt (touchPoint: Vector4) =        
        Vector4.Normalize(touchPoint - Vector4.UnitW)


    let private intersector (ray: Ray) =
        let tToIntersectionPoint (t: float32) inside =
            let touchPoint =
                ray.Origin + t * ray.Direction

            let normal = normalAt (touchPoint)

            { Distance = t
              Location = touchPoint
              Normal = if inside then -normal else normal
              Inside = Some inside }

        let sphereToRay = ray.Origin - Vector4.UnitW

        let a = ray.Direction.LengthSquared()
        let b = 2.0f * Vector4.Dot(ray.Direction, sphereToRay)
        let c = sphereToRay.LengthSquared() - 1.0f
        let discriminant = b * b - 4.0f * a * c
           
        // No intersection.
        if discriminant < 0.0f then
            List.empty

        else
            let sqrtDiscriminant = MathF.Sqrt(discriminant)

            let t1, t2 =
                (-b - sqrtDiscriminant) / (2.0f * a), (-b + sqrtDiscriminant) / (2.0f * a)

            [tToIntersectionPoint t1 false; tToIntersectionPoint (MathF.Max(t1 + Single.Epsilon, t2)) true]


    /// <summary>
    /// Create a sphere of unit radius centered at the origin.
    /// </summary>    
    let geometry =
        Geometry ("Sphere", Intersector intersector)
