
namespace FSRayTracer

[<AutoOpen>]
module public Ray =

    open System.Numerics
    open System.Runtime.CompilerServices


    [<Struct; IsReadOnly>]
    type Ray =
        { Origin: Vector4
          Direction: Vector4 }


    type Vector4 with
        member internal this.SetW (w) =
            Vector4(this.X, this.Y, this.Z, w)

        static member internal Reflect (inVector, normalVector) =
            inVector - 2.0f * normalVector * Vector4.Dot(inVector, normalVector)
