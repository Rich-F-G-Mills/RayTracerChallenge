
namespace FSRayTracer.Materials

[<AutoOpen>]
module CartesianBoxMaterial =

    open System
    open System.Numerics

    let cartesianBoxMaterial mapper1 mapper2 (xStride, yStride, zStride) =
        let inner (location: Vector4) =
            let xDiv = int (MathF.Floor(location.X / xStride + 0.5f))
            let yDiv = int (MathF.Floor(location.Y / yStride + 0.5f))
            let zDiv = int (MathF.Floor(location.Z / zStride + 0.5f))

            let mapper =
                if (xDiv + yDiv + zDiv) % 2 = 0 then mapper2 else mapper1

            applyMaterialMapper mapper location

        MaterialMapper inner