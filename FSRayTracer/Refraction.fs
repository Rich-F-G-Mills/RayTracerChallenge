
namespace FSRayTracer

module Refraction =

    open FSRayTracer.Scene


    let private transitionThroughObject status intersection =
        // With constructuve solid geometry (CSG), the presence of intersection points
        // where the 'Inside' property is None is not possible as CSG is only
        // designed to work with geometries that have volume.
        // Here, we are more forgiving as (for example) a group could contain
        // both a sphere and a plane.

        match status with
        | None as v ->
            match intersection with
            | { Inside = None } -> v
            | { Distance = d; Inside = Some false; Material = m } ->
                Some (d, m.RefractiveIndex)
            | { Inside = Some true } ->
                failwith "Cannot leave a geometry before entering it!"

        | Some _ ->
            match intersection with
            | { Inside = None } ->
                failwith "Cannot enter non-voluminous geometry whilst within volume."
            | { Inside = Some false } ->
                failwith "Cannot enter the same geometry twice in succession!"
            | { Inside = Some true } -> None


    let inline private sceneObject intersection =
        intersection.SceneObject

    let inline private getClosingInsideStatus (_, intersections) =
        intersections
        |> List.fold transitionThroughObject None
                
    let inline private hasYetToLeaveObject (closingInside: (float32 * float32) option) =
        match closingInside with
        | Some _ as v -> v
        | None -> None


    let private getRefractiveIndices =     
        // Represents the RI of the material through which the ray was passing
        // before hitting the first intersection point with a non-negative distance.
            
        // Because intersections are ordered by distance, this should be more performant.
        List.takeWhile (not << hasPositiveDistance)
        >> List.groupBy sceneObject            
        >> List.choose getClosingInsideStatus
        >> List.sortByDescending fst
        >> List.map snd
        >> function
            | [] -> (1.0f, 1.0f)
            | [ri] -> (ri, 1.0f)
            | ri1::ri2::_ -> (ri1, ri2)


    let getRefractedColour colourizer scene ray intersectionPoints =
        intersectionPoints
        |> List.tryFind hasPositiveDistance
        |> function
            | Some nextIntersection ->
                let priorRI, priorPriorRI =
                    getRefractiveIndices intersectionPoints
                
                let n1, n2 =
                    match nextIntersection.Inside with
                    | None -> (priorRI, priorRI)
                    | Some false -> (priorRI, nextIntersection.Material.RefractiveIndex)
                    | Some true -> (nextIntersection.Material.RefractiveIndex, priorPriorRI)

                Colour.Black

            | None -> Colour.Black




        
        







