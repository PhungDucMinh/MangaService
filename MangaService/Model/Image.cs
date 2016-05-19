using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Model
{
    [DataContract]
    public abstract class Image
    {
        [DataMember]
        public Uri ImageUri { get; set; }
    }
}
