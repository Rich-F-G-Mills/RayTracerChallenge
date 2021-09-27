
open System.Drawing
open System.Drawing.Imaging
open System
open System.Runtime.InteropServices
open System.Numerics
open System.Collections.Generic
open System.Diagnostics
open System.Threading.Tasks
open FSRayTracer
open FSRayTracer.Lighting
open FSRayTracer.Geometries
open FSRayTracer.Materials
open FSRayTracer.ConstructiveSolidGeometry



[<EntryPoint>]
let main _ =

    let viewSettings =
        { Scene.ViewSettings.Default with From = Vector3(10.0f, 10.0f, 15.0f) }


    let matFloor =
        cartesianBoxMaterial
            (solidMaterial Material.Default)
            (solidMaterial { Material.Default with Colour = Colour.Red })
            (5.0f, 5.0f, 5.0f)

    let matTeapot =
        solidMaterial
            { Material.Default with
                Colour = { R = 0.3f; G = 0.8f; B = 0.3f }
                Reflectivity = 0.0f }
     

    let teapot =
        TriangleMesh.OBJFile.loadToGeometry false true "TEAPOT.TXT"

    let disc =
        Intersection.create
            (Sphere.geometry, applyScaling (10.0f, 10.0f, 10.0f))
            (Plane.geometry, applyNullTransformation)

    let int1 =
        Intersection.create
            (Cube.geometry, applyScaling (4.0f, 4.0f, 4.0f))
            (Sphere.geometry, applyScaling (6.0f, 6.0f, 6.0f))
            


    let scene =
        Scene.createEmptyScene ()
        //|> Scene.add
        //        disc
        //        (applyRotationX (-MathF.PI / 3.0f, Vector3.Zero))
        //        matTeapot
        |> Scene.add
                int1
                applyNullTransformation
                matTeapot
        //|> Scene.add
        //        teapot
        //        (applyScaling (0.3f, 0.3f, 0.3f)
        //         >> applyRotationX (-MathF.PI / 2.0f, Vector3.Zero)
        //         >> applyTranslation (0.0f, 7.5f, 0.0f))
        //        matTeapot
        //|> Scene.add
        //        Plane.geometry
        //        applyNullTransformation
        //        matFloor


    let (lightSource1: LightSource) =
        { Intensity = Colour.White; Position = Vector3(10.0f, 0.0f, 0.0f); }

    let (lightSource2: LightSource) =
        { Intensity = Colour.White; Position = Vector3(0.0f, 30.0f, 30.0f) }      


    let colourizer =
        Colourizer.createDefault 4 (colourFromLighting [lightSource1; lightSource2])

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

    let renderer =
        Scene.createPixelWiseRenderer cameraSettings viewSettings scene colourizer         

    let reciever (px, py) colour =
        let ipx, ipy = int px, int py
        let index = ipx * 3 + ipy * imageOutputData.Stride

        pixelData.[index]       <- byte (255.0f * colour.B)
        pixelData.[index + 1]   <- byte (255.0f * colour.G)
        pixelData.[index + 2]   <- byte (255.0f * colour.R)

    let xPixels =
        [| 0.0f..cameraSettings.Width-1.0f |]

    let renderRow py =
        xPixels
        |> Array.iter (fun px ->
            renderer (px, py)
            |> reciever (px, py))

    let (yPixels: IEnumerable<_>) =
        { 0.0f..cameraSettings.Height-1.0f }

    printfn "Starting render..."

    let timer = Stopwatch()

    timer.Start()

    ignore <| Parallel.ForEach(yPixels, renderRow)

    //toProcess |> Seq.iter (reciever Colour.Black)

    timer.Stop()

    printfn "Finished. Time taken = %ims" timer.ElapsedMilliseconds

    Marshal.Copy (pixelData, 0, imageOutputData.Scan0, pixelData.Length)

    imageOutput.UnlockBits(imageOutputData)
    
    imageOutput.Save("IMAGE.BMP")

    0