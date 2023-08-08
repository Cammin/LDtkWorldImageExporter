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
                Identifier = ProjectName,
                Iid = json.DummyWorldIid,
                Levels = json.Levels,
                WorldLayout = json.WorldLayout.Value,
                WorldGridWidth = json.WorldGridWidth.Value,
                WorldGridHeight = json.WorldGridHeight.Value
            }};
        }

        public void CreateWorldImage(World world)
        {
            Console.WriteLine($"Creating world {world.Identifier} ({world.WorldGridWidth},{world.WorldGridHeight})");
            
            Level[] levels = world.Levels.Where(p => p.WorldDepth == WorldDepth).ToArray();
            
            Rectangle worldRect = new Rectangle
            {
                X = levels.Min(lvl => lvl.WorldX),
                Y = levels.Min(lvl => lvl.WorldY),
            };
            worldRect.Width = levels.Max(lvl => lvl.WorldX + lvl.PxWid) - worldRect.X;
            worldRect.Height = levels.Max(lvl => lvl.WorldY + lvl.PxHei) - worldRect.Y;
            
            Console.WriteLine($"World: {worldRect}");


            Point worldTopLeft = worldRect.Location;
            
            Point worldBottomRight = new Point(
                levels.Min(lvl => lvl.WorldX), 
                levels.Min(lvl => lvl.WorldY));
            
            
            
            
            //expect -1024, -256
            Console.WriteLine($"worldTopLeft ({worldTopLeft.X},{worldTopLeft.Y})");
            foreach (Level lvl in levels)
            {
                Console.WriteLine($"worldPos {lvl.Identifier} ({lvl.WorldX},{lvl.WorldY})");
                lvl.WorldX -= worldTopLeft.X;
                lvl.WorldY -= worldTopLeft.Y;
            }



            Bitmap map = new Bitmap(worldRect.Width, worldRect.Height);
            
            
            
            foreach (Level lvl in levels)
            {
                Bitmap image = LoadLevelImage(lvl);
                
                
                Point worldPos = new Point(lvl.WorldX, lvl.WorldY);
                
               // Console.WriteLine($"worldPos ({lvl.WorldX},{lvl.WorldY})");
                //Console.WriteLine($"worldPos ({worldPos.X},{worldPos.Y})");
                
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color px = image.GetPixel(x, y);

                        
                        //Console.WriteLine($"Setting px of {lvl.Identifier} from src ({x},{y}) from worldPos ({lvl.WorldX},{lvl.WorldY}) for actual: ({pointOnMap.X},{pointOnMap.Y})");
                        
                        map.SetPixel(worldPos.X + x, worldPos.Y + y, px);
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
            string path = Path.Combine(ProjectDir, ProjectName, "png", lvl.Identifier + ".png");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Didn't find image file at {path}");
            }
            Console.WriteLine($"Reading world image {path}");
            return new Bitmap(path);
        }
    }
}