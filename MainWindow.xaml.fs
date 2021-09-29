namespace KmlDxf

open FsXaml
open System.Xml
open System
open System.Windows
open Microsoft.Win32
open System.IO

type MainWindowXaml = XAML<"MainWindow.xaml">

type MainWindow() as this =
    inherit MainWindowXaml()

    let getCsNames (root: XmlNode) =
        root.SelectNodes "/systems/system"
        |> Seq.cast<XmlNode>
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

        let gsXml = new XmlDocument()
        gsXml.Load "geosystems.xml"
        let root = gsXml.DocumentElement

        let findChosenCsNode =
            root.SelectNodes "/systems/system"
            |> Seq.cast<XmlNode>
            |> Seq.filter
                (fun node -> node.Attributes.GetNamedItem("name").InnerText = this.csComboBox.Text.Split(':').[0])
        
        // catch errors and run the export
        (File.Exists this.kmlTextBox.Text,
         (Seq.length findChosenCsNode) >= 1,
         Directory.Exists(Path.GetDirectoryName this.dxfTextBox.Text),
         (Path.GetExtension this.dxfTextBox.Text) = ".dxf")
        |> function
            | (false, _, _, _) ->
                MessageBox.Show("KML файл не найден", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
                |> ignore
            | (_, false, _, _) ->
                MessageBox.Show("Неверное имя СК", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
                |> ignore
            | (_, _, false, _) ->
                MessageBox.Show("Некорректный путь файла DXF", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
                |> ignore
            | (_, _, _, false) ->
                MessageBox.Show("Неверное расширение файла DXF", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
                |> ignore
            | _ ->
                try
                    let projStringNode =
                        (Seq.item 0 findChosenCsNode).ChildNodes
                        |> Seq.cast<XmlNode>
                        |> Seq.filter (fun node -> node.Attributes.GetNamedItem("receiver").InnerText = "вега")
                        |> Seq.item 0

                    KmlReader.getObjects this.kmlTextBox.Text
                    |> Reprojections.reproject projStringNode.InnerText
                    |> DxfWriter.writeDxf this.dxfTextBox.Text

                    MessageBox.Show("Экспорт завершен", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Information)
                    |> ignore

                    this.proressBar.Value <- 100.
                with
                | _ -> MessageBox.Show("Неопределённая ошибка приложения", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
                       |> ignore


    do
        this.kmlButton.Click.Add kmlBtnClick
        this.dxfButton.Click.Add dxfButtonClick
        this.exportButton.Click.Add exportButtonClick

        // add coordinate systems to combobox
        if File.Exists "geosystems.xml" then
            let gsXml = new XmlDocument()
            gsXml.Load "geosystems.xml"

            getCsNames gsXml.DocumentElement
            |> Seq.iter (fun line -> this.csComboBox.Items.Add(line) |> ignore)
        else
            MessageBox.Show("Файл СК geosystems.xml не найден", "KML в DXF", MessageBoxButton.OK, MessageBoxImage.Error)
            |> ignore
