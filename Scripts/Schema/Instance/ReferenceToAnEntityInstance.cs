﻿using System.Runtime.Serialization;

namespace WorldImageMerger
{
    /// <summary>
    /// This object describes the "location" of an Entity instance in the project worlds.
    /// </summary>
    public partial class ReferenceToAnEntityInstance
    {
        /// <summary>
        /// IID of the refered EntityInstance
        /// </summary>
        [DataMember(Name = "entityIid")]
        public string EntityIid { get; set; }

        /// <summary>
        /// IID of the LayerInstance containing the refered EntityInstance
        /// </summary>
        [DataMember(Name = "layerIid")]
        public string LayerIid { get; set; }

        /// <summary>
        /// IID of the Level containing the refered EntityInstance
        /// </summary>
        [DataMember(Name = "levelIid")]
        public string LevelIid { get; set; }

        /// <summary>
        /// IID of the World containing the refered EntityInstance
        /// </summary>
        [DataMember(Name = "worldIid")]
        public string WorldIid { get; set; }
    }
}