namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Materials


type public ``Chapter 08: Shadows`` (output: ITestOutputHelper) =
    
    [<Fact>]
    let ``Lighting with the surface in shadow`` () =
        let light =
            { Position = Vector3(0.0f, 0.0f, -10.0f); Intensity = Colour.White }

        let colour =
            phongColourCalculation true Material.Default Vector4.UnitW Vector4.UnitZ -Vector4.UnitZ light

        Assert.Equal({ R = 0.1f; G = 0.1f; B = 0.1f }, colour)


    [<Fact>]
    let ``No shadow when nothing collinear with point and light`` () =
        checkInShadow
            defaultWorldScene
            (Vector4(0.0f, 10.0f, 0.0f, 1.0f))
            (Vector4(defaultLight.Position, 1.0f))
        |> Assert.False


    [<Fact>]
    let ``Shadow when and object is between point and light`` () =
        checkInShadow
            defaultWorldScene
            (Vector4(10.0f, -10.0f, 10.0f, 1.0f))
            (Vector4(defaultLight.Position, 1.0f))
        |> Assert.True


    [<Fact>]
    let ``No shadow when an object is behind the light`` () =
        checkInShadow
            defaultWorldScene
            (Vector4(-20.0f, 20.0f, -20.0f, 1.0f))
            (Vector4(defaultLight.Position, 1.0f))
        |> Assert.False


    [<Fact>]
    let ``No shadow when an object is behind the point`` () =
        checkInShadow
            defaultWorldScene
            (Vector4(-2.0f, 2.0f, -2.0f, 1.0f))
            (Vector4(defaultLight.Position, 1.0f))
        |> Assert.False


    [<Fact>]
    let ``Colour when an intersection is in shadow`` () =
        let light =
            { Position = Vector3(0.0f, 0.0f, -10.0f); Intensity = Colour.White }

        let scene =
            Scene.createEmptyScene()
            |> Scene.add
                    Sphere.geometry
                    applyNullTransformation
                    (solidMaterial Material.Default)
            |> Scene.add
                    Sphere.geometry
                    (applyTranslation (0.0f, 0.0f, 10.0f))
                    (solidMaterial Material.Default)

        let r =
            { Origin = Vector4(0.0f, 0.0f, 5.0f, 1.0f); Direction = Vector4.UnitZ }

        match Scene.getWorldIntersections scene r with
        | i1::_ ->
            let colour =
                usePhongLighting [light] scene r i1

            Assert.Equal({ R = 0.1f; G = 0.1f; B = 0.1f }, colour)

        | [] ->
            raise (Xunit.Sdk.XunitException "No intersections found!")
