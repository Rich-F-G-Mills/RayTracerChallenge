namespace FSRayTracer.Geometries

module Group =

    open FSRayTracer
    open FSRayTracer.Geometries

    type Group =
        | Group of TransformedGeometry list


    let private intersector groupObjects (ray: Ray) =
        groupObjects
        |> List.map (getTransformedGeometryIntersections ray)
        |> List.concat
        |> List.sortBy (fun i -> i.Distance)
        |> List.map transformedToObjectIntersectionPoint


    let createEmptyGroup () =
        Group List.empty<TransformedGeometry>

    let add geometry transformer (Group groupObjects) =
        let newGroupedGeom =
            createTransformedGeometry geometry transformer

        Group (newGroupedGeom :: groupObjects)

    let finalize (Group groupObjects) =
        Geometry ("Group", Intersector (intersector groupObjects))


        
        