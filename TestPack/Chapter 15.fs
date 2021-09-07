namespace FSRayTracerUnitTests

open System.Collections.Generic
open System.Numerics
open Xunit
open Xunit.Abstractions
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Geometries.TriangleMesh


type public ``Chapter 15: Triangles`` (output: ITestOutputHelper) =

    static let p1 = Vector3.UnitY
    static let p2 = -Vector3.UnitX
    static let p3 = Vector3.UnitX

    static let defaultTriangleGeom =
        createEmptyMesh ()
        |> beginFaceWithoutNormals
            |> addVertexWithoutNormal p1
            |> addVertexWithoutNormal p2
            |> addVertexWithoutNormal p3
        |> endFaceWithoutNormals
        |> finalize false false


    [<Fact>]
    let ``Constructing a triangle`` () =
        let t = Triangle.create false (p1, p2, p3)

        Assert.Equal(t.e1, Vector3(-1.0f, -1.0f, 0.0f))
        Assert.Equal(t.e2, Vector3(1.0f, -1.0f, 0.0f))
        Assert.Equal(t.p1Normal, -Vector4.UnitZ)


    [<Fact>]
    let ``Intersecting a ray parallel to the triangle`` () =
        let r =
            { Origin = Vector4(0.0f, -1.0f, -2.0f, 1.0f); Direction = Vector4.UnitY }

        getGeometryIntersections defaultTriangleGeom r
        |> Assert.Empty


    [<Fact>]
    let ``A ray misses the p1-p3 edge`` () =
        let r =
            { Origin = Vector4(1.0f, 1.0f, -2.0f, 1.0f); Direction = Vector4.UnitZ }

        getGeometryIntersections defaultTriangleGeom r
        |> Assert.Empty


    [<Fact>]
    let ``A ray misses the p1-p2 edge`` () =
        let r =
            { Origin = Vector4(-1.0f, 1.0f, -2.0f, 1.0f); Direction = Vector4.UnitZ }

        getGeometryIntersections defaultTriangleGeom r
        |> Assert.Empty


    [<Fact>]
    let ``A ray misses the p2-p3 edge`` () =
        let r =
            { Origin = Vector4(0.0f, -1.0f, -2.0f, 1.0f); Direction = Vector4.UnitZ }

        getGeometryIntersections defaultTriangleGeom r
        |> Assert.Empty


    [<Fact>]
    let ``A ray strikes the triangle`` () =
        let r =
            { Origin = Vector4(0.0f, 0.5f, -2.0f, 1.0f); Direction = Vector4.UnitZ }

        match getGeometryIntersections defaultTriangleGeom r with
        | [{Distance = t}] ->
            Assert.Equal(2.0f, t)

        | _ ->
            raise (Xunit.Sdk.XunitException "Expected a single intersection.")
        
        
    [<Fact>]
    let ``Can parse single OBJ face from file`` () =
        match OBJFile.loadToMesh "OBJ TEST.OBJ" with
        | Mesh [FaceWithoutNormals face] ->
            let expected =
                [VertexWithoutNormal (Vector3(-1.0f, 1.0f, 0.0f))
                 VertexWithoutNormal (Vector3(-1.0f, 0.0f, 0.0f))
                 VertexWithoutNormal (Vector3(1.0f, 0.0f, 0.0f))
                 VertexWithoutNormal (Vector3(1.0f, 1.0f, 0.0f))
                 VertexWithoutNormal (Vector3(0.0f, 2.0f, 0.0f))]
                
            Assert.StrictEqual(expected, face)

        | _ ->
            raise (Xunit.Sdk.XunitException "Expected a single face with 5 vertices.")
