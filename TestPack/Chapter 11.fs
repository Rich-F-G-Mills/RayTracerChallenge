namespace FSRayTracerUnitTests

open System
open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Materials


type public ``Chapter 11: Reflection and Refraction`` (output: ITestOutputHelper) =

    [<Fact>]
    let ``Pre-compute the reflection vector`` () =
        let twoRootTwo = MathF.Sqrt(2.0f) / 2.0f

        let r =
            { Origin = Vector4(0.0f, 1.0f, -1.0f, 1.0f)
              Direction = Vector4(0.0f, -twoRootTwo, twoRootTwo, 0.0f) }        


        match getGeometryIntersections Plane.geometry r with
        | [i] ->
            let reflected =
                Vector4.Reflect(r.Direction, i.Normal)

            Assert.Equal(MathF.Sqrt(2.0f), i.Distance)
            Assert.True(            
                Vector4.Distance(
                    Vector4(0.0f, twoRootTwo, twoRootTwo, 0.0f),
                    reflected
                ) < 0.0001f
            )

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected single intersection; {is.Length} returned.")


    [<Fact>]
    let ``Strike a reflective surface`` () =
        let twoRootTwo = MathF.Sqrt(2.0f)

        let r =
            { Origin = Vector4(0.0f, 0.0f, -3.0f, 1.0f)
              Direction = Vector4(0.0f, -twoRootTwo, twoRootTwo, 0.0f) }

        let scene =
            defaultWorldScene
            |> Scene.add
                Plane.geometry
                (applyTranslation (0.0f, -1.0f, 0.0f))
                (solidMaterial ({ Material.Default with Reflectivity = 0.5f }))

        
                

            