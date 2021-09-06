
namespace FSRayTracer.Materials

[<AutoOpen>]
module CartesianRingMaterial =

    open System.Numerics

    let cartesianRingMaterial material1 material2 rStride =
        let inner (location: Vector4) =
            let rDiv =
                int ((length (location.X, location.Z) + rStride / 2.0f) / rStride)

            let mapper =
                if rDiv % 2 = 0 then material2 else material1

            applyMaterialMapper mapper location

        MaterialMapper inner
