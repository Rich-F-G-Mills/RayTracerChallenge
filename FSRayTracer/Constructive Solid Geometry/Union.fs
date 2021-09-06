namespace FSRayTracer.ConstructiveSolidGeometry

module Union =

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | Neither, Geometry1Only
        | Neither, Geometry2Only -> Some false
        | _, Neither -> Some true
        | _ -> None

    let create =
        createIntersector "Union" isValidPoint
