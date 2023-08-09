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
        public int WorldDepth;
        public ProgressBar Bar;
        public BackgroundWorker Worker = new BackgroundWorker();

        public WorldMaker(string projectPath, int worldDepth, ProgressBar progressBar)
        {
            WorldDepth = worldDepth;
            
            ProjectPath = projectPath;
            ProjectDir = Path.GetDirectoryName(projectPath);
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
            Bar = progressBar;
        }
        
        public void ExportWorldWithProgress(World world)
        {
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += (sender, args) =>
            {
                ExportWorld(world);
                /*for(int i = 0; i < 100; i++)
                {
                    (sender as BackgroundWorker)?.ReportProgress(i);
                    Thread.Sleep(100);
                }*/
            };
            
            Bar.Minimum = 0;
            Bar.Maximum = 100;
            Bar.Value = 0;
            
            Worker.ProgressChanged += (sender, args) =>
            {
                Bar.Value = args.ProgressPercentage;
            };
            Worker.RunWorkerAsync();
        }
        
        public void ExportWorld(World world)
        {
            Level[] levels = world.Levels.Where(p => p.WorldDepth == WorldDepth).ToArray();

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

            string fileName = $"{world.Identifier}_{WorldDepth}.png";
            string writeDir = Path.Combine(ProjectDir, ProjectName, "world");
            string writePath = Path.Combine(writeDir, fileName);
            
            Console.WriteLine($"Writing world image to {writePath}");
            Directory.CreateDirectory(writeDir);
            worldImg.Save(writePath, ImageFormat.Png);
            Console.WriteLine($"Wrote!");
            Worker.ReportProgress(100);
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