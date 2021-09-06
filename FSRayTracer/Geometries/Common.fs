
namespace FSRayTracer.Geometries


[<AutoOpen>]
module Common =

    open System
    open System.Numerics
    open FSRayTracer


    let inline internal Max3 (n1, n2, n3) =
        MathF.Max(MathF.Max(n1, n2), n3)

    let inline internal Min3 (n1, n2, n3) =
        MathF.Min(MathF.Min(n1, n2), n3)


    type ObjectIntersectionPoint =
        { Distance: float32
          Location: Vector4
          Normal: Vector4
          Inside: bool }

    type Intersector =
        | Intersector of (Ray -> ObjectIntersectionPoint list)

    type Geometry =
        | Geometry of Name: string * Intersector: Intersector

    let getGeometryIntersections (Geometry (_, Intersector intersector)) ray =
        intersector ray

    let rename name (Geometry (_, intersector)) =
        Geometry (name, intersector)
