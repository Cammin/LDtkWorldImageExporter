using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using LDtkUnity;

namespace WorldImageMerger
{
    public class ImageMaker
    {
        public string ProjectPath;
        public string ProjectDir;
        public string ProjectName;
        public int WorldDepth;

        public ImageMaker(string projectPath, int worldDepth = 0)
        {
            WorldDepth = worldDepth;
            
            ProjectPath = projectPath;
            ProjectDir = Path.GetDirectoryName(projectPath);
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
        }
        
        public void ExportWorld(World world)
        {
            Level[] levels = world.Levels.Where(p => p.WorldDepth == WorldDepth).ToArray();
            
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
            foreach (Level lvl in levels)
            {
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