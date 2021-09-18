
namespace FSRayTracer.Materials

[<AutoOpen>]
module Common =

    open System
    open System.Numerics
    open FSRayTracer


    type Material =
        { Colour: Colour
          Ambient: float32
          Diffuse: float32
          Specular: float32
          Shininess: float32
          Reflectivity: float32
          Transparency: float32
          RefractiveIndex: float32 }

        static member Default =
            { Colour = Colour.White 
              Ambient = 0.1f
              Diffuse = 0.9f
              Specular = 0.9f
              Shininess = 200.0f
              Reflectivity = 0.0f }

    
    type MaterialMapper =
        | MaterialMapper of (Vector4 -> Material)

    
    let internal applyMaterialMapper (MaterialMapper mapper) location =
        mapper location


    let inline internal length (f1: float32, f2: float32) =
        MathF.Sqrt(f1 * f1 + f2 * f2)

    let inline internal atan2' (y: float32) (x: float32) =
        let result = atan2 y x

        if result >= 0.0f then result else (2.0f * MathF.PI + result)