namespace FSRayTracer.Geometries

module TriangleMesh =

    open System
    open System.Text.RegularExpressions
    open System.Numerics
    open FSharpx
    open FSharpx.IO
    open FSharpx.Text.Regex
    open FSRayTracer

    type VertexWithNormal =
        | VertexWithNormal of Position: Vector3 * Normal: Vector3

    type VertexWithoutNormal =
        | VertexWithoutNormal of Position: Vector3

    type Face = 
        | FaceWithNormals of Vertices: VertexWithNormal list
        | FaceWithoutNormals of Vertices: VertexWithoutNormal list

    type Mesh =
        | Mesh of Faces: Face list

    type FaceWithNormalsInProgress =
        { Vertices: VertexWithNormal list
          Mesh: Mesh }

    type FaceWithoutNormalsInProgress =
        { Vertices: VertexWithoutNormal list
          Mesh: Mesh }

    type Triangle =
        { p1: Vector3
          p2: Vector3
          p3: Vector3
          e1: Vector3
          e2: Vector3
          p1Normal: Vector4
          p2Normal: Vector4
          p3Normal: Vector4 }

        member this.minX = Min3(this.p1.X, this.p2.X, this.p3.X)
        member this.minY = Min3(this.p1.Y, this.p2.Y, this.p3.Y)
        member this.minZ = Min3(this.p1.Z, this.p2.Z, this.p3.Z)
        member this.maxX = Max3(this.p1.X, this.p2.X, this.p3.X)
        member this.maxY = Max3(this.p1.Y, this.p2.Y, this.p3.Y)
        member this.maxZ = Max3(this.p1.Z, this.p2.Z, this.p3.Z)

        static member createWithNormals (p1, p2, p3) (p1Normal, p2Normal, p3Normal) =
            let e1 = p2 - p1
            let e2 = p3 - p1            

            { p1 = p1;
              p2 = p2;
              p3 = p3;
              e1 = e1;
              e2 = e2;
              p1Normal = Vector4(p1Normal, 0.0f)
              p2Normal = Vector4(p2Normal, 0.0f)
              p3Normal = Vector4(p3Normal, 0.0f) }

        static member create invertNormal (p1, p2, p3) =
            let e1 = p2 - p1
            let e2 = p3 - p1

            let normal =
                Vector3.Cross(e2, e1) * (if invertNormal then -1.0f else 1.0f)
                |> Vector3.Normalize

            Triangle.createWithNormals (p1, p2, p3) (normal, normal, normal)              
        
                

    let private intersector interpolateNormals boundingCube (triangles: Triangle list) (ray: Ray) =

        match getTransformedGeometryIntersections ray boundingCube with
        | [] ->
            List.empty
        | _ ->       
            let rayOrigin = Vector3(ray.Origin.X, ray.Origin.Y, ray.Origin.Z)
            let rayDirection = Vector3(ray.Direction.X, ray.Direction.Y, ray.Direction.Z)

            let intersectTriangle triangle =
                let dirCrossE2 = Vector3.Cross(rayDirection, triangle.e2)
                let det = Vector3.Dot(triangle.e1, dirCrossE2)

                if MathF.Abs(det) <= Single.Epsilon then
                    List.empty

                else
                    let f = 1.0f / det
                    let p1ToOrigin = rayOrigin - triangle.p1
                    let u = f * Vector3.Dot(p1ToOrigin, dirCrossE2)

                    if (u < 0.0f) || (u > 1.0f) then
                        List.empty

                    else
                        let originCrossE1 = Vector3.Cross(p1ToOrigin, triangle.e1)
                        let v = f * Vector3.Dot(rayDirection, originCrossE1)

                        if (v < 0.0f) || (u + v) > 1.0f then
                            List.empty

                        else
                            let t = f * Vector3.Dot(triangle.e2, originCrossE1)

                            let normal =
                                if interpolateNormals then
                                    u * triangle.p2Normal + v * triangle.p3Normal + (1.0f - u - v) * triangle.p1Normal
                                else
                                    triangle.p1Normal

                            [{ Distance = t
                               Location = ray.Origin + t * ray.Direction
                               Normal = normal
                               Inside = false }]

            triangles
            |> List.collect intersectTriangle
            |> List.sortBy (fun i -> i.Distance)             


    let createEmptyMesh () =
        Mesh List.empty

    let beginFaceWithNormals mesh: FaceWithNormalsInProgress =
        { Vertices = List.empty; Mesh = mesh }

    let beginFaceWithoutNormals mesh: FaceWithoutNormalsInProgress =
        { Vertices = List.empty; Mesh = mesh }

    let addVertexWithNormal vertex face: FaceWithNormalsInProgress =
        { face with Vertices = vertex :: face.Vertices }

    let addVertexWithoutNormal vertex face: FaceWithoutNormalsInProgress =
        { face with Vertices = vertex :: face.Vertices }

    let endFaceWithNormals ({ Vertices = vertices; Mesh = Mesh faces }: FaceWithNormalsInProgress) =
        Mesh ((FaceWithNormals (List.rev vertices)) :: faces)

    let endFaceWithoutNormals ({ Vertices = vertices; Mesh = Mesh faces }: FaceWithoutNormalsInProgress) =
        Mesh ((FaceWithoutNormals (List.rev vertices)) :: faces)


    let private faceToTriangles invertNormals =
        function
        | FaceWithNormals (VertexWithNormal (p1, n1) :: remaining) when remaining.Length > 1 ->
            remaining
            |> List.pairwise
            |> List.map (fun (VertexWithNormal (p2, n2), VertexWithNormal (p3, n3)) ->
                Triangle.createWithNormals (p1, p2, p3) (n1, n2, n3))

        | FaceWithoutNormals (VertexWithoutNormal p1 :: remaining) when remaining.Length > 1 ->
            remaining
            |> List.pairwise
            |> List.map (fun (VertexWithoutNormal p2, VertexWithoutNormal p3) ->
                Triangle.create invertNormals (p1, p2, p3))

        | _ ->
            failwith "Faces must contain at least 3 vertices"


    let meshToTriangles invertNormals (Mesh faces) =
        faces
        |> List.collect (faceToTriangles invertNormals)


    let trianglesToGeometry interpolateNormals (triangles: Triangle list) =
        let xMin = triangles |> List.map (fun t -> t.minX) |> List.min
        let yMin = triangles |> List.map (fun t -> t.minY) |> List.min
        let zMin = triangles |> List.map (fun t -> t.minZ) |> List.min
        let xMax = triangles |> List.map (fun t -> t.maxX) |> List.max
        let yMax = triangles |> List.map (fun t -> t.maxY) |> List.max
        let zMax = triangles |> List.map (fun t -> t.maxZ) |> List.max

        let xSpan, ySpan, zSpan =
            MathF.Max(xMax - xMin, 0.0001f),
            MathF.Max(yMax - yMin, 0.0001f),
            MathF.Max(zMax - zMin, 0.0001f)

        let transformation =
            applyScaling (xSpan / 2.0f, ySpan / 2.0f, zSpan / 2.0f)
            >> applyTranslation ((xMin + xMax) / 2.0f, (yMin + yMax) / 2.0f, (zMin + zMax) / 2.0f)     

        let boundingCube =
            createTransformedGeometry Cube.geometry transformation

        Geometry ("Triangle Mesh", Intersector (intersector interpolateNormals boundingCube triangles))


    let finalize invertNormals interpolateNormals =
        (meshToTriangles invertNormals) >> (trianglesToGeometry interpolateNormals)       
   


    module OBJFile =
        
        type VertexIDWithNormal =
            | VertexIDWithNormal of VertexID: int * NormalID: int

        type VertexIDWithoutNormal =
            | VertexIDWithoutNormal of VertexID: int            

        type internal Element =
            | VertexElement of Vector3
            | NormalElement of Vector3
            | FaceWithNormalsElement of VertexIDWithNormal list
            | FaceWithoutNormalsElement of VertexIDWithoutNormal list

        type internal Content =
            { Vertices: Map<int, Vector3>
              Normals: Map<int, Vector3>  
              FacesWithNormals: VertexIDWithNormal list list
              FacesWithoutNormals: VertexIDWithoutNormal list list }

            static member Empty =
                { Vertices = Map.empty
                  Normals = Map.empty
                  FacesWithNormals = List.empty
                  FacesWithoutNormals = List.empty }


        let private (|IsFloat|_|) (str: string) =
            match Single.TryParse str with
            | (true, v) -> Some v
            | _ -> None

        let private (|IsInt|_|) (str: string) =
            match Int32.TryParse str with
            | (true, v) -> Some v
            | _ -> None

        let private allValid<'T> (elems: 'T option list) =            
            if elems |> List.forall (fun e -> e.IsSome) then
                elems
                |> List.choose id
                |> Some
            else
                None                        
      
        let private (|IsListOfVertexIDs|_|) elems =
            elems
            |> List.map (function | IsInt v -> Some (VertexIDWithoutNormal v) | _ -> None)
            |> allValid

        let private (|IsVertexIDWithNormal|_|) =
            function
            | Match RegexOptions.None @"(\d+)//(\d+)"
                { GroupValues = [IsInt vertexId; IsInt normalId] } ->
                    Some (VertexIDWithNormal (vertexId, normalId))
            | _ -> None

        let private (|IsVertexIDWithTextureAndNormal|_|) =
            function
            | Match RegexOptions.None @"(\d+)/\d*/(\d+)"
                { GroupValues = [IsInt vertexId; IsInt normalId] } ->
                    Some (VertexIDWithNormal (vertexId, normalId))
            | _ -> None

        let private (|IsListOfVertexIDsWithNormals|_|) elems =
            elems
            |> List.map (function | IsVertexIDWithNormal v -> Some v | _ -> None)
            |> allValid

        let private (|IsListOfVertexIDsWithTexturesAndNormals|_|) elems =
            elems
            |> List.map (function | IsVertexIDWithTextureAndNormal v -> Some v | _ -> None)
            |> allValid  
                
            
        let private parseLine =
            String.trim
            >> String.toLower
            >> String.splitCharWithOptions [|' '|] StringSplitOptions.RemoveEmptyEntries
            >> List.ofArray
            >> function
                | "v" :: [IsFloat x; IsFloat y; IsFloat z] ->
                    Some (VertexElement (Vector3(x, y, z)))

                | "vn" :: [IsFloat dx; IsFloat dy; IsFloat dz] ->
                    Some (NormalElement (Vector3(dx, dy, dz)))

                | "f" :: (IsListOfVertexIDsWithTexturesAndNormals vertices) ->
                    if vertices.Length < 3 then
                        failwith "OBJ faces must contain at least 3 vertices."
                    else
                        vertices |> FaceWithNormalsElement |> Some

                | "f" :: (IsListOfVertexIDsWithNormals vertices) ->
                    if vertices.Length < 3 then
                        failwith "OBJ faces must contain at least 3 vertices."
                    else
                        vertices |> FaceWithNormalsElement |> Some

                | "f" :: (IsListOfVertexIDs vertices) ->
                    if vertices.Length < 3 then
                        failwith "OBJ faces must contain at least 3 vertices."
                    else
                        vertices |> FaceWithoutNormalsElement |> Some
                | _ -> None                


        let private parseElement (content: Content) = function
        | VertexElement v ->
            { content with
                Vertices =
                    content.Vertices
                    |> Map.add (content.Vertices.Count + 1) v }
        | NormalElement n ->
            { content with
                Normals =
                    content.Normals
                    |> Map.add (content.Normals.Count + 1) n }            
        | FaceWithNormalsElement vertices ->
            { content with
                FacesWithNormals =
                    vertices :: content.FacesWithNormals } 
        | FaceWithoutNormalsElement vertices ->
            { content with
                FacesWithoutNormals =
                    vertices :: content.FacesWithoutNormals } 


        let loadToMesh file =
            let content =
                readFile file
                |> Seq.choose parseLine
                |> Seq.fold parseElement Content.Empty

            match content with
            | { Vertices = vertices
                Normals = normals
                FacesWithNormals = facesWithNormals
                FacesWithoutNormals = facesWithoutNormals } ->
                    let processVertexWithNormal (VertexIDWithNormal (vID, nID)) =
                        VertexWithNormal (vertices.[vID], normals.[nID])

                    let processVertexWithoutNormal (VertexIDWithoutNormal vID) =
                        VertexWithoutNormal vertices.[vID]

                    let processFaceWithNormals =
                        List.map processVertexWithNormal >> FaceWithNormals

                    let processFaceWithoutNormals =
                        List.map processVertexWithoutNormal >> FaceWithoutNormals
                    
                    List.map processFaceWithNormals facesWithNormals
                    |> List.append (List.map processFaceWithoutNormals facesWithoutNormals)
                    |> Mesh                    


        let loadToTriangles invertNormals =
            loadToMesh >> (meshToTriangles invertNormals)

        let loadToGeometry invertNormals interpolateNormals =
            (loadToTriangles invertNormals) >> (trianglesToGeometry interpolateNormals)
        