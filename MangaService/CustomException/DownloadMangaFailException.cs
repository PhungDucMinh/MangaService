using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.CustomException
{
    public class DownloadMangaFailException : Exception
    {
        public string MangaSource { get; set; }
        public string MangaName { get; set; }
        public DownloadMangaFailException(string message) : base(message)
        {
        }

        public DownloadMangaFailException(string mangaSource, string mangaName, string message) : base(message)
        {
            MangaSource = mangaSource;
            MangaName = mangaName;
        }
    }
}
