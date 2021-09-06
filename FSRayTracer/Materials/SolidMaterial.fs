
namespace FSRayTracer.Materials

[<AutoOpen>]
module SolidMaterial =

    let solidMaterial material =
        MaterialMapper (fun _ -> material)