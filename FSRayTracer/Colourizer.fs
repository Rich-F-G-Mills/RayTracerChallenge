
namespace FSRayTracer

module Colourizer =

    open FSRayTracer.Scene

    let rec createDefault remaining lightingColourizer (scene: Scene) (ray: Ray) (intersectionPoints: SceneIntersectionPoint list) =
        if remaining <= 0 || intersectionPoints.IsEmpty then
            Colour.Black

        else
            let createDefault' =
                createDefault (remaining - 1) lightingColourizer                

            let colourFromLighting, reflectedColour =
                intersectionPoints
                |> List.tryFind hasPositiveDistance
                |> function
                    | Some nextIntersectionPoint ->
                        lightingColourizer scene ray nextIntersectionPoint,
                        Reflection.getReflectedColour createDefault' scene ray nextIntersectionPoint

                    | _ ->
                        Colour.Black, Colour.Black

            let refractedColour =
                Refraction.getRefractedColour createDefault' scene ray intersectionPoints
                

            colourFromLighting + reflectedColour
        
        

