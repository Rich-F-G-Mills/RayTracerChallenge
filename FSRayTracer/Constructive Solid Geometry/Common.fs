namespace FSRayTracer.ConstructiveSolidGeometry

[<AutoOpen>]
module Common =

    open FSRayTracer
    open FSRayTracer.Geometries


    type internal IntersectionStatus =
        // Note that there is no instance where we can be 'touching' both geometries.
        // 
        | WithinNeither 
        | WithinGeometry1Only
        | WithinGeometry2Only
        | WithinGeometry1AndTouchingGeometry2
        | WithinGeometry2AndTouchingGeomerty1
        | TouchingGeometry1Only
        | TouchingGeometry2Only
        | WithinBoth

    type private SelectedGeometry =
        | Geometry1
        | Geometry2


    let private updateWithinState within (attributedIntersection: SelectedGeometry * TransformedIntersectionPoint) =
        match within with
        | WithinNeither ->
            match attributedIntersection with
            | Geometry1, { Inside = Some false } -> WithinGeometry1Only
            | Geometry2, { Inside = Some false } -> WithinGeometry2Only
            | _, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry1, { Inside = None } -> TouchingGeometry1Only
            | Geometry2, { Inside = None } -> TouchingGeometry2Only

        | WithinGeometry1Only ->
            match attributedIntersection with            
            | Geometry1, { Inside = Some false } ->
                failwith "Cannot enter a geometry again before leaving it."
            | Geometry2, { Inside = Some false } -> WithinBoth
            | Geometry1, { Inside = Some true } -> WithinNeither
            | Geometry2, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry1, { Inside = None } ->
                failwith "A geometry cannot intersect its own non-voluminous section whilst within it."
            | Geometry2, { Inside = None } -> WithinGeometry1AndTouchingGeometry2

        | WithinGeometry2Only ->
            match attributedIntersection with            
            | Geometry1, { Inside = Some false } -> WithinBoth                
            | Geometry2, { Inside = Some false } ->
                failwith "Cannot enter a geometry again before leaving it."
            | Geometry1, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry2, { Inside = Some true } -> WithinNeither
            | Geometry1, { Inside = None } -> WithinGeometry2AndTouchingGeomerty1                
            | Geometry2, { Inside = None } ->
                failwith "A geometry cannot intersect its own non-voluminous section whilst within it."

        | WithinGeometry1AndTouchingGeometry2 ->
            match attributedIntersection with
            | Geometry1, { Inside = Some false } ->
                failwith "Cannot enter a geometry again before leaving it."
            | Geometry2, { Inside = Some false } -> WithinBoth
            // Although we have now left the first geometry, we can also no longer
            // be touching the second (non-voluminous) geometry.
            | Geometry1, { Inside = Some true } -> WithinNeither
            | Geometry2, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!" 
            | Geometry1, { Inside = None } ->
                failwith "A geometry cannot intersect its own non-voluminous section whilst within it."
            | Geometry2, { Inside = None } -> WithinGeometry1AndTouchingGeometry2

        | WithinGeometry2AndTouchingGeomerty1 ->
            match attributedIntersection with
            | Geometry1, { Inside = Some false } -> WithinBoth
            // Although we have now left the first geometry, we can also no longer
            // be touching the second (non-voluminous) geometry.
            | Geometry2, { Inside = Some false } ->
                failwith "Cannot enter a geometry again before leaving it."            
            | Geometry1, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry2, { Inside = Some true } -> WithinNeither
            | Geometry1, { Inside = None } -> WithinGeometry2AndTouchingGeomerty1
            | Geometry2, { Inside = None } ->
                failwith "A geometry cannot intersect its own non-voluminous section whilst within it."

        | TouchingGeometry1Only
        | TouchingGeometry2Only ->
            match attributedIntersection with
            | Geometry1, { Inside = Some false } -> WithinGeometry1Only
            | Geometry2, { Inside = Some false } -> WithinGeometry2Only          
            | _, { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry1, { Inside = None } -> TouchingGeometry1Only
            | Geometry2, { Inside = None } -> TouchingGeometry2Only

        | WithinBoth ->
            match attributedIntersection with
            | _, { Inside = Some false } ->
                failwith "Cannot enter a geometry again before leaving it."             
            | Geometry1, { Inside = Some true } -> WithinGeometry2Only
            | Geometry2, { Inside = Some true } -> WithinGeometry1Only
            | _, { Inside = None } ->
                failwith "A geometry cannot intersect its own non-voluminous section whilst within it."
       

    let internal createIntersector name pointValidator (geometry1, transformer1) (geometry2, transformer2) =
        let transformedGeometry1 =
            createTransformedGeometry geometry1 transformer1

        let transformedGeometry2 =
            createTransformedGeometry geometry2 transformer2

        let intersector (ray: Ray) =
            let _getTransformedGeometryIntersections =
                getTransformedGeometryIntersections ray

            let is1 =
                _getTransformedGeometryIntersections transformedGeometry1
                |> List.map (fun i -> (Geometry1, i))

            let is2 =
                _getTransformedGeometryIntersections transformedGeometry2
                |> List.map (fun i -> (Geometry2, i))

            let intersections =
                is1
                |> List.append is2
                |> List.sortBy (fun (_, i) -> i.Distance)

            let validIntersections =
                intersections
                |> List.scan updateWithinState WithinNeither
                |> List.pairwise
                |> List.zip (intersections |> List.map snd)
                |> List.choose (fun ((ti, _) as i) ->
                    match pointValidator i with
                    | Some insideStatus -> Some { ti with Inside = insideStatus }
                    | None -> None)
                |> List.map transformedIntersectionToObjectIntersection

            validIntersections


        Geometry (name, Intersector intersector)