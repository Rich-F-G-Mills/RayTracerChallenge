namespace FSRayTracerUnitTests


open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries


type public ``Chapter 14: Groups`` (output: ITestOutputHelper) =

    [<Fact>]
    let ``Intersecting a ray with an empty group`` () =
        let group =
            Group.createEmptyGroup ()
            |> Group.finalize

        let r =
            { Origin = Vector4.UnitW; Direction = Vector4.UnitZ }

        getGeometryIntersections group r
        |> Assert.Empty


    [<Fact>]
    let ``Intersecting a ray with a non-empty group`` () =
        let group =
            Group.createEmptyGroup ()
            |> Group.add Sphere.geometry applyNullTransformation
            |> Group.add Sphere.geometry (applyTranslation (0.0f, 0.0f, -3.0f))
            |> Group.add Sphere.geometry (applyTranslation (5.0f, 0.0f, 0.0f))
            |> Group.finalize

        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        let intersections =
            getGeometryIntersections group r
        
        Assert.Equal(4, intersections.Length)


    [<Fact>]
    let ``Intersecting a transformed group`` () =
        let group =
            Group.createEmptyGroup ()
            |> Group.add Sphere.geometry (applyTranslation (5.0f, 0.0f, 0.0f))
            |> Group.finalize

        let r =
            { Origin = Vector4(10.0f, 0.0f, -10.0f, 1.0f); Direction = Vector4.UnitZ }

        let intersections =
            createTransformedGeometry group (applyScaling (2.0f, 2.0f, 2.0f))
            |> getTransformedGeometryIntersections r
        
        Assert.Equal(2, intersections.Length)           

