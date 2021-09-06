namespace FSRayTracer.ConstructiveSolidGeometry

module Difference =

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | _, Geometry1Only -> Some false
        | Geometry1Only, _ -> Some true
        | _ -> None

    let create =
        createIntersector "Difference" isValidPoint