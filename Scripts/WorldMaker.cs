using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using LDtkUnity;

namespace WorldImageMerger
{
    public class WorldMaker
    {
        public string ProjectPath;
        public string ProjectDir;
        public string ProjectName;

        public string WriteDir;
        
        public ProgressBar Bar;
        public BackgroundWorker Worker;
        
        public event Action<string> OnExported;
        

        public WorldMaker(string projectPath, ProgressBar progressBar)
        {
            ProjectPath = projectPath;
            ProjectDir = Path.GetDirectoryName(projectPath);
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
            WriteDir = Path.Combine(ProjectDir, ProjectName, "world");
            Bar = progressBar;
        }

        public bool IsNotBusy => Worker == null || !Worker.IsBusy;

        public void ExportWorldWithProgress(World world, int worldDepth)
        {
            Bar.Minimum = 0;
            Bar.Maximum = 100;
            Bar.Value = 0;

            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += (sender, args) =>
            {
                ExportWorld(world, worldDepth);
            };
            Worker.ProgressChanged += (sender, args) =>
            {
                Bar.Value = args.ProgressPercentage;
            };
            Worker.RunWorkerAsync();
        }
        
        public void ExportWorld(World world, int worldDepth)
        {
            Level[] levels = world.Levels.Where(p => p.WorldDepth == worldDepth).ToArray();

            Worker.ReportProgress(0);
            
            Rectangle worldRect = new Rectangle
            {
                X = levels.Min(lvl => lvl.WorldX),
                Y = levels.Min(lvl => lvl.WorldY),
            };
            worldRect.Width = levels.Max(lvl => lvl.WorldX + lvl.PxWid) - worldRect.X;
            worldRect.Height = levels.Max(lvl => lvl.WorldY + lvl.PxHei) - worldRect.Y;
            
            foreach (Level lvl in levels)
            {
                lvl.WorldX -= worldRect.Location.X;
                lvl.WorldY -= worldRect.Location.Y;
            }
            
            Bitmap worldImg = new Bitmap(worldRect.Width, worldRect.Height);
            for (int i = 0; i < levels.Length; i++)
            {
                double fraction = i/(double)levels.Length;
                Worker.ReportProgress((int)Math.Round(fraction * 100));
                
                Level lvl = levels[i];
                Console.WriteLine($"Adding lvl {lvl.Identifier}");
                Bitmap lvlImg = LoadLevelImage(lvl);
                Point worldPos = new Point(lvl.WorldX, lvl.WorldY);

                for (int x = 0; x < lvlImg.Width; x++)
                {
                    for (int y = 0; y < lvlImg.Height; y++)
                    {
                        Color px = lvlImg.GetPixel(x, y);
                        worldImg.SetPixel(worldPos.X + x, worldPos.Y + y, px);
                    }
                }
            }

            string writePath = GetWritePath(world.Identifier, worldDepth);
            
            Directory.CreateDirectory(WriteDir);
            worldImg.Save(writePath, ImageFormat.Png);
            Console.WriteLine($"Wrote world image to {writePath}");
            Worker.ReportProgress(100);
            OnExported?.Invoke(writePath);
        }
        
        public string GetWritePath(string worldName, int worldDepth)
        {
            return Path.Combine(WriteDir, $"{worldName}_{worldDepth}.png");
        }

        public Bitmap LoadLevelImage(Level lvl)
        {
            string path = Path.Combine(ProjectDir, ProjectName, "png", lvl.Identifier + ".png");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Didn't find image file at {path}");
            }
            return new Bitmap(path);
        }
    }
}