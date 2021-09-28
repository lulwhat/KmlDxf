namespace KmlDxf

open netDxf
open netDxf.Entities
open netDxf.Tables

module DxfWriter =
    let writeLines (doc: DxfDocument) (lines: List<(string * float [] * float [])>) =
        List.iter
            (fun (name: string, xy: float [], z: float []) ->
                let polyVertices =
                    [ for i in 0 .. 2 .. (xy.Length - 1) do
                          yield LwPolylineVertex(new Vector2(xy.[i], xy.[i + 1])) ]

                let poly = new LwPolyline(polyVertices)
                poly.Elevation <- z.[0]
                doc.AddEntity poly
                
                if name <> "" then
                    let textMark =
                        new Text(name, Vector3(xy.[0], xy.[1], z.[0]), 10., TextStyle.Default)

                    doc.AddEntity textMark)

            lines

        doc

    let writePoints (doc: DxfDocument) (points: List<(string * float [] * float [])>) =

        0

    let saveDxf (dxfFile: string) (doc: DxfDocument) = doc.Save dxfFile |> ignore