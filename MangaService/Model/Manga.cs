using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Model
{
    [DataContract]
    public class Manga
    {
        #region Constructors

        public Manga()
        {

        }

        public Manga(MangaInfo mangaInfo)
        {
            MangaInfo = mangaInfo;
            ChapterList = null;
        }

        #endregion

        [DataMember]
        public MangaInfo MangaInfo { get; set; }

        [DataMember]
        public List<Chapter> ChapterList { get; set; }
    }
}
