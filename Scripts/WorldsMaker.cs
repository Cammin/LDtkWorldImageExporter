using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using LDtkUnity;
using Utf8Json;

namespace WorldImageMerger
{
    public class WorldsMaker
    {
        public string ProjectPath;
        public LdtkJson Json;
        public ProgressBar Bar;
        public World[] Worlds;
        
        public WorldsMaker(string projectPath, ProgressBar progressBar)
        {
            ProjectPath = projectPath;
            byte[] bytes = File.ReadAllBytes(ProjectPath);
            Json = JsonSerializer.Deserialize<LdtkJson>(bytes);
            Worlds = GetWorlds();
            Bar = progressBar;
        }
        
        public void Export(string worldIdentifier, int worldDepth)
        {
            World worldToExport = GetWorldByName(worldIdentifier);
            WorldMaker maker = new WorldMaker(ProjectPath, worldDepth, Bar);
            maker.ExportWorldWithProgress(worldToExport);
        }

        public World GetWorldByName(string identifier)
        {
            World world = Worlds.FirstOrDefault(p => p.Identifier == identifier);
            if (world == null)
            {
                throw new Exception("Issue");
            }
            return world;
        }

        public World[] GetWorlds()
        {
            if (!Json.Worlds.IsNullOrEmpty())
            {
                return Json.Worlds;
            }
            
            return new World[] { new World 
            {
                Identifier = "World",
                Iid = Json.DummyWorldIid,
                Levels = Json.Levels,
                WorldLayout = Json.WorldLayout.Value,
                WorldGridWidth = Json.WorldGridWidth.Value,
                WorldGridHeight = Json.WorldGridHeight.Value
            }};
        }
    }
}