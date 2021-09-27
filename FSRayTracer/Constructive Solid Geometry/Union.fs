namespace FSRayTracer.ConstructiveSolidGeometry

module Union =

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | WithinNeither, WithinGeometry1Only
        | WithinNeither, WithinGeometry2Only -> Some (Some false)
        | WithinNeither, TouchingGeometry1Only
        | WithinNeither, TouchingGeometry2Only -> Some None
        | WithinGeometry1Only, WithinNeither
        | WithinGeometry2Only, WithinNeither -> Some (Some true)
        | _ -> None

    let create =
        createIntersector "Union" isValidPoint
