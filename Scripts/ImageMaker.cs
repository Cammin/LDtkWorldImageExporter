using System.Drawing;
using System.IO;
using System.Linq;
using LDtkUnity;

namespace WorldImageMerger
{
    public class ImageMaker
    {
        public string ProjectPath;
        public string ProjectName;

        public ImageMaker(string projectPath)
        {
            ProjectPath = projectPath;
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


            string writePath = Path.GetDirectoryName(ProjectPath) + '/' + world.Identifier + ".png";
            
            map.Save(writePath);
        }

        public Bitmap LoadLevelImage(Level lvl)
        {
            string path = Path.Combine(ProjectPath, ProjectName, "png") + lvl.Identifier + ".png";
            return new Bitmap(path);
        }
    }
}