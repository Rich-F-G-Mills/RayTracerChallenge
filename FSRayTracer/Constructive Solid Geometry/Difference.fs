namespace FSRayTracer.ConstructiveSolidGeometry

module Difference =

    // When creating differences, it is not possible to generate non-voluminous geometries.

    let private isValidPoint (_, withinStates) =
        match withinStates with
        | WithinNeither, WithinGeometry1Only
        | WithinBoth, WithinGeometry1Only -> Some (Some false)
        | WithinGeometry1Only, WithinNeither
        | WithinGeometry1Only, WithinBoth -> Some (Some true)
        | _ -> None

    let create =
        createIntersector "Difference" isValidPoint