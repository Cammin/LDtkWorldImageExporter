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

        public ImageMaker(string projectPath)
        {
            ProjectPath = projectPath;
            ProjectDir = Path.GetDirectoryName(projectPath);
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
        }
        
        public void MakeTheImage(LdtkJson json)
        {
            World[] worlds = GetWorlds(json);

            World first = worlds.First();
            CreateWorldImage(first);
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

        public void CreateWorldImage(World world)
        {
            Console.WriteLine($"Creating world {world.Identifier}");
            
            Level[] levels = world.Levels;
            
            Rectangle worldRect = new Rectangle
            {
                X = levels.Min(lvl => lvl.WorldX),
                Y = levels.Max(lvl => lvl.WorldY),
                Width = levels.Max(lvl => lvl.WorldX + lvl.PxWid),
                Height = levels.Max(lvl => lvl.WorldY + lvl.PxHei)
            };
            Point relOffset = worldRect.Location;
            relOffset.X = -relOffset.X;
            relOffset.Y = -relOffset.Y;

            
            Bitmap map = new Bitmap(worldRect.Width, worldRect.Height);

            foreach (Level lvl in levels)
            {
                Bitmap image = LoadLevelImage(lvl);
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color px = image.GetPixel(x, y);
                        map.SetPixel(lvl.WorldX + relOffset.X, lvl.WorldY + relOffset.Y, px);
                    }
                }
            }
            //
            
            //Bitmap lvlImg = 


            string writePath = ProjectDir + '/' + world.Identifier + ".png";
            
            Console.WriteLine($"Writing world image to {writePath}");
            map.Save(writePath, ImageFormat.Png);
        }

        public Bitmap LoadLevelImage(Level lvl)
        {
            string path = Path.Combine(ProjectPath, ProjectName, "png") + lvl.Identifier + ".png";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Didn't find image file at {path}");
            }
            Console.WriteLine($"Reading world image {path}");
            return new Bitmap(path);
        }
    }
}