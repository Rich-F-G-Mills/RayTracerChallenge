
namespace FSRayTracer.Materials

[<AutoOpen>]
module PolarBoxMaterial =

    let polarBoxMaterial mapper1 mapper2 (thetaStride, psiStride) =
        let inner (_, theta, psi) =
            let thetaDiv = int (theta / thetaStride)
            let psiDiv = int (psi / psiStride)

            if (thetaDiv + psiDiv) % 2 = 0 then mapper2 else mapper1

        inner
        |> PolarMaterialMapper
        |> polarMaterial
