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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TruyenTranhTuanService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TruyenTranhTuanService.svc or TruyenTranhTuanService.svc.cs at the Solution Explorer and start debugging.
    public class TruyenTranhTuanMangaService : IMangaService
    {
        #region Fields
        private IService _dataService;


        #endregion

        #region Properties

        public IService DataService
        {
            get
            {
                return _dataService;
            }

            set
            {
                _dataService = value;
            }
        }

        #endregion

        #region Constructor

        public TruyenTranhTuanMangaService(IService dataService)
        {
            DataService = dataService;
        }

        public TruyenTranhTuanMangaService()
        {
            DataService = new TruyenTranhTuan();
        }

        public async Task<List<Manga>> LoadMoreMangaFastAsync(Category category, int count)
        {
            if (category == Category.All)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaFastAsync(category, count);
                return loadedManga;
            }
            else if (category == Category.MostPopular)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaFastAsync(category, count);
                return loadedManga;
            }
            else if (category == Category.LatestUpdated)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaFastAsync(category, count);
                return loadedManga;
            }
            return null;
        }



        #endregion


        #region Important Methods
        public async Task<List<Manga>> LoadMoreMangaAsync(Category category, int count)
        {
            if (category == Category.All)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaAsync(category, count);
                return loadedManga;
            }
            else if (category == Category.MostPopular)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaAsync(category, count);
                return loadedManga;
            }
            else if (category == Category.LatestUpdated)
            {
                List<Manga> loadedManga = await DataService.LoadMoreMangaAsync(category, count);
                return loadedManga;
            }
            return null;
        }

        public List<Manga> GetAllManga()
        {
            return DataService.AllManga;
        }

        public async Task<Uri> GetMangaImageUri(Manga manga)
        {
            return await DataService.GetMangaCoverUri(manga);
        }

        public async Task<List<Chapter>> LoadMoreChapterAsync(Uri mangaUri, uint count)
        {
            return await DataService.LoadMoreChapterAsync(mangaUri, count);
        }

        /*
        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga)
        {
            return await _dataService.DownloadMangaWithLocalFolderAsync(manga);
        }

        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga, List<Chapter> chapters)
        {
            return await _dataService.DownloadMangaWithLocalFolderAsync(manga, chapters);
        }

        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga)
        {
            return await _dataService.DownloadMangaWithSpecifyFolderAsync(manga);
        }

        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga, List<Chapter> chapters)
        {
            return await _dataService.DownloadMangaWithSpecifyFolderAsync(manga, chapters);
        }

        */

        #endregion

    }
}
