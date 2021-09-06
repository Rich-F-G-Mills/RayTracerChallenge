
namespace FSRayTracer.Geometries

module Plane =

    open System
    open System.Numerics
    open FSRayTracer

    let private intersector (ray: Ray) =
        let tToIntersectionPoint (t: float32) =            
            { Distance = t
              Location = ray.Origin + t * ray.Direction
              Normal = Vector4.UnitY
              Inside = false }

        if MathF.Abs(ray.Direction.Y) <= Single.Epsilon then
            List.Empty

        else
            let t = -ray.Origin.Y / ray.Direction.Y

            [tToIntersectionPoint t]


    let geometry =
        Geometry ("Plane", Intersector intersector)
