namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries


type public ``Chapter 12: Cubes`` (output: ITestOutputHelper) =
    
    [<Theory>]
    [<InlineData(5.0f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 4.0f, 6.0f)>]
    [<InlineData(-5.0f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 4.0f, 6.0f)>]
    [<InlineData(0.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 4.0f, 6.0f)>]
    [<InlineData(0.5f, -5.0f, 0.0f, 0.0f, 1.0f, 0.0f, 4.0f, 6.0f)>]
    [<InlineData(0.5f, 0.0f, 5.0f, 0.0f, 0.0f, -1.0f, 4.0f, 6.0f)>]
    [<InlineData(0.5f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f, 4.0f, 6.0f)>]
    [<InlineData(0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f, -1.0f, 1.0f)>]    // Inside cube
    let ``A ray intersects with a cube`` (rx, ry, rz, rdx, rdy, rdz, t1, t2) =
        let r =
            { Origin = Vector4(rx, ry, rz, 1.0f); Direction = Vector4(rdx, rdy, rdz, 0.0f) }        

        match getGeometryIntersections (Cube.geometry) r with
        | [i1; i2] ->
            Assert.Equal(i1.Distance, t1)
            Assert.Equal(i2.Distance, t2)

            Assert.False(i1.Inside)
            Assert.True(i2.Inside)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 2 intersections; {is.Length} returned.")


    [<Theory>]
    [<InlineData(-2.0f, 0.0f, 0.0f, 0.2673f, 0.5345f, 0.8018f)>]
    [<InlineData(0.0f, -2.0f, 0.0f, 0.8018f, 0.2673f, 0.5345f)>]
    [<InlineData(0.0f, 0.0f, -2.0f, 0.5345f, 0.8018f, 0.2673f)>]
    [<InlineData(2.0f, 0.0f, 2.0f, 0.0f, 0.0f, -1.0f)>]
    [<InlineData(0.0f, 2.0f, 2.0f, 0.0f, -1.0f, 0.0f)>]
    [<InlineData(2.0f, 2.0f, 0.0f, -1.0f, 0.0f, 0.0f)>]
    let ``A ray misses a cube`` (rx, ry, rz, rdx, rdy, rdz) =
        let r =
            { Origin = Vector4(rx, ry, rz, 1.0f); Direction = Vector4(rdx, rdy, rdz, 0.0f) }

        getGeometryIntersections (Cube.geometry) r
        |> Assert.Empty


    [<Theory>]
    [<InlineData(1.0f, 0.5f, -0.8f, 1.0f, 0.0f, 0.0f)>]
    [<InlineData(-1.0f, -0.2f, 0.9f, -1.0f, 0.0f, 0.0f)>]
    [<InlineData(-0.4f, 1.0f, -0.1f, 0.0f, 1.0f, 0.0f)>]
    [<InlineData(0.3f, -1.0f, -0.7f, 0.0f, -1.0f, 0.0f)>]
    [<InlineData(-0.6f, 0.3f, 1.0f, 0.0f, 0.0f, 1.0f)>]
    [<InlineData(0.4f, 0.4f, -1.0f, 0.0f, 0.0f, -1.0f)>]
    [<InlineData(1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f)>]
    [<InlineData(-1.0f, -1.0f, -1.0f, -1.0f, 0.0f, 0.0f)>]
    let ``The normal on the surface of a cube`` (rx, ry, rz, ndx, ndy, ndz) =
        Assert.Equal(
            Vector4(ndx, ndy, ndz, 0.0f),
            Cube.normalAt (Vector4(rx, ry, rz, 1.0f))
        )
