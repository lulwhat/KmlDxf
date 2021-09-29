namespace KmlDxf

open System
open FsXaml

type App = XAML<"App.xaml">

module Main =
    [<EntryPoint; STAThread>]
    let main argv =
        //App().Run()

        let app = App()
        let mainWindow = new MainWindow()
        app.Run(mainWindow)
