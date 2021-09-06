namespace FSRayTracer.ConstructiveSolidGeometry

module Intersection =

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | _, Both -> Some false
        | Both, _ -> Some true
        | _ -> None

    let create =
        createIntersector "Intersection" isValidPoint
