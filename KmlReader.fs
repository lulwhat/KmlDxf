namespace KmlDxf

open System.Xml
open System.Text

module KmlReader =
    let getPoints (root: XmlElement) =
        let ptCoordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "Point")
            |> List.ofSeq

        let ptNames =
            ptCoordNodes
            |> List.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let ptCoords =
            ptCoordNodes
            |> List.map
                (fun node ->
                    node.InnerText.Split [| ',' |]
                    |> Array.map (fun s -> s |> float))

        ptNames, ptCoords

    let getLines (root: XmlElement) =
        let lnCoordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "LineString")
            |> List.ofSeq

        let lnNames =
            lnCoordNodes
            |> List.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let lnCoords =
            lnCoordNodes
            |> List.map
                (fun node ->
                    node.InnerText.Replace(' ', ',').Split [| ',' |]
                    |> function
                        | ar ->
                            ar.[..(ar.Length - 2)]
                            |> Array.map (fun s -> s |> float))

        lnNames, lnCoords

    let getPolygons (root: XmlElement) =
        let polCoordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "LinearRing")
            |> List.ofSeq

        let polNames =
            polCoordNodes
            |> List.map (fun node -> node.ParentNode.ParentNode.ParentNode.ParentNode.FirstChild.InnerText)

        let polCoords =
            polCoordNodes
            |> List.map
                (fun node ->
                    node.InnerText.Replace(' ', ',').Split [| ',' |]
                    |> function
                        | ar ->
                            ar.[..(ar.Length - 2)]
                            |> Array.map (fun s -> s |> float))

        polNames, polCoords

    let getObjects (kmlPath: string) =
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        let kmlXml = new XmlDocument()
        kmlXml.Load kmlPath

        List.concat [ fst (getPoints kmlXml.DocumentElement)
                      fst (getLines kmlXml.DocumentElement)
                      fst (getPolygons kmlXml.DocumentElement) ],
        List.concat [ snd (getPoints kmlXml.DocumentElement)
                      snd (getLines kmlXml.DocumentElement)
                      snd (getPolygons kmlXml.DocumentElement) ]
