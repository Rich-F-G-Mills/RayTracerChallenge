
namespace FSRayTracer

[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("TestPack")>]
[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OBJReader")>]
do()


[<AutoOpen>]
module public Colour =

    open System
    open System.Runtime.CompilerServices


    let private clamp n =
        MathF.Max(MathF.Min(n, 1.0f), 0.0f)


    [<Struct; IsReadOnly>]
    type Colour =
        { R: float32
          G: float32
          B: float32 }

        override this.ToString() =
            $"Colour ({this.R}, {this.G}, {this.B})"


        static member Clamp (c: Colour) =
            { R = clamp(c.R); G = clamp(c.G); B = clamp(c.B) }

        static member (+) (lhs: Colour, rhs: Colour) =
            { R = lhs.R + rhs.R; G = lhs.G + rhs.G; B = lhs.B + rhs.B }

        /// Hadamard product.
        static member (*) (lhs: Colour, rhs: Colour) =
            { R = lhs.R * rhs.R; G = lhs.G * rhs.G; B = lhs.B * rhs.B }

        /// Hadamard product.
        static member (*) (lhs: Colour, rhs: float32) =
            { R = lhs.R * rhs; G = lhs.G * rhs; B = lhs.B * rhs }


    let Black =   { R = 0.0f; G = 0.0f; B = 0.0f }
    let White =   { R = 1.0f; G = 1.0f; B = 1.0f }
    let Red =     { R = 1.0f; G = 0.0f; B = 0.0f }
    let Green =   { R = 0.0f; G = 1.0f; B = 0.0f }
    let Blue =    { R = 0.0f; G = 0.0f; B = 1.0f }
    