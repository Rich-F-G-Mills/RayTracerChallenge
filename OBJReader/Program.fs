
open FSRayTracer
open FSRayTracer.Geometries.TriangleMesh


[<EntryPoint>]
let main argv =
    //let mesh =
    //    FSRayTracer.Geometries.TriangleMesh.OBJFile.loadToMesh "teapot.obj"

    let (Mesh faces2) =
        FSRayTracer.Geometries.TriangleMesh.OBJFile.loadToMesh "teapot2.obj"

    printfn "Number of faces = %i" faces2.Length

    0