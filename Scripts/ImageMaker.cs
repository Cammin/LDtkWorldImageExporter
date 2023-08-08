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
            //check if there's no world. if not, make a dummy world
            
            
            //var worlds = GetWorlds()
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
            
            

            //
            
            //Bitmap lvlImg = 
            
            Rectangle rect = new Rectangle
            {
                X = levels.Min(p => p.WorldX),
                Y = levels.Max(p => p.WorldY),
                Width = levels.Max(lvl => lvl.WorldX + lvl.PxWid),
                Height = levels.Max(lvl => lvl.WorldY + lvl.PxHei)
            };

            Bitmap map = new Bitmap(rect.Width - rect.X, rect.Height - rect.Y);
        }

        public Image LoadLevelImage(Level lvl)
        {
            string path = Path.Combine(ProjectPath, ProjectName, "png") + lvl.Identifier + ".png";
            return Image.FromFile(path);
        }
    }
}