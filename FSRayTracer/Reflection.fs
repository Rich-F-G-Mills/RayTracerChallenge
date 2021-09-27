
namespace FSRayTracer

module Reflection =

    open System.Numerics
    open FSRayTracer.Scene

    let inline private getOverPoint intersectionPoint =
        intersectionPoint.WorldLocation + intersectionPoint.WorldNormal * 0.001f

    let getReflectedColour colourizer scene ray intersectionPoint =
        if intersectionPoint.Material.Reflectivity = 0.0f then
            Colour.Black

        else
            let reflectedRay =
                { Origin = getOverPoint intersectionPoint
                  Direction = Vector4.Reflect(ray.Direction, intersectionPoint.WorldNormal) }

            let (reflectedColour: Colour) =
                reflectedRay
                |> getWorldIntersections scene
                |> colourizer scene ray
                    
            reflectedColour * intersectionPoint.Material.Reflectivity
            
