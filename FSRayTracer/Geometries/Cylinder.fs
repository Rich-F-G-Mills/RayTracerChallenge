namespace FSRayTracer.Geometries

module Cylinder =

    open System
    open System.Numerics
    open System.Diagnostics
    open FSRayTracer

    let internal nonCapNormalAt (touchPoint: Vector4) =
        Vector4(touchPoint.X, 0.0f, touchPoint.Z, 0.0f)
        |> Vector4.Normalize


    let internal capNormalAt (touchPoint: Vector4) =
        if touchPoint.Y > 0.0f then
            Vector4.UnitY
        else
            -Vector4.UnitY


    let private capIntersector (ray: Ray) =
        let tToCapIntersectionPoint (t: float32) inside =
            let touchPoint =
                ray.Origin + t * ray.Direction

            if touchPoint.X * touchPoint.X + touchPoint.Z * touchPoint.Z <= 1.0f then
                let normal = capNormalAt (touchPoint)

                Some { Distance = t
                       Location = touchPoint
                       Normal = if inside then -normal else normal
                       Inside = inside }

            else None

        // Ray parallel to caps.
        if MathF.Abs(ray.Direction.Y) <= Single.Epsilon then
            List.Empty 
        
        else
            let t1, t2 =
                (-1.0f - ray.Origin.Y) / ray.Direction.Y, (1.0f - ray.Origin.Y) / ray.Direction.Y

            let t1', t2' =
                if t1 > t2 then t2, t1 else t1, t2
             
            List.choose id [tToCapIntersectionPoint t1' false; tToCapIntersectionPoint t2' true]           


    let private nonCapIntersector (ray: Ray) =
        let tToNonCapIntersectionPoint (t: float32) inside =
            let touchPoint =
                ray.Origin + t * ray.Direction

            if (touchPoint.Y > -1.0f) && (touchPoint.Y < 1.0f) then
                let normal = nonCapNormalAt (touchPoint)

                Some { Distance = t
                       Location = touchPoint
                       Normal = if inside then -normal else normal
                       Inside = inside }

            else None


        let a =
            ray.Direction.X * ray.Direction.X + ray.Direction.Z * ray.Direction.Z

        let b =
            2.0f * (ray.Origin.X * ray.Direction.X + ray.Origin.Z * ray.Direction.Z)

        let c =
            ray.Origin.X * ray.Origin.X + ray.Origin.Z * ray.Origin.Z - 1.0f

        let discriminant = b * b - 4.0f * a * c
         
        // No intersection.
        if discriminant < 0.0f then
            List.empty

        else
            let sqrtDiscriminant = MathF.Sqrt(discriminant)

            let t1, t2 =
                (-b - sqrtDiscriminant) / (2.0f * a), (-b + sqrtDiscriminant) / (2.0f * a)

            List.choose id [tToNonCapIntersectionPoint t1 false; tToNonCapIntersectionPoint t2 true]


    let private intersector (ray: Ray) =
        let intersections =
            capIntersector ray
            |> List.append (nonCapIntersector ray)
            |> List.sortBy (fun i -> i.Distance)

        if intersections.Length = 0 || intersections.Length = 2 then
            intersections
        else
            List.empty


    let geometry =
        Geometry ("Cylinder", Intersector intersector)
            

     