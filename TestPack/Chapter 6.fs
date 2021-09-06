namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Materials


type public ``Chapter 06: Light and Shading`` (output: ITestOutputHelper) =

    [<Fact>]
    let ``Sphere normal at point on X axis`` () =
        Assert.Equal(Vector4.UnitX, Sphere.normalAt(Vector4(1.0f, 0.0f, 0.0f, 1.0f)))


    [<Fact>]
    let ``Sphere normal at point on Y axis`` () =
        Assert.Equal(Vector4.UnitY, Sphere.normalAt(Vector4(0.0f, 1.0f, 0.0f, 1.0f)))


    [<Fact>]
    let ``Sphere normal at point on Z axis`` () =
        Assert.Equal(Vector4.UnitZ, Sphere.normalAt(Vector4(0.0f, 0.0f, 1.0f, 1.0f)))


    [<Fact>]
    let ``Sphere normal at non-axial point`` () =
        let point = Vector3.Normalize(Vector3(3.0f, 3.0f, 3.0f))

        let normal = Sphere.normalAt(Vector4(point, 1.0f))
        let expected = Vector4(point, 0.0f)

        Assert.True (Vector4.Distance(normal, expected) < 0.0001f)


    [<Fact>]
    let ``Reflect vector approaching at 45 degrees`` () =
        let inVector = Vector4(1.0f, -1.0f, 0.0f, 0.0f)
        let normal = Vector4(0.0f, 1.0f, 0.0f, 0.0f)

        let outVector = Vector4.Reflect(inVector, normal)

        Assert.Equal(Vector4(1.0f, 1.0f, 0.0f, 0.0f), outVector)


    [<Fact>]
    let ``Reflect vector off a slanted surface`` () =
        let inVector = Vector4(0.0f, -1.0f, 0.0f, 0.0f)
        let normal =
            Vector4(1.0f, 1.0f, 0.0f, 0.0f)
            |> Vector4.Normalize

        let outVector = Vector4.Reflect(inVector, normal)

        let expectedOutVector = Vector4(1.0f, 0.0f, 0.0f, 0.0f)

        Assert.True (Vector4.Distance(outVector, expectedOutVector) < 0.0001f)


    let targetPoint = Vector4.UnitW
    let surfaceNormal = -Vector4.UnitZ


    [<Fact>]
    let ``Eye directly between light and surface`` () =      
        let lightSource =
            { Intensity = Colour.White; Position = Vector3(0.0f, 0.0f, -10.0f) }

        let rayDirection = Vector4.UnitZ        

        let colour = 
            phongColourCalculation false Material.Default targetPoint rayDirection surfaceNormal lightSource

        Assert.Equal({ R = 1.9f; G = 1.9f; B = 1.9f}, colour)


    [<Fact>]
    let ``Lighting with eye between light and surface, eye offset 45 degrees`` () =
        let lightSource =
            { Intensity = Colour.White; Position = Vector3(0.0f, 0.0f, -10.0f) }

        let rayDirection =
            Vector4(0.0f, -1.0f, 1.0f, 0.0f)
            |> Vector4.Normalize

        let colour = 
            phongColourCalculation false Material.Default targetPoint rayDirection surfaceNormal lightSource

        Assert.Equal(Colour.White, colour)


    [<Fact>]
    let ``Lighting with eye opposite surface, light offset 45 degrees`` () =
        let lightSource =
            { Intensity = Colour.White; Position = Vector3(0.0f, 10.0f, -10.0f) }

        let rayDirection = Vector4.UnitZ

        let colour = 
            phongColourCalculation false Material.Default targetPoint rayDirection surfaceNormal lightSource

        Assert.True(ColourDistance({ R = 0.7364f; G = 0.7364f; B = 0.7364f }, colour) < 0.0001f)

        
    [<Fact>]
    let ``Lighting with eye in path of reflection vector`` () =
        let lightSource =
            { Intensity = Colour.White; Position = Vector3(0.0f, 10.0f, -10.0f) }

        let rayDirection =
            Vector4(0.0f, 1.0f, 1.0f, 0.0f)
            |> Vector4.Normalize

        let colour = 
            phongColourCalculation false Material.Default targetPoint rayDirection surfaceNormal lightSource

        Assert.True(ColourDistance({ R = 1.6364f; G = 1.6364f; B = 1.6364f }, colour) < 0.0001f)
