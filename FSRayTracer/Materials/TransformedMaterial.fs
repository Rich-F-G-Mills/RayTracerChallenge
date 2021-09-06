
namespace FSRayTracer.Materials

[<AutoOpen>]
module TransformedMaterial =

    open System.Numerics
    open FSRayTracer.Geometries

    let transformMaterial transformer (MaterialMapper mapper) =
        let (Transformation transformation) =
            identityTransformation |> transformer

        let inverseTransformation =
            invertTransformationMatrix transformation

        let inner (location: Vector4) =
            let transformedLocation =
                Vector4.Transform(location, inverseTransformation)

            mapper transformedLocation

        MaterialMapper inner