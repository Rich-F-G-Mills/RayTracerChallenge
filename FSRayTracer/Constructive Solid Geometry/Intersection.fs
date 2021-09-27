namespace FSRayTracer.ConstructiveSolidGeometry

module Intersection =

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | _, WithinBoth -> Some (Some false)
        | WithinBoth, _ -> Some (Some true)
        | _, WithinGeometry1AndTouchingGeometry2
        | _, WithinGeometry2AndTouchingGeomerty1 -> Some None
        | _ -> None

    let create =
        createIntersector "Intersection" isValidPoint
