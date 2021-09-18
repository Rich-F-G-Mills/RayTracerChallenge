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
        |> List.map transformedIntersectionToObjectIntersection


    /// <summary>
    /// Create an empty group to which geometries and their respective transformations can later be added.
    /// </summary>
    let createEmptyGroup () =
        Group List.empty<TransformedGeometry>

    /// <summary>
    /// Add a geometry along with its transformation to an existing group.
    /// </summary>
    let add geometry transformer (Group groupObjects) =
        let newGroupedGeom =
            createTransformedGeometry geometry transformer

        Group (newGroupedGeom :: groupObjects)

    /// <summary>
    /// Build the group based on the specified child geometries.
    /// </summary>
    let finalize (Group groupObjects) =
        Geometry ("Group", Intersector (intersector groupObjects))


        
        