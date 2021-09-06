namespace FSRayTracerUnitTests

open System
open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Scene
open FSRayTracer.Materials


type public ``Chapter 07: Making a Scene`` (output: ITestOutputHelper) =

    [<Fact>]
    let ``Intersect a world with a ray`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getWorldIntersections defaultWorldScene r with
        | [i1; i2; i3; i4] ->
            Assert.Equal(4.0f, i1.Distance)
            Assert.Equal(4.5f, i2.Distance)
            Assert.Equal(5.5f, i3.Distance)
            Assert.Equal(6.0f, i4.Distance)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 4 intersections; {is.Length} returned.")   
            

    [<Fact>]
    let ``Precomputing the state of an intersection`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; _] ->
            Assert.Equal(Vector4(0.0f, 0.0f, -1.0f, 1.0f), i1.Location)
            Assert.Equal(Vector4(0.0f, 0.0f, -1.0f, 0.0f), i1.Normal)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 4 intersections; {is.Length} returned.")    



    [<Fact>]
    let ``Ray approaches from outside of sphere along radius`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; i2] ->
            Assert.Equal(4.0f, i1.Distance)
            Assert.Equal(6.0f, i2.Distance)

            Assert.False(i1.Inside)
            Assert.True(i2.Inside)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 2 intersections; {is.Length} returned.")


    [<Fact>]
    let ``Ray starts at center of sphere`` () =      
        let r =
            { Origin = Vector4.UnitW; Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; i2] ->
            Assert.Equal(-1.0f, i1.Distance)
            Assert.Equal(1.0f, i2.Distance)

            Assert.False(i1.Inside)
            Assert.True(i2.Inside)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 2 intersections; {is.Length} returned.")


    [<Fact>]
    let ``Shading an intersection from the outside`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getWorldIntersections defaultWorldScene r with
        | [i1; _; _; _] ->          
            let colour =
                usePhongLighting [defaultLight] defaultWorldScene r i1

            Assert.Equal(4.0f, i1.Distance)
            Assert.True(ColourDistance({ R = 0.38066f; G = 0.47583f; B = 0.2855f }, colour) < 0.0001f)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 4 intersections; {is.Length} returned.")
        

    [<Fact>]
    let ``Shading an intersection from the inside`` () =
        let light =
            { Position = Vector3(-0.0f, 0.25f, 0.0f); Intensity = Colour.White }

        let r =
            { Origin = Vector4.UnitW; Direction = Vector4.UnitZ }

        match getWorldIntersections defaultWorldScene r with
        | [i1; _] ->          
            let colour =
                usePhongLighting [light] defaultWorldScene r i1

            Assert.Equal(0.5f, i1.Distance)
            Assert.True(ColourDistance({ R = 0.90498f; G = 0.90498f; B = 0.90498f }, colour) < 0.0001f)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 2 intersections; {is.Length} returned.")


    [<Fact>]
    let ``Pixel size for a horizontal canvas`` () =
        let camera =
            { Width = 200.0f; Height = 125.0f; FieldOfView = MathF.PI / 2.0f }

        let tc =
            getTransformedCamera camera Matrix4x4.Identity

        Assert.Equal(0.01f, tc.PixelSize)


    [<Fact>]
    let ``Pixel size for a vertical canvas`` () =
        let camera =
            { Width = 125.0f; Height = 200.0f; FieldOfView = MathF.PI / 2.0f }

        let tc =
            getTransformedCamera camera Matrix4x4.Identity

        Assert.Equal(0.01f, tc.PixelSize)


    [<Fact>]
    let ``Constructing ray through center of canvas`` () =
        let camera =
            { Width = 201.0f; Height = 101.0f; FieldOfView = MathF.PI / 2.0f }

        let r =
            getTransformedCamera camera Matrix4x4.Identity
            |> getRayForPixel <| (100.0f, 50.0f)

        Assert.True(Vector4.Distance(Vector4.UnitW, r.Origin) <= 0.00001f)
        Assert.True(Vector4.Distance(-Vector4.UnitZ, r.Direction) <= 0.00001f)


    [<Fact>]
    let ``Constructing ray through a corner of the canvas`` () =
        let camera =
            { Width = 201.0f; Height = 101.0f; FieldOfView = MathF.PI / 2.0f }

        let r =
            getTransformedCamera camera Matrix4x4.Identity
            |> getRayForPixel <| (0.0f, 0.0f)

        Assert.True(Vector4.Distance(Vector4.UnitW, r.Origin) <= 0.00001f)
        Assert.True(Vector4.Distance(Vector4(0.66519f, 0.33259f, -0.66851f, 0.0f), r.Direction) <= 0.00001f)


    [<Fact>]
    let ``Constructing ray when the camera is transformed`` () =
        let camera =
            { Width = 201.0f; Height = 101.0f; FieldOfView = MathF.PI / 2.0f }

        let trans =
            Matrix4x4.Multiply(                
                Matrix4x4.CreateTranslation(0.0f, -2.0f, 5.0f),
                Matrix4x4.CreateRotationY(MathF.PI / 4.0f)
            )

        let r =
            getTransformedCamera camera trans
            |> getRayForPixel <| (100.0f, 50.0f)

        Assert.True(
            Vector4.Distance(Vector4(0.0f, 2.0f, -5.0f, 1.0f), r.Origin) <= 0.00001f
        )

        Assert.True(
            Vector4.Distance(
                Vector4(MathF.Sqrt(2.0f) / 2.0f, 0.0f, -MathF.Sqrt(2.0f) / 2.0f, 0.0f),
                r.Direction
            ) <= 0.00001f
        )


    [<Fact>]
    let ``Rendering a world with a camera`` () =
        let camera =
            { Width = 11.0f; Height = 11.0f; FieldOfView = MathF.PI / 2.0f }

        let view =
            { From = Vector3(0.0f, 0.0f, -5.0f)
              To = Vector3.Zero
              Up = Vector3.UnitY }

        let colourizer =
            usePhongLighting [defaultLight]

        Scene.render camera view defaultWorldScene colourizer
        |> Seq.map (fun (x, y, c) -> (int x, int y, c))
        |> Seq.tryFind (fun (x, y, _) -> (x = 5) && (y = 5))
        |> function
            | Some (_, _, ValueSome c) ->
                Assert.True(ColourDistance(c, { R = 0.38066f; G = 0.47583f; B = 0.2855f }) < 0.0001f)
            | _ ->
                raise (Xunit.Sdk.XunitException "Unable to get colour of specified pixel.")
        