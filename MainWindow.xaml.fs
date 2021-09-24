namespace KmlDxf

open FsXaml
open System.Xml
open System
open System.Windows
open Microsoft.Win32

type MainWindowXaml = XAML<"MainWindow.xaml">

type MainWindow() as this =
    inherit MainWindowXaml()

    let getCsNames (root: XmlNode) =
        root.SelectNodes "/systems/system"
        |> Seq.cast<XmlNode>
        //|> Seq.filter (fun node -> node.Attributes.GetNamedItem("receiver").Value = "вега")
        |> Seq.map
            (fun node ->
                node.Attributes.GetNamedItem("name").InnerText
                + ": "
                + node.Attributes.GetNamedItem("folder").InnerText)

    let kmlBtnClick _ =
        let openFileDialog = new OpenFileDialog()
        openFileDialog.Title <- "Выберите kml"
        openFileDialog.Filter <- "Файл меток Google Maps (*.kml)|*.kml|Все файлы(*.*)|*.*"
        openFileDialog.InitialDirectory <- Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        if openFileDialog.ShowDialog() = Nullable(true) then
            this.kmlTextBox.Text <- openFileDialog.FileName

    let dxfButtonClick _ =
        let saveFileDialog = new SaveFileDialog()
        saveFileDialog.Title <- "Сохранить dxf как"
        saveFileDialog.Filter <- "Файлы чертежей dxf (*.dxf)|*.dxf|Все файлы(*.*)|*.*"
        saveFileDialog.InitialDirectory <- Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        if saveFileDialog.ShowDialog() = Nullable(true) then
            this.dxfTextBox.Text <- saveFileDialog.FileName

    let exportButtonClick _ =
        this.proressBar.Value <- 0.

        MessageBox.Show("Экспорт завершен", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Information)
        |> ignore

        this.proressBar.Value <- 100.


    do
        this.kmlButton.Click.Add kmlBtnClick
        this.dxfButton.Click.Add dxfButtonClick
        this.exportButton.Click.Add exportButtonClick

        let gsXml = new XmlDocument()
        gsXml.Load "geosystems.xml"
        let root = gsXml.DocumentElement

        getCsNames root
        |> Seq.iter (fun line -> this.csComboBox.Items.Add(line) |> ignore)
