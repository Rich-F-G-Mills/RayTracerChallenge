
namespace FSRayTracer

[<AutoOpen>]
module Reflections =

    open System.Numerics
    open FSRayTracer.Scene

    let withReflections maxDepth matteColourizer scene ray intersectionPoint =
        let _getWorldIntersections = getWorldIntersections scene
        let _matteColourizer = matteColourizer scene

        let rec getReflections remaining accumulatedReflectivity accumulatedColour ray intersectionPoint =
            let (matteColour: Colour) = _matteColourizer ray intersectionPoint

            let (matteColourPostReflect: Colour) =
                matteColour * accumulatedReflectivity * (1.0f - intersectionPoint.Material.Reflectivity)

            let newAccumulatedReflection =
                accumulatedReflectivity * intersectionPoint.Material.Reflectivity

            let newAccumulatedColour =
                accumulatedColour + matteColourPostReflect

            if intersectionPoint.Material.Reflectivity < 0.001f then
                newAccumulatedColour

            else if remaining = 0 then
                newAccumulatedColour

            else
                let reflectedRay =
                    { Origin = intersectionPoint.WorldLocation + intersectionPoint.WorldNormal * 0.001f
                      Direction = Vector4.Reflect(ray.Direction, intersectionPoint.WorldNormal) }
                
                reflectedRay 
                |> _getWorldIntersections
                |> List.tryHead                
                |> function
                   | Some nextIntersectionPoint ->
                        getReflections (remaining - 1) newAccumulatedReflection newAccumulatedColour reflectedRay nextIntersectionPoint
                
                   | None ->
                        newAccumulatedColour
                    
        getReflections maxDepth 1.0f Colour.Black ray intersectionPoint
