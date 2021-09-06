namespace FSRayTracerUnitTests

[<AutoOpen>]
module Common =

    open System.Numerics
    open FSRayTracer
    open FSRayTracer.Geometries
    open FSRayTracer.Materials


    let defaultWorldScene =
        let m =
            { Material.Default with
                Colour = { R = 0.8f; G = 1.0f; B = 0.6f }
                Diffuse = 0.7f
                Specular = 0.2f }

        Scene.createEmptyScene ()
        |> Scene.add
                Sphere.geometry
                applyNullTransformation
                (solidMaterial m)
        |> Scene.add
                Sphere.geometry
                (applyScaling (0.5f, 0.5f, 0.5f))
                (solidMaterial Material.Default)


    let defaultLight =
        { Position = Vector3(-10.0f, 10.0f, -10.0f); Intensity = Colour.White }


    let ColourDistance(lhs: Colour, rhs: Colour) =
        let _lhs = Vector3(lhs.R, lhs.G, lhs.B)
        let _rhs = Vector3(rhs.R, rhs.G, rhs.B)

        Vector3.Distance(_lhs, _rhs)
