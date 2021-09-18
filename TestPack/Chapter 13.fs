namespace FSRayTracerUnitTests

open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries


type public ``Chapter 13: Cylinder`` (output: ITestOutputHelper) =

    [<Theory>]
    [<InlineData(1.1f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f)>]
    [<InlineData(-2.1f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f)>]
    [<InlineData(-1.1f, 1.1f, 0.0f, 1.0f, 0.0f, 0.0f)>]
    [<InlineData(-1.1f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f)>]
    let ``A ray misses a cylinder`` (rx, ry, rz, rdx, rdy, rdz) =
        let r =
            { Origin = Vector4(rx, ry, rz, 1.0f); Direction = Vector4(rdx, rdy, rdz, 0.0f) }        

        getGeometryIntersections (Cylinder.geometry) r
        |> Assert.Empty


    [<Theory>]
    [<InlineData(1.0f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f, 5.0f, 5.0f)>]
    [<InlineData(0.0f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f, 4.0f, 6.0f)>]
    [<InlineData(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f, 1.0f)>]
    [<InlineData(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, -1.0f, 1.0f)>]
    let ``A ray intersects a cylinder`` (rx, ry, rz, rdx, rdy, rdz, t1, t2) =
        let r =
            { Origin = Vector4(rx, ry, rz, 1.0f); Direction = Vector4(rdx, rdy, rdz, 0.0f) }        

        match getGeometryIntersections (Cylinder.geometry) r with
        | [i1; i2] ->
            Assert.Equal(i1.Distance, t1)
            Assert.Equal(i2.Distance, t2)

            Assert.False(i1.Inside)
            Assert.True(i2.Inside)

        | is ->
            raise (Xunit.Sdk.XunitException $"Expected 2 intersections; {is.Length} returned.")


    [<Theory>]
    [<InlineData(1.0f, 0.0f, 0.0f)>]
    [<InlineData(0.0f, 5.0f, -1.0f)>]
    [<InlineData(0.0f, -2.0f, 1.0f)>]
    [<InlineData(-1.0f, 1.0f, 0.0f)>]
    let ``Normal vector on a cylinder`` (rx, ry, rz) =
        Assert.Equal(
            Vector4(rx, 0.0f, rz, 0.0f),
            Cylinder.nonCapNormalAt (Vector4(rx, ry, rz, 1.0f))
        )