
using MangaService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MangaService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITruyenTranhTuanService" in both code and config file together.
    [ServiceContract]
    public interface IMangaService
    {
        [OperationContract]
        Task<List<Manga>> LoadMoreMangaFastAsync(Category category, int count);

        [OperationContract]
        Task<List<Manga>> LoadMoreMangaAsync(Category category, int count);
    }
}
