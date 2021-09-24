namespace KmlDxf

open System.Xml

module KmlReader =
    let root (kmlPath: string) =
        let kmlXml = new XmlDocument()
        let loadKml = kmlXml.Load kmlPath
        kmlXml.DocumentElement

    let getPoints (root: XmlElement) =
        let coordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "Point")

        let names =
            coordNodes
            |> Seq.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let coords =
            coordNodes |> Seq.map (fun node -> node.InnerText)

        Seq.map2 (fun name crd -> (name, crd)) names coords

    let getLines (root: XmlElement) =
        let coordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "LineString")

        let names =
            coordNodes
            |> Seq.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let coords =
            coordNodes |> Seq.map (fun node -> node.InnerText)

        Seq.map2 (fun name crd -> (name, crd)) names coords
