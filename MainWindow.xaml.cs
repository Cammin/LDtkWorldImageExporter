using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
                
                
            };
        }
    }
}
