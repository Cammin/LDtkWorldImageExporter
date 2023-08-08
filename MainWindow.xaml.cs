using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        
        public string ChosenPath;
        
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("Start app");

            ExportButton.IsEnabled = false;
            
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
                
                ExportButton.IsEnabled = !string.IsNullOrEmpty(ChosenPath);
            };

            ExportButton.Click += (sender, args) =>
            {
                Console.WriteLine($"Export! \"{ChosenPath}\"");

                byte[] bytes = File.ReadAllBytes(ChosenPath);
                LdtkJson json = JsonSerializer.Deserialize<LdtkJson>(bytes);
                
                ImageMaker maker = new ImageMaker(ChosenPath);
                maker.MakeTheImage(json);
            };
        }
    }
}
