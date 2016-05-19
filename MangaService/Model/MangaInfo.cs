using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Model
{
    [DataContract]
    public class MangaInfo
    {
        [DataMember]
        public string MangaName { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string LatestChapter { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public List<string> GenreListName { get; set; }
        
        [DataMember]
        public Uri CoverImage { get; set; }

        [DataMember]
        public Uri MangaUri { get; set; }

        [DataMember]
        public DateTime LatestUpdatedDate { get; set; }

        [DataMember]
        public int Rating { get; set; }

        [DataMember]
        public int MaxRating { get; set; }
    }
}
