namespace FSRayTracer.ConstructiveSolidGeometry

[<AutoOpen>]
module Common =

    open System.Diagnostics
    open FSRayTracer
    open FSRayTracer.Geometries


    type internal IntersectionWithin =
        | Neither 
        | Geometry1Only
        | Geometry2Only
        | Both

    type private SelectedGeometry =
        | Geometry1
        | Geometry2


    let private updateWithinState within (attributedIntersection: SelectedGeometry * TransformedIntersectionPoint) =
        match within with
        | Neither ->
            match attributedIntersection with
            | Geometry1, { Inside = false } -> Geometry1Only
            | Geometry2, { Inside = false } -> Geometry2Only
            | _ -> Neither

        | Geometry1Only ->
            match attributedIntersection with
            | Geometry1, { Inside = false } ->
                failwith "Cannot enter the same geometry twice!"
            | Geometry2, { Inside = false } -> Both
            | Geometry1, { Inside = true } -> Neither
            | Geometry2, { Inside = true } ->
                failwith "Cannot leave a geometry before entering it!"

        | Geometry2Only ->
            match attributedIntersection with
            | Geometry1, { Inside = false } -> Both
            | Geometry2, { Inside = false } ->
                failwith "Cannot enter the same geometry twice!"
            | Geometry1, { Inside = true } ->
                failwith "Cannot leave a geometry before entering it!"
            | Geometry2, { Inside = true } -> Neither                  

        | Both ->
            match attributedIntersection with
            | Geometry1, { Inside = false }
            | Geometry2, { Inside = false } ->
                failwith "Cannot enter the same geometry twice!"
            | Geometry1, { Inside = true } -> Geometry2Only
            | Geometry2, { Inside = true } -> Geometry1Only


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
                |> List.scan updateWithinState Neither
                |> List.pairwise
                |> List.zip (intersections |> List.map snd)
                |> List.choose (fun ((t, _) as i) ->
                    match pointValidator i with
                    | Some inside -> Some {t with Inside = inside}
                    | None -> None)
                |> List.map transformedIntersectionToObjectIntersection

            validIntersections


        Geometry (name, Intersector intersector)