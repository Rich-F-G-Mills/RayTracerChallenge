namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Materials


type public ``Chapter 05: Ray-Sphere Intersections`` (output: ITestOutputHelper) =

    let outputDistances is =
        is
        |> List.map (fun i -> i.Distance)
        |> List.map string
        |> String.concat ", "
        |> sprintf "Distances were %s."
        |> output.WriteLine
        

    [<Fact>]
    let ``Ray intersects sphere at 2 points`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; i2] ->
            Assert.True(i1.Distance = 4.0f)
            Assert.True(i2.Distance = 6.0f)

        | is ->
            outputDistances is

            raise (Xunit.Sdk.XunitException $"{is.Length} intersection(s) detected!")


    [<Fact>]
    let ``Ray intersects sphere at tangent`` () =
        let r =
            { Origin = Vector4(0.0f, 1.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1;_] ->
            Assert.True(i1.Distance = 5.0f)

        | is ->
            outputDistances is

            raise (Xunit.Sdk.XunitException $"{is.Length} intersection(s) detected!")


    [<Fact>]
    let ``Ray starts inside sphere`` () =
        let r =
            { Origin = Vector4.UnitW; Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; i2] ->
            Assert.True(i1.Distance = -1.0f)
            Assert.True(i2.Distance = 1.0f)

        | is ->
            outputDistances is

            raise (Xunit.Sdk.XunitException $"{is.Length} intersection(s) detected!")


    [<Fact>]
    let ``Ray starts outside sphere and looking outward`` () =
        let r =
            { Origin = Vector4(0.0f, 0.0f, 5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections (Sphere.geometry) r with
        | [i1; i2] ->
            Assert.True(i1.Distance = -6.0f)
            Assert.True(i2.Distance = -4.0f)

        | is ->
            outputDistances is

            raise (Xunit.Sdk.XunitException $"{is.Length} intersection(s) detected!")


    [<Fact>]
    let ``Intersecting a scaled sphere with ray`` () =
        let s =
            createTransformedGeometry (Sphere.geometry) (applyScaling (2.0f, 2.0f, 2.0f))

        let r =
            { Origin = Vector4(0.0f, 0.0f, -5.0f, 1.0f); Direction = Vector4.UnitZ }

        match getTransformedGeometryIntersections r s with
        | [i1; i2] ->
            Assert.True(i1.Distance = 3.0f)
            Assert.True(i2.Distance = 7.0f)

        | is ->
            raise (Xunit.Sdk.XunitException $"{is.Length} intersection(s) detected!")
