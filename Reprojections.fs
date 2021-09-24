namespace KmlDxf

open DotSpatial.Projections

module Reprojections = 
    let crsWgs = ProjectionInfo.FromEpsgCode(4326)
    let crsTarget projString = ProjectionInfo.FromProj4String(projString)



    0 |> ignore