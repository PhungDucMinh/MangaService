using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Model
{
    [DataContract]
    public class CoverImage : Image
    {
        public CoverImage(Uri imageUri)
        {
            this.ImageUri = imageUri;
        }
    }
}
