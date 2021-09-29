namespace KmlDxf

open netDxf
open netDxf.Entities
open netDxf.Tables

module DxfWriter =
    let writeDxf (dxfFile: string) (objects: List<(string * float [] * float [])>) =
        let doc = new DxfDocument()

        List.iter
            (fun (name: string, xy: float [], z: float []) ->
                if xy.Length > 2 then
                    // Polyline case
                    let polyVertices =
                        [ for i in 0 .. 2 .. (xy.Length - 1) do
                              yield LwPolylineVertex(new Vector2(xy.[i], xy.[i + 1])) ]

                    let poly = new LwPolyline(polyVertices)
                    poly.Elevation <- z.[0]
                    doc.AddEntity poly
                else
                    // Point case
                    let pt = Point(xy.[0], xy.[1], z.[0])
                    doc.AddEntity pt

                if name <> "" then
                    let textMark =
                        new Text(name, Vector3(xy.[0], xy.[1], z.[0]), 10., TextStyle.Default)

                    doc.AddEntity textMark)

            objects

        doc.Save dxfFile |> ignore
