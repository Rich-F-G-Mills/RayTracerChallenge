
namespace FSRayTracer

module Lighting =

    open System
    open System.Numerics
    open FSRayTracer.Materials
    open FSRayTracer.Scene


    type LightSource =
        { Intensity: Colour
          Position: Vector3 }


    let checkInShadow scene targetPoint lightLocation =
        let overPointToLight =
            // Don't normalise as we are comparing distance relative to 1.
            { Origin = targetPoint; Direction = lightLocation - targetPoint }

        getWorldIntersections scene overPointToLight
        |> List.tryFind hasPositiveDistance
        |> function
            | Some { Distance = d } when d < 1.0f -> true
            | _ -> false


    let internal phongColourCalculation inShadow material targetPoint rayDirection normalVector light =
        let lightLocation = Vector4(light.Position, 1.0f)

        let objectToLightVectorNrmld =
            Vector4.Normalize (lightLocation - targetPoint)

        let effectiveColour = material.Colour * light.Intensity
        
        let ambient = effectiveColour * material.Ambient
        
        let lightDotNormal = Vector4.Dot(objectToLightVectorNrmld, normalVector)
        
        let diffuse =
            if inShadow then Colour.Black
            else if lightDotNormal < 0.0f then Colour.Black
            else effectiveColour * material.Diffuse * lightDotNormal
        
        let specular =
            if inShadow then Colour.Black
            else if lightDotNormal < 0.0f then Colour.Black
            else
                let reflectVector = Vector4.Reflect(-objectToLightVectorNrmld, normalVector)
                let reflectDotEye = Vector4.Dot(reflectVector, -rayDirection)
        
                if reflectDotEye <= 0.0f then
                    Colour.Black
                else
                    let factor = MathF.Pow(reflectDotEye, material.Shininess)
        
                    light.Intensity * material.Specular * factor
        
        ambient + diffuse + specular        


    let internal castFromLight scene material targetPoint rayDirection normalVector light =
        let lightLocation = Vector4(light.Position, 1.0f)        

        let inShadow =
            checkInShadow scene (targetPoint + normalVector * 0.0001f) lightLocation

        phongColourCalculation inShadow material targetPoint rayDirection normalVector light
        
    
    let colourFromLighting lightSources scene ray intersectionPoint =
        let material =
            intersectionPoint.Material

        let _castFromLight =
            castFromLight scene material intersectionPoint.WorldLocation ray.Direction intersectionPoint.WorldNormal

        lightSources
        |> List.map _castFromLight
        |> List.reduce (+)
