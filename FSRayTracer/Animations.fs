
namespace FSRayTracer

open FSRayTracer.Shapes

module public Animations =

    open System
    open System.Numerics


    type AnimatedTransformation =
        | UnboundedTransformation of Func: (float32 -> Transformation)
        | BoundedTransformation of Func: (float32 -> Transformation) * Duration: float32


    let run func =
        match func with
        | UnboundedTransformation func ->
            fun time -> func time

        | BoundedTransformation (func, duration) ->
            let closingTransformation = func duration

            fun time ->
                if time < duration then func time
                else closingTransformation

  
    let (-->) lhs rhs =
        match lhs with
        | UnboundedTransformation _ -> lhs
        | BoundedTransformation (_, lhsDuration) ->
            let lhsRunner = run lhs
            let rhsRunner = run rhs

            match rhs with
            | UnboundedTransformation _ ->
                let transformer time =
                    let lhsTransformation = lhsRunner time

                    if time < lhsDuration then
                        lhsTransformation

                    else
                        let rhsTransformation = rhsRunner (time - lhsDuration)

                        combineTransformations lhsTransformation rhsTransformation

                UnboundedTransformation transformer

            | BoundedTransformation (_, rhsDuration) ->
                let transformer time =
                    let lhsTransformation = lhsRunner time

                    if time < lhsDuration then
                        lhsTransformation

                    else
                        let rhsTransformation = rhsRunner (time - lhsDuration)
                        
                        combineTransformations lhsTransformation rhsTransformation

                BoundedTransformation (transformer, lhsDuration + rhsDuration)


    let (<->) lhs rhs =
        let lhsRunner = run lhs
        let rhsRunner = run rhs

        let transformer time =
            let lhsTransformation = lhsRunner time
            let rhsTransformation = rhsRunner time

            combineTransformations lhsTransformation rhsTransformation

        match lhs with
        | UnboundedTransformation _ ->
            UnboundedTransformation transformer

        | BoundedTransformation (_, lhsDuration) ->
            match rhs with
            | UnboundedTransformation _ ->
                UnboundedTransformation transformer

            | BoundedTransformation (_, rhsDuration) ->
                BoundedTransformation (transformer, MathF.Max(lhsDuration, rhsDuration))
 
 
    let repeat (n: int option) transformer =
        match transformer with
        | UnboundedTransformation _ -> transformer
        | BoundedTransformation (func, duration) ->
            let transformer time = func (time % duration)

            match n with
            | Some n -> BoundedTransformation (transformer, duration * float32(n))
            | None -> UnboundedTransformation transformer


    let forDuration length transformer =
        match transformer with
        | UnboundedTransformation func ->
            BoundedTransformation (func, length)

        | BoundedTransformation (func, _) -> 
            BoundedTransformation (func, length)


    let immediate transformer =
        match transformer with
        | UnboundedTransformation func ->
            let transformation = func 1.0f

            let transformer _ = transformation

            BoundedTransformation (transformer, 0.0f)

        | BoundedTransformation (func, duration) ->
            let transformation = func duration
            
            let transformer _ = transformation
            
            BoundedTransformation (transformer, 0.0f)


    let wait duration =
        let transformer _ = identityTransformation

        BoundedTransformation (transformer, duration)


    let translate (dx, dy, dz) =
        let delta = Vector3(dx, dy, dz)

        let transformer (time: float32) =
            createTranslation (dx * time, dy * time, dz * time)

        UnboundedTransformation transformer


    let rotateX rate (x, y, z) =
        let center = Vector3(x, y, z)

        let transformer time =
            createRotationX (rate * time, center)

        UnboundedTransformation transformer
    