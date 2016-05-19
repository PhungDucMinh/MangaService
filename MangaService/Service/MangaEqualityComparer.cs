
using MangaService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaService.Service
{
    public class MangaEqualityComparer : IEqualityComparer<Manga>
    {
        public bool Equals(Manga x, Manga y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.MangaInfo.MangaName == y.MangaInfo.MangaName)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Manga obj)
        {
            return 0;
        }
    }
}
