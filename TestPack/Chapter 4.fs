namespace FSRayTracerUnitTests

open System
open System.Numerics
open Xunit
open Xunit.Abstractions

open FSRayTracer.Geometries


type public ``Chapter 04: Matrix Transformation`` (output: ITestOutputHelper) =

    [<Fact>]
    let ``Multiplying by a translation matrix`` () =       
        let (Transformation t) = createTranslation (5.0f, -3.0f, 2.0f)

        let p = Vector4(-3.0f, 4.0f, 5.0f, 1.0f)

        Assert.Equal(
            Vector4(2.0f, 1.0f, 7.0f, 1.0f),
            Vector4.Transform(p, t)
        )

    [<Fact>]
    let ``Multiplying by the inverse of a translation matrix`` () =       
        let t =
            createTranslation (5.0f, -3.0f, 2.0f)
            |> invertTransformation

        let p = Vector4(-3.0f, 4.0f, 5.0f, 1.0f)

        Assert.Equal(
            Vector4(-8.0f, 7.0f, 3.0f, 1.0f),
            Vector4.Transform(p, t)
        )

    [<Fact>]
    let ``Translation does not affect vectors`` () =       
        let t =
            createTranslation (5.0f, -3.0f, 2.0f)
            |> invertTransformation

        let v = Vector4(-3.0f, 4.0f, 5.0f, 0.0f)

        Assert.Equal(v, Vector4.Transform(v, t))

    [<Fact>]
    let ``A scaling matrix applied to a point`` () =       
        let (Transformation t) =
            createScaling (2.0f, 3.0f, 4.0f)

        let p = Vector4(-4.0f, 6.0f, 8.0f, 1.0f)

        Assert.Equal(
            Vector4(-8.0f, 18.0f, 32.0f, 1.0f),
            Vector4.Transform(p, t)
        )

    [<Fact>]
    let ``A scaling matrix applied to a vector`` () =       
        let (Transformation t) =
            createScaling (2.0f, 3.0f, 4.0f)

        let p = Vector4(-4.0f, 6.0f, 8.0f, 0.0f)

        Assert.Equal(
            Vector4(-8.0f, 18.0f, 32.0f, 0.0f),
            Vector4.Transform(p, t)
        )

    [<Fact>]
    let ``Multiplying by the inverse of a scaling matrix`` () =       
        let t =
            createScaling (2.0f, 3.0f, 4.0f)
            |> invertTransformation

        let p = Vector4(-4.0f, 6.0f, 8.0f, 0.0f)

        Assert.Equal(
            Vector4(-2.0f, 2.0f, 2.0f, 0.0f),
            Vector4.Transform(p, t)
        )

    [<Fact>]
    let ``Rotating a point around the X axis`` () =       
        let (Transformation t1) =
            createRotationX (MathF.PI / 4.0f, Vector3.Zero)
            
        let (Transformation t2) =
            createRotationX (MathF.PI / 2.0f, Vector3.Zero)   

        let p = Vector4(0.0f, 1.0f, 0.0f, 1.0f)

        Assert.True(
            Vector4.Distance(
                Vector4(0.0f, MathF.Sqrt(2.0f) / 2.0f, MathF.Sqrt(2.0f) / 2.0f, 1.0f),
                Vector4.Transform(p, t1)
            ) < 0.00001f
        )

        Assert.True(
            Vector4.Distance(
                    Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                    Vector4.Transform(p, t2)
            ) < 0.00001f
        )

    [<Fact>]
    let ``Rotating a point around the Y axis`` () =       
        let (Transformation t1) =
            createRotationY (MathF.PI / 4.0f, Vector3.Zero)
            
        let (Transformation t2) =
            createRotationY (MathF.PI / 2.0f, Vector3.Zero)   

        let p = Vector4(0.0f, 0.0f, 1.0f, 1.0f)

        Assert.True(
            Vector4.Distance(
                Vector4(MathF.Sqrt(2.0f) / 2.0f, 0.0f, MathF.Sqrt(2.0f) / 2.0f, 1.0f),
                Vector4.Transform(p, t1)
            ) < 0.00001f
        )

        Assert.True(
            Vector4.Distance(
                    Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    Vector4.Transform(p, t2)
            ) < 0.00001f
        )

    [<Fact>]
    let ``Rotating a point around the Z axis`` () =       
        let (Transformation t1) =
            createRotationZ (MathF.PI / 4.0f, Vector3.Zero)
        
        let (Transformation t2) =
            createRotationZ (MathF.PI / 2.0f, Vector3.Zero)   

        let p = Vector4(0.0f, 1.0f, 0.0f, 1.0f)

        Assert.True(
            Vector4.Distance(
                Vector4(-MathF.Sqrt(2.0f) / 2.0f, MathF.Sqrt(2.0f) / 2.0f, 0.0f, 1.0f),
                Vector4.Transform(p, t1)
            ) < 0.00001f
        )

        Assert.True(
            Vector4.Distance(
                    Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                    Vector4.Transform(p, t2)
            ) < 0.00001f
        )

    [<Theory>]
    [<InlineData(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 3.0f, 4.0f)>]
    [<InlineData(0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 6.0f, 3.0f, 4.0f)>]
    [<InlineData(0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 2.0f, 5.0f, 4.0f)>]
    [<InlineData(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 2.0f, 7.0f, 4.0f)>]
    [<InlineData(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 2.0f, 3.0f, 6.0f)>]
    [<InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 2.0f, 3.0f, 7.0f)>]
    let ``A shearing transformation applied to a point`` (xInPropY, xInPropZ, yInPropX, yInPropZ, zInPropX, zInPropY, expX, expY, expZ) =
        let (Transformation t) =
            createShearing (xInPropY, xInPropZ, yInPropX, yInPropZ, zInPropX, zInPropY)

        let p = Vector4(2.0f, 3.0f, 4.0f, 1.0f)

        let result = Vector4.Transform(p, t)

        output.WriteLine(result.ToString())

        Assert.True(
            Vector4.Distance(
                    Vector4(expX, expY, expZ, 1.0f),
                    result
            ) < 0.00001f
        )