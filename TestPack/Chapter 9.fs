namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries


type public ``Chapter 09: Planes`` (output: ITestOutputHelper) =
    
    [<Theory>]
    [<InlineData(0.0f, 0.0f)>]
    [<InlineData(10.0f, -10.0f)>]
    [<InlineData(-5.0f, 150.0f)>]
    let ``The normal of a plane is constant everywhere`` (x, z) =
        let r =
            { Origin = Vector4(x, 0.1f, z, 1.0f); Direction = -Vector4.UnitY }

        match getGeometryIntersections (Plane.geometry) r with
        | [{ Normal = normal }] ->
            Assert.Equal(Vector4.UnitY, normal)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 1 intersections; {is.Length} returned.")  
            

    [<Fact>]
    let ``Intersect with a ray parallel to the plane`` () =
        let r =
            { Origin = Vector4(0.0f, 10.0f, 0.0f, 1.0f); Direction = -Vector4.UnitZ }

        getGeometryIntersections (Plane.geometry) r
        |> Assert.Empty


    [<Fact>]
    let ``Intersect with a coplanar ray`` () =
        let r =
            { Origin = Vector4.UnitW; Direction = Vector4.UnitZ }

        getGeometryIntersections (Plane.geometry) r
        |> Assert.Empty


    [<Fact>]
    let ``Ray intersecting a plane from above`` () =
        let r =
            { Origin = Vector4(0.0f, 1.0f, 0.0f, 1.0f); Direction = -Vector4.UnitY }

        match getGeometryIntersections (Plane.geometry) r with
        | [i1] ->
            Assert.Equal(1.0f, i1.Distance)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 1 intersections; {is.Length} returned.")


    [<Fact>]
    let ``Ray intersecting a plane from below`` () =
        let r =
            { Origin = Vector4(0.0f, -1.0f, 0.0f, 1.0f); Direction = Vector4.UnitY }

        match getGeometryIntersections (Plane.geometry) r with
        | [i1] ->
            Assert.Equal(1.0f, i1.Distance)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 1 intersections; {is.Length} returned.")
