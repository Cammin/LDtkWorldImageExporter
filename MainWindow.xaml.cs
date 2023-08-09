using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace WorldImageMerger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public partial class MainWindow
    {
        public string WorkingDirectory;
        public WorldsMaker Maker;
        public string ChosenPath;

        public List<string> WorldIdentifierOptions;
        public string WorldIdentifier;
        
        public List<int> WorldDepthOptions;
        public int? WorldDepth;
        
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("Start app");
            UpdateEnableds();
            WorkingDirectory = Environment.CurrentDirectory;
            
            ButtonPickPath.Click += (sender, args) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = ChosenPath ?? WorkingDirectory;
                dialog.Title = "Pick LDtk project";
                dialog.Filter = "LDtk project (*.ldtk, *.json)|*.ldtk;*.json|All files (*.*)|*.*";
                
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    ChosenPath = dialog.FileName;
                    Console.WriteLine($"Picked. \"{ChosenPath}\"");
                    
                    LabelChosenPath.Content = ChosenPath;
                    Maker = new WorldsMaker(ChosenPath, LoadingBar);
                    Maker.Maker.OnExported += writePath =>
                    {
                        Console.WriteLine($"OnExported!");
                        LabelWritePath.Content = $"Saved to: {writePath}";
                    };
                    
                    WorldIdentifierOptions = Maker.Worlds.Select(p => p.Identifier).ToList();
                    WorldIdentifier = WorldIdentifierOptions.First();

                    ComboBoxWorldIdentifier.ItemsSource = WorldIdentifierOptions;
                    ComboBoxWorldIdentifier.SelectedValue = WorldIdentifier;
                }
                
                UpdateEnableds();
            };
            
            ComboBoxWorldIdentifier.SelectionChanged += (sender, args) =>
            {
                WorldIdentifier = (string)ComboBoxWorldIdentifier.SelectedValue;
                Console.WriteLine($"Picked WorldIdentifier \"{WorldIdentifier}\"");
                
                //when a world is picked, get that world and solve what world depth options there are
                World world = Maker.GetWorldByName(WorldIdentifier);
                WorldDepthOptions = world.Levels.Select(p => p.WorldDepth).Distinct().ToList();
                WorldDepth = WorldDepthOptions.First();
                
                ComboBoxWorldDepth.ItemsSource = WorldDepthOptions;
                ComboBoxWorldDepth.SelectedValue = WorldDepth;
                
                UpdateEnableds();
            };
            ComboBoxWorldDepth.SelectionChanged += (sender, args) =>
            {
                WorldDepth = (int)ComboBoxWorldDepth.SelectedValue;
                Console.WriteLine($"Picked WorldDepth \"{WorldDepth}\"");
                
                UpdateEnableds();
            };
            
            ButtonGenerate.Click += (sender, args) =>
            {
                if (Maker.Maker.IsNotBusy)
                {
                    Console.WriteLine($"Export! {WorldIdentifier}:{WorldDepth.Value} \"{ChosenPath}\"");
                    Maker.Export(WorldIdentifier, WorldDepth.Value);
                }

                UpdateEnableds();
            };
            ButtonOpenFolderPath.Click += (sender, args) =>
            {
                if (Directory.Exists(Maker.Maker.WriteDir))
                {
                    Process.Start(Maker.Maker.WriteDir);
                }
            };
        }
        
        private void UpdateEnableds()
        {
            bool chosePath = !string.IsNullOrEmpty(ChosenPath);
            bool choseWorld = chosePath && !WorldIdentifier.IsNullOrEmpty();
            bool choseDepth = choseWorld && WorldDepth != null;
            bool hasImagePath = Maker != null && Directory.Exists(Maker.Maker.WriteDir);
            
            LabelChosenPath.IsEnabled = true;
            ComboBoxWorldIdentifier.IsEnabled = chosePath;
            ComboBoxWorldDepth.IsEnabled = choseWorld;
            ButtonGenerate.IsEnabled = choseDepth;
            LoadingBar.IsEnabled = true;
            ButtonOpenFolderPath.IsEnabled = hasImagePath;

            if (Maker != null && choseDepth)
            {
                LabelWritePath.Content = $"Save to: {Maker.Maker.GetWritePath(WorldIdentifier, WorldDepth.Value)}";
            }
        }
    }
}
