
namespace FSRayTracer.Materials

[<AutoOpen>]
module PolarMaterial =

    open System
    open System.Numerics

    type PolarMaterialMapper =
    | PolarMaterialMapper of (float32 * float32 * float32 -> MaterialMapper)


    let polarMaterial (PolarMaterialMapper polarMapper) =
        let inner (location: Vector4) =
            let r = location.Length()

            let theta = atan2' location.Y location.X

            let psi = atan2' (length(location.X, location.Y)) location.Z

            applyMaterialMapper (polarMapper (r, theta, psi)) location

        MaterialMapper inner
                