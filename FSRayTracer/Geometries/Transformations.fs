
namespace FSRayTracer.Geometries

[<AutoOpen>]
module public Transformations =
   
    open System.Numerics
    open System.Runtime.CompilerServices
    open FSRayTracer

    /// <summary>
    /// Wrapper for a transformation matrix.
    /// </summary>
    type Transformation = 
        | Transformation of Matrix4x4

    /// <summary>
    /// Combines a <c>Geometry</c> with a <c>Transformation</c>.
    /// </summary>
    type TransformedGeometry =
        { Geometry: Geometry
          Transformation: Matrix4x4
          InverseTransformation: Matrix4x4
          TransposeInverseTransformation: Matrix4x4 }

    /// <summary>
    /// Represents a point of intersection in world space.
    /// </summary>
    type TransformedIntersectionPoint =
        { Distance: float32
          Location: Vector4
          ObjectLocation: Vector4
          Normal: Vector4
          Inside: bool option }

    let internal combineTransformationMatrices (t1: Matrix4x4, t2: Matrix4x4) =
        Matrix4x4.Multiply(t1, t2)

    let internal invertTransformationMatrix transformation =
        match Matrix4x4.Invert(transformation) with
        | (true, inv) -> inv
        | _ -> failwith "Unable to invert transformation."

    let internal invertTransformation (Transformation transformation) =
        invertTransformationMatrix transformation

    let internal identityTransformation =
        Transformation Matrix4x4.Identity

    let internal combineTransformations (Transformation newTransformation) (Transformation transformation) =
        combineTransformationMatrices (transformation, newTransformation)
        |> Transformation

    let internal createTranslation (dx, dy, dz) =
        Matrix4x4.CreateTranslation(dx, dy, dz)
        |> Transformation

    let internal createScaling (dx, dy, dz) =
        Matrix4x4.CreateScale(dx, dy, dz)
        |> Transformation

    let internal createRotationX (r, origin) =
        Matrix4x4.CreateRotationX(r, origin)
        |> Transformation

    let internal createRotationY (r, origin) =
        Matrix4x4.CreateRotationY(r, origin)
        |> Transformation

    let internal createRotationZ (r, origin) =
        Matrix4x4.CreateRotationZ(r, origin)
        |> Transformation

    let internal createShearing (xInPropY, xInPropZ, yInPropX, yInPropZ, zInPropX, zInPropY) =
        Matrix4x4(
            1.0f, yInPropX, zInPropX, 0.0f,
            xInPropY, 1.0f, zInPropY, 0.0f,
            xInPropZ, yInPropZ, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f
        )|> Transformation

    let applyNullTransformation =
        combineTransformations identityTransformation        

    let applyTranslation =
        createTranslation >> combineTransformations

    let applyScaling =
        createScaling >> combineTransformations

    let applyRotationX =
        createRotationX >> combineTransformations

    let applyRotationY =
        createRotationY >> combineTransformations

    let applyRotationZ =
        createRotationZ >> combineTransformations

    let applyShearing =
        createShearing >> combineTransformations

    /// <summary>
    /// Combines an untransformed <c>Geometry</c> with a <c>Transformation</c>
    /// </summary>
    let createTransformedGeometry geometry transformer =
        let (Transformation transformation) =
            identityTransformation |> transformer

        let inverseTransformation = invertTransformationMatrix transformation

        { Geometry = geometry
          Transformation = transformation
          InverseTransformation = inverseTransformation
          TransposeInverseTransformation = Matrix4x4.Transpose(inverseTransformation) }

    
    /// <summary>
    /// Converts an intersection point in (untransformed) object space to that in world space.
    /// </summary>
    let objectIntersectionToTransformedIntersection transformedGeometry (objIntersection: ObjectIntersectionPoint) =
        let transformedLocation =
            Vector4.Transform(
                objIntersection.Location, 
                transformedGeometry.Transformation
            )

        let transformedNormal =
            Vector4.Transform(
                objIntersection.Normal,
                transformedGeometry.TransposeInverseTransformation
            ).SetW(0.0f)
            |> Vector4.Normalize

        { Distance = objIntersection.Distance
          Location = transformedLocation
          ObjectLocation = objIntersection.Location
          Normal = transformedNormal
          Inside = objIntersection.Inside }

    /// <summary>
    /// Treate an intersection point in world space as if it was in object space.
    /// This is useful for nested geometries where the top-level scene object is ultimately expecting all intersections to be in object space.
    /// </summary>
    let transformedIntersectionToObjectIntersection transformedIntersection =
        { Distance = transformedIntersection.Distance
          Location = transformedIntersection.Location
          Normal = transformedIntersection.Normal
          Inside = transformedIntersection.Inside }

    /// <summary>
    /// Determines intersections (in world space) between a transformed geometry and light ray.
    /// </summary>
    let getTransformedGeometryIntersections (ray: Ray) (transformedGeometry: TransformedGeometry) =
        let _objectIntersectionToTransformedIntersection =
            objectIntersectionToTransformedIntersection transformedGeometry

        let transRayOrigin =
            Vector4.Transform(ray.Origin, transformedGeometry.InverseTransformation)

        let transRayDirection =
            // Do NOT normalize the direction as this would back-out any transformations made.
            Vector4.Transform(ray.Direction, transformedGeometry.InverseTransformation)

        let transRay =
            { Origin = transRayOrigin; Direction = transRayDirection }

        transRay
        |> getGeometryIntersections transformedGeometry.Geometry
        |> List.sortBy (fun i -> i.Distance)
        |> List.map _objectIntersectionToTransformedIntersection
    