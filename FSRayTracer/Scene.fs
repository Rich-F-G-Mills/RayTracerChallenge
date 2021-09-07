
namespace FSRayTracer


module public Scene =

    open System
    open System.Numerics
    open FSRayTracer.Geometries
    open FSRayTracer.Materials


    type SceneObject =
        { TransformedGeometry: TransformedGeometry
          MaterialMapper: MaterialMapper }


    type Scene =
        | Scene of SceneObject list


    type SceneIntersectionPoint =
        { SceneObject: SceneObject
          WorldLocation: Vector4
          WorldNormal: Vector4
          Distance: float32
          Material: Material
          Inside: bool }


    type ViewSettings =
        { From: Vector3
          To: Vector3
          Up: Vector3 }

        static member Default =
            { From = Vector3.Zero; To = -Vector3.UnitZ; Up = Vector3.UnitY }


    type CameraSettings =
        { Width: float32
          Height: float32
          FieldOfView: float32 }


    type TransformedCamera =
        { PixelSize: float32
          HalfWidth: float32
          HalfHeight: float32
          ViewTransformation: Matrix4x4
          InverseViewTransformation: Matrix4x4 }


    let createEmptyScene () =
        Scene List.empty<SceneObject>


    let add geometry transformer materialMapper (Scene sceneObjects) =
        let newSceneGeom =
            createTransformedGeometry geometry transformer

        let newSceneObject =
            { TransformedGeometry = newSceneGeom; MaterialMapper = materialMapper }

        Scene (newSceneObject :: sceneObjects)


    let transformedToWorldIntersectionPoint sceneObject (transformedIntersection: TransformedIntersectionPoint) =
        { SceneObject = sceneObject
          WorldLocation = transformedIntersection.Location
          WorldNormal = transformedIntersection.Normal
          Distance = transformedIntersection.Distance
          Material = applyMaterialMapper sceneObject.MaterialMapper transformedIntersection.ObjectLocation
          Inside = transformedIntersection.Inside }


    let getWorldIntersections (Scene sceneObjects) ray = 
        let _getTransformedGeometryIntersections =
            getTransformedGeometryIntersections ray

        sceneObjects
        |> List.map (fun sceneObject ->
            _getTransformedGeometryIntersections sceneObject.TransformedGeometry
            |> List.map (transformedToWorldIntersectionPoint sceneObject))
        |> List.concat
        |> List.filter (fun i -> i.Distance >= 0.0f)
        |> List.sortBy (fun i -> i.Distance)


    let getWorldTransformation viewSettings =
        let forward =
            Vector3.Normalize(viewSettings.To - viewSettings.From)

        let up = Vector3.Normalize(viewSettings.Up)

        let left = Vector3.Cross(forward, up)

        let trueUp = Vector3.Cross(left, forward)

        let orientation =
            Matrix4x4(
                left.X, left.Y, left.Z, 0.0f,
                trueUp.X, trueUp.Y, trueUp.Z, 0.0f,
                -forward.X, -forward.Y, -forward.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            ) |> Matrix4x4.Transpose

        let (Transformation translation) =
            createTranslation(-viewSettings.From.X, -viewSettings.From.Y, -viewSettings.From.Z)

        combineTransformationMatrices (translation, orientation)


    let getTransformedCamera cameraSettings worldTransformation =
        let halfView = MathF.Tan(cameraSettings.FieldOfView / 2.0f)
        
        let aspectRatio = cameraSettings.Width / cameraSettings.Height
        
        let halfWidth, halfHeight =
            if aspectRatio >= 1.0f then
                halfView, halfView / aspectRatio
            else
                halfView * aspectRatio, halfView
        
        let pixelSize = halfWidth * 2.0f / cameraSettings.Width

        { PixelSize = pixelSize
          HalfWidth = halfWidth
          HalfHeight = halfHeight
          ViewTransformation = worldTransformation
          InverseViewTransformation = invertTransformationMatrix worldTransformation }


    let getRayForPixel transformedCamera (px, py) =
        let xOffset = (px + 0.5f) * transformedCamera.PixelSize
        let yOffset = (py + 0.5f) * transformedCamera.PixelSize

        let worldX = transformedCamera.HalfWidth - xOffset
        let worldY = transformedCamera.HalfHeight - yOffset

        let pixelLocation =
            Vector4.Transform(
                Vector4(worldX, worldY, -1.0f, 1.0f),
                transformedCamera.InverseViewTransformation
            )

        let originLocation =
            Vector4.Transform(
                Vector4.UnitW,
                transformedCamera.InverseViewTransformation
            )

        let direction = Vector4.Normalize(pixelLocation - originLocation)

        { Origin = originLocation; Direction = direction }


    let render cameraSettings viewSettings scene colourizer =
        let transformedCamera =
            viewSettings
            |> getWorldTransformation
            |> getTransformedCamera cameraSettings

        let _getWorldIntersections = getWorldIntersections scene
        let _getRayForPixel = getRayForPixel transformedCamera
        let _colourizer = colourizer scene

        seq {
            for y in 0.0f .. (cameraSettings.Height - 1.0f) do
                for x in 0.0f .. (cameraSettings.Width - 1.0f) do
                    let ray = _getRayForPixel (x, y)

                    let colour =
                        ray
                        |> _getWorldIntersections
                        |> List.tryHead
                        |> function
                           | Some intersectionPoint ->
                                let (colour: Colour) = _colourizer ray intersectionPoint

                                ValueSome (Colour.Clamp(colour))

                           | None ->
                                ValueNone

                    yield (x, y, colour)
        }
