
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

    /// <summary>
    /// Represents a point of intersection in object space.
    /// </summary>
    type ObjectIntersectionPoint =
        { Distance: float32
          Location: Vector4
          Normal: Vector4
          /// <summary>
          /// Represents whether the ray that arose a point of intersection is either:
          /// | (Some false): Approaching from the outside.
          /// | (Some true): Approaching from the inside.
          /// | (None): Not applicable.
          /// </summary>
          Inside: bool option }

    /// <summary>
    /// Represents a mapping between a light ray and all intersections (if any) in object space.
    /// </summary>
    type Intersector =
        | Intersector of (Ray -> ObjectIntersectionPoint list)

    /// <summary>
    /// All geometries within the framework provide only a name and an <c>Intersector</c> function.
    /// </summary>
    type Geometry =
        | Geometry of Name: string * Intersector: Intersector

    /// <summary>
    /// Get all intersections between a ray of light and an untransformed geometry in object space.
    /// </summary>
    /// <returns>A list of <c>ObjectIntersectionPoint</c>s</returns>
    let getGeometryIntersections (Geometry (_, Intersector intersector)) ray =
        intersector ray

    /// <summary>
    /// Rename a geometry.
    /// </summary>
    /// <returns>A new geomtry with the specified name</returns>
    let rename name (Geometry (_, intersector)) =
        Geometry (name, intersector)
