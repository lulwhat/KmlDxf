namespace KmlDxf

open FsXaml

type MainWindowXaml = XAML<"MainWindow.xaml">

type MainWindow() as this =
    inherit MainWindowXaml()

    let kmlBtnClick _ =
        this.Title <- "Change is good"

    do
        this.kmlButton.Click.Add kmlBtnClick