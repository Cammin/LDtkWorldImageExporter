using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using LDtkUnity;
using Microsoft.Win32;
using Utf8Json;

namespace WorldImageMerger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public partial class MainWindow
    {
        public string WorkingDirectory;
        
        #if DEBUG
        //public string ChosenPath = "C:/Users/cameo/Documents/_Personal/LDtkWorldImageMerger/Test/WorldMap_GridVania_layout.ldtk";
        #else
#endif
        public string ChosenPath;
        
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("Start app");

            UpdateExportButton();
            
            WorkingDirectory = Environment.CurrentDirectory;
            
            PickButton.Click += (sender, args) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = ChosenPath ?? WorkingDirectory;
                dialog.Title = "Pick LDtk project";
                dialog.Filter = "LDtk project (*.ldtk, *.json)|*.ldtk;*.json|All files (*.*)|*.*";
                
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    ChosenPath = dialog.FileName;
                    Path.Content = ChosenPath;
                    Console.WriteLine($"Picked. \"{ChosenPath}\"");
                }
                
                UpdateExportButton();
            };

            ExportButton.Click += (sender, args) =>
            {
                Console.WriteLine($"Export! \"{ChosenPath}\"");

                byte[] bytes = File.ReadAllBytes(ChosenPath);
                LdtkJson json = JsonSerializer.Deserialize<LdtkJson>(bytes);
                
                ImageMaker maker = new ImageMaker(ChosenPath);
                
                World[] worlds = GetWorlds(json);
                World worldToExport = worlds.FirstOrDefault();
                if (worldToExport == null)
                {
                    throw new Exception("Issue");
                }
                
                maker.ExportWorld(worldToExport);
            };
        }
        
        public World[] GetWorlds(LdtkJson json)
        {
            if (!json.Worlds.IsNullOrEmpty())
            {
                return json.Worlds;
            }
            
            return new World[] { new World 
            {
                Identifier = "World",
                Iid = json.DummyWorldIid,
                Levels = json.Levels,
                WorldLayout = json.WorldLayout.Value,
                WorldGridWidth = json.WorldGridWidth.Value,
                WorldGridHeight = json.WorldGridHeight.Value
            }};
        }
        

        private void UpdateExportButton()
        {
            ExportButton.IsEnabled = !string.IsNullOrEmpty(ChosenPath);
        }
    }
}
