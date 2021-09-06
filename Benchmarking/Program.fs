
open BenchmarkDotNet.Attributes

[<Struct>]
type public PointNoInlineNoByRef =
    { x: double
      y: double
      z: double }

    [<Benchmark>]
    static member (-) (lhs, rhs) =
        { dx = lhs.x - rhs.x; dy = lhs.y - rhs.y; dz = lhs.z - rhs.z }


and [<Struct>] public PointInlineNoByRef =
    { x: double
      y: double
      z: double }

    [<Benchmark>]
    static member inline (-) (lhs, rhs) =
        { dx = lhs.x - rhs.x; dy = lhs.y - rhs.y; dz = lhs.z - rhs.z }

and [<Struct>] PointNoInlineByRef =
    { x: double
      y: double
      z: double }

    [<Benchmark>]
    static member (-) (lhs: inref<PointNoInlineByRef>, rhs: inref<PointNoInlineByRef>) =
        { dx = lhs.x - rhs.x; dy = lhs.y - rhs.y; dz = lhs.z - rhs.z }

and [<Struct>] PointInlineByRef =
    { x: double
      y: double
      z: double }

    [<Benchmark>]
    static member inline (-) (lhs: inref<PointNoInlineByRef>, rhs: inref<PointNoInlineByRef>) =
        { dx = lhs.x - rhs.x; dy = lhs.y - rhs.y; dz = lhs.z - rhs.z }

and [<Struct>] public Vector = 
    { dx: double
      dy: double
      dz: double }


[<EntryPoint>]
let main argv =
    0