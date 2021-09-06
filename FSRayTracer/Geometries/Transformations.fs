
namespace FSRayTracer.Geometries

[<AutoOpen>]
module public Transformations =
   
    open System.Numerics
    open System.Runtime.CompilerServices
    open FSRayTracer

    type Transformation = 
        | Transformation of Matrix4x4

    type TransformedGeometry =
        { Geometry: Geometry
          Transformation: Matrix4x4
          InverseTransformation: Matrix4x4
          TransposeInverseTransformation: Matrix4x4 }

    type TransformedIntersectionPoint =
        { Distance: float32
          Location: Vector4
          ObjectLocation: Vector4
          Normal: Vector4
          Inside: bool }


    let internal combineTransformationMatrices (t1: Matrix4x4, t2: Matrix4x4) =
        Matrix4x4.Multiply(t1, t2)

    let internal invertTransformationMatrix (transformation: Matrix4x4) =
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

    let createTransformedGeometry geometry transformer =
        let (Transformation transformation) =
            identityTransformation |> transformer

        let inverseTransformation = invertTransformationMatrix transformation

        { Geometry = geometry
          Transformation = transformation
          InverseTransformation = inverseTransformation
          TransposeInverseTransformation = Matrix4x4.Transpose(inverseTransformation) }


    let objectToTransformedIntersection transformedGeometry (objIntersection: ObjectIntersectionPoint) =
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


    let transformedToObjectIntersectionPoint transformedIntersection =
        { Distance = transformedIntersection.Distance
          Location = transformedIntersection.Location
          Normal = transformedIntersection.Normal
          Inside = transformedIntersection.Inside }


    let getTransformedGeometryIntersections (ray: Ray) (transformedGeometry: TransformedGeometry) =
        let _objectToTransformedIntersection =
            objectToTransformedIntersection transformedGeometry

        let transRayOrigin =
            Vector4.Transform(ray.Origin, transformedGeometry.InverseTransformation)

        let transRayDirection =
            // Don't normalize the direction as this would back-out any transformations made.
            Vector4.Transform(ray.Direction, transformedGeometry.InverseTransformation)

        let transRay =
            { Origin = transRayOrigin; Direction = transRayDirection }

        transRay
        |> getGeometryIntersections transformedGeometry.Geometry
        |> List.sortBy (fun i -> i.Distance)
        |> List.map _objectToTransformedIntersection
    