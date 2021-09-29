namespace KmlDxf

open DotSpatial.Projections

module Reprojections =
    let reproject projString (coordsSeq: seq<string * float []>) =
        let crsWgs = ProjectionInfo.FromEpsgCode(4326)

        let crsTarget =
            ProjectionInfo.FromProj4String(projString)

        // extract xy float arrays for every object and put to list
        let xy =
            Seq.map (fun ln -> Array.indexed (snd ln)) coordsSeq
            |> Seq.map (fun ar -> Array.filter (fun (i: int, coord: float) -> (i + 1) % 3 <> 0) ar)
            |> Seq.map (fun ar -> Array.map (fun (_, coord: float) -> coord) ar)
            |> Seq.toList

        // extract z float arrays for every object and put to list
        let z =
            Seq.map (fun ln -> Array.indexed (snd ln)) coordsSeq
            |> Seq.map (fun ar -> Array.filter (fun (i: int, coord: float) -> (i + 1) % 3 = 0) ar)
            |> Seq.map (fun ar -> Array.map (fun (_, coord: float) -> coord) ar)
            |> Seq.toList

        // iterate xy and z lists, reproject and store in new lists
        // reverse lists after tail recursion to return in right order
        let rec reprojXYZ toReprojectXY toReprojectZ outputXY outputZ =
            (toReprojectXY, toReprojectZ)
            |> function
                | [], [] -> List.rev outputXY, List.rev outputZ
                | headXY :: tailXY, headZ :: tailZ ->
                    Reproject.ReprojectPoints(headXY, headZ, crsWgs, crsTarget, 0, headZ.Length)
                    reprojXYZ tailXY tailZ (headXY :: outputXY) (headZ :: outputZ)
                | _ -> outputXY, outputZ

        // fold coordinates and names to list
        (List.ofSeq (Seq.map (fun (names, _) -> names) coordsSeq), reprojXYZ xy z [] [])
        |> function
            | (names, (reprojectedXY, reprojectedZ)) ->
                List.map3
                    (fun names reprojectedXY reprojectedZ -> names, reprojectedXY, reprojectedZ)
                    names
                    reprojectedXY
                    reprojectedZ
