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

        let ptNames =
            ptCoordNodes
            |> Seq.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let ptCoords =
            ptCoordNodes
            |> Seq.map (fun node -> node.InnerText)
            |> Seq.map (fun s -> s.Split [| ',' |])
            |> Seq.map (fun ar -> Array.map (fun s -> s |> float) ar)

        Seq.map2 (fun name crd -> (name, crd)) ptNames ptCoords

    let getLines (root: XmlElement) =
        let lnCoordNodes =
            root.SelectNodes "//*"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node ->
                    node.Name = "coordinates"
                    && node.ParentNode.Name = "LineString")

        let lnNames =
            lnCoordNodes
            |> Seq.map (fun node -> node.ParentNode.ParentNode.FirstChild.InnerText)

        let lnCoords =
            lnCoordNodes
            |> Seq.map (fun node -> node.InnerText)
            |> Seq.map (fun s -> s.Replace(' ', ','))
            |> Seq.map (fun s -> s.Split [| ',' |])
            |> Seq.map (fun ar -> Array.take (ar.Length - 1) ar)
            |> Seq.map (fun ar -> Array.map (fun s -> s |> float) ar)

        Seq.map2 (fun name crd -> (name, crd)) lnNames lnCoords

    let getObjects (kmlPath: string) =
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        let kmlXml = new XmlDocument()
        kmlXml.Load kmlPath

        Seq.append (getPoints kmlXml.DocumentElement) (getLines kmlXml.DocumentElement)
