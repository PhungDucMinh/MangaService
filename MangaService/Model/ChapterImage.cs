using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Model
{
    [DataContract]
    public class ChapterImage : Image
    {
        [DataMember]
        public int ImageIndex { get; set; }
        [DataMember]
        public bool IsDownloaded { get; set; }
    }
}
