using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.CustomException
{
    public class MangaOutOfSourceException : Exception
    {
        public string SourceName { get; set; }
        public string MangaName { get; set; }
        public MangaOutOfSourceException(string message) : base(message)
        {
        }

        public MangaOutOfSourceException(string sourceName ,string mangaName, string message) : base(message)
        {
            SourceName = sourceName;
            MangaName = mangaName;
        }
    }
}
