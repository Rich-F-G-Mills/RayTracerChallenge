
open System.Drawing
open System.Drawing.Imaging
open System
open System.Runtime.InteropServices
open System.Numerics
open FSRayTracer
open FSRayTracer.Geometries
open FSRayTracer.Materials
open FSRayTracer.ConstructiveSolidGeometry


[<EntryPoint>]
let main _ =

    let viewSettings =
        { Scene.ViewSettings.Default with From = Vector3(5.0f, 7.5f, 10.0f) }


    let material1 =
        solidMaterial { Material.Default with Colour = Colour.Red; }

    let material2 =
        solidMaterial
            { Material.Default with Colour = Colour.Blue; }
     


    let difference1 =
        Difference.create
            (Cube.geometry |> rename "Main cube", applyNullTransformation)
            (Sphere.geometry |> rename "Main sphere", (applyScaling (1.3f, 1.3f, 1.3f)))
        |> rename "difference1"

    let intersection1 =
        Intersection.create
            (difference1, applyNullTransformation)
            (Sphere.geometry, (applyScaling (1.4f, 1.4f, 1.4f)))
        |> rename "intersection1"

    let difference2 =
        Difference.create
            (Cylinder.geometry, applyScaling (3.0f, 5.0f, 3.0f))
            (Cylinder.geometry, (applyScaling (2.5f, 5.0f, 2.5f)) >> (applyTranslation (0.0f, 0.5f, 0.0f)))
        |> rename "difference2"

    let difference3 =
        Difference.create
            (difference2, applyNullTransformation)
            (Cylinder.geometry,
                (applyScaling (2.0f, 5.0f, 2.0f))
                >> (applyRotationX (MathF.PI / 2.0f, Vector3.Zero))
                >> (applyRotationY (MathF.PI / 4.0f, Vector3.Zero))
                >> (applyTranslation (0.0f, -0.5f, 0.0f)))
        |> rename "difference3"


    let group1 =
        Group.createEmptyGroup ()
        |> Group.add Sphere.geometry (applyTranslation (-0.5f, 0.0f, 0.0f))
        |> Group.add Sphere.geometry (applyTranslation (0.5f, 0.0f, 0.0f))
        |> Group.finalize


    let teapot =
        //TriangleMesh.OBJFile.loadToGeometry true false "teapot.obj"
        TriangleMesh.OBJFile.loadToGeometry false true "teapot2.obj"


    let scene =
        Scene.createEmptyScene ()
        |> Scene.add
                teapot
                //(applyScaling (2.0f, 2.0f, 2.0f))
                (applyScaling (0.4f, 0.4f, 0.4f)
                 >> applyRotationX (-MathF.PI * 1.0f / 3.0f, Vector3.Zero))
                material2
        |> Scene.add
                Plane.geometry
                (applyRotationX (-MathF.PI * 3.0f / 4.0f, Vector3.Zero)
                >> applyTranslation (0.0f, 0.0f, 50.0f))
                material1


    let (lightSource1: LightSource) =
        { Intensity = Colour.White; Position = Vector3(0.0f, 30.0f, 0.0f); }

    let (lightSource2: LightSource) =
        { Intensity = Colour.White; Position = Vector3(0.0f, -30.0f, 0.0f) }      


    let colourizer =
        usePhongLighting [lightSource1; lightSource2]
        |> withReflections 4


    use imageOutput =
        new Bitmap(1000, 1000, PixelFormat.Format24bppRgb)

    let (cameraSettings: Scene.CameraSettings) =
        { Width = float32 imageOutput.Width
          Height = float32 imageOutput.Width
          FieldOfView = MathF.PI / 2.0f }

    let imageOutputData =
        imageOutput.LockBits(
            Rectangle(0, 0, imageOutput.Width, imageOutput.Height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb)

    let pixelData =
        Array.create (imageOutputData.Stride * imageOutputData.Height) 0uy

    let reciever defaultColour (px, py, colour: Colour voption) =
        let colour =
            colour
            |> ValueOption.defaultValue defaultColour

        let ipx, ipy = int px, int py
        let index = ipx * 3 + ipy * imageOutputData.Stride

        if (ipx = 0) && (ipy % 10 = 0) then
            Console.WriteLine($"Processing y = {ipy}")

        pixelData.[index]       <- byte (255.0f * colour.B)
        pixelData.[index + 1]   <- byte (255.0f * colour.G)
        pixelData.[index + 2]   <- byte (255.0f * colour.R)

    Scene.render cameraSettings viewSettings scene colourizer
    |> Seq.iter (reciever Colour.Black)

    Marshal.Copy (pixelData, 0, imageOutputData.Scan0, pixelData.Length)

    imageOutput.UnlockBits(imageOutputData)
    
    imageOutput.Save("IMAGE.BMP")

    0