
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using MangaService;
using MangaService.Model;

namespace MangaService
{
    public enum AvailableMangaSource
    {
        TruyenTranhTuan,
        MangaFox,
    }

    public abstract class IService 
    {
        #region Fields
        private int _loadedManga = 0;

        private int _prepareLoadedManga = 0;

        private int _imageDownloadStartIndex = 0;

        private int _startLoadedAllMangaIndex = 0;

        private string _imageDownloadStartName = "image_";

        private List<Uri> _allMangaUri;

        private List<Uri> _mostPopularMangaUri;

        private List<Uri> _latestUpdateMangaUri;

        private List<Manga> _allManga = new List<Manga>();

        private List<Manga> _mostMopularManga = new List<Manga>();

        private List<Manga> _latestUpdatedManga = new List<Manga>();

        private List<Chapter> _currentMangaChapters = new List<Chapter>();

        private Uri _currentMangaUri;

        private static HttpClient _client = new HttpClient();
        #endregion

        #region Properties

        protected List<Uri> AllMangaUri
        {
            get { return _allMangaUri; }
            set { _allMangaUri = value; }
        }

        protected List<Uri> MostPopularMangaUri
        {
            get { return _mostPopularMangaUri; }
            set { _mostPopularMangaUri = value; }
        }

        protected List<Uri> LatestUpdatedMangaUri
        {
            get { return _latestUpdateMangaUri; }
            set { _latestUpdateMangaUri = value; }
        }

        public List<Manga> AllManga
        {
            get { return _allManga; }
            set { _allManga = value; }
        }

        public List<Manga> MostPopularManga
        {
            get { return _mostMopularManga; }
            set { _mostMopularManga = value; }
        }


        public List<Manga> LatestUpdatedManga
        {
            get { return _latestUpdatedManga; }
            set { _latestUpdatedManga = value; }
        }

        protected List<Chapter> CurrentMangaChapters
        {
            get { return _currentMangaChapters; }
            set { _currentMangaChapters = value; }
        }

        protected Uri CurrentMangaUri
        {
            get { return _currentMangaUri; }
            set { _currentMangaUri = value; }
        }

        //protected IStorageFolder DownloadFolder
        //{
        //    get { return _downloadFolder; }
        //    private set { _downloadFolder = value; }
        //}

        public abstract string DataServiceName { get; }
        #endregion

        static public async Task<HttpResponseMessage> GetResponse(Uri requestUri)
        {

            var message = await _client.GetAsync(requestUri);
            return message;

        }

        public abstract Task<List<Manga>> LoadAllMangaFastAsync();

        #region Load Manga

        public virtual async Task<List<Manga>> LoadMoreMangaAsync(Category category, int count)
        {
            List<Uri> uriList = null;
            if (category == Category.All)
            {
                if (AllMangaUri == null)
                {
                    AllMangaUri = await GetAllMangaUriAsync();
                }
                uriList = AllMangaUri;
            }
            else if (category == Category.MostPopular)
            {
                if (MostPopularMangaUri == null)
                {
                    MostPopularMangaUri = await GetMostPopularMangaUriAsync();
                }
                uriList = MostPopularMangaUri;
            }
            else if (category == Category.LatestUpdated)
            {
                if (LatestUpdatedMangaUri == null)
                {
                    LatestUpdatedMangaUri = await GetLatestUpdatedMangaUriAsync();
                }
                uriList = LatestUpdatedMangaUri;
            }

            if (uriList.Count == 0)
            {
                return null;
            }

            List<Task<Manga>> mangaTasks = new List<Task<Manga>>();
            List<Uri> loadMoreMangaUriList = null;
            try
            {
                if (uriList.Count >= count)
                {
                    loadMoreMangaUriList = uriList.GetRange(0, count);
                    uriList.RemoveRange(0, count);
                }
                else
                {
                    loadMoreMangaUriList = uriList.GetRange(0, uriList.Count);
                    uriList.RemoveRange(0, uriList.Count);
                }

                foreach (var uri in loadMoreMangaUriList)
                {
                    mangaTasks.Add(GetMangaAsync(uri, 0));
                    Debug.WriteLine("{0}> " + uri, ++_prepareLoadedManga);
                }
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
            }

            List<Manga> mangaList = new List<Manga>();
            while (mangaTasks.Count > 0)
            {
                try
                {
                    var finishedTask = await Task.WhenAny(mangaTasks.ToArray());
                    mangaTasks.Remove(finishedTask);

                    Manga manga = finishedTask.Result;
                    if (manga != null)
                    {
                        Debug.WriteLine(String.Format("{0}> " + manga.MangaInfo.MangaName, ++_loadedManga));
                        mangaList.Add(manga);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
                }
            }

            return mangaList;
        }

        public virtual async Task<List<Manga>> LoadMoreMangaFastAsync(Category category, int count)
        {

            if (category == Category.All)
            {
                if (AllMangaUri == null)
                {
                    AllManga = await LoadAllMangaFastAsync();
                }
            }
            List<Manga> mangaList = null;
            if (AllManga.Count - 1 - _startLoadedAllMangaIndex > count)
                mangaList = AllManga.GetRange(_startLoadedAllMangaIndex, count);
            else
                mangaList = AllManga.GetRange(_startLoadedAllMangaIndex, AllManga.Count - 1);

            _startLoadedAllMangaIndex += mangaList.Count;
            return mangaList;
        }

        public async Task<List<string>> GetAllMangaNameAsync()
        {
            if (AllMangaUri == null)
            {
                AllMangaUri = await GetAllMangaUriAsync();
            }
            var mangaTasks = new List<Task<string>>();
            foreach (var uri in AllMangaUri)
            {
                mangaTasks.Add(GetMangaNameAsync(uri, 0));
                Debug.WriteLine("{0}> " + uri, ++_prepareLoadedManga);
            }

            var mangaNameList = new List<string>();
            while (mangaTasks.Count > 0)
            {
                try
                {
                    var finishedTask = await Task.WhenAny(mangaTasks.ToArray());
                    mangaTasks.Remove(finishedTask);

                    string name = finishedTask.Result;
                    if (name != null)
                    {
                        Debug.WriteLine(String.Format(name));
                        mangaNameList.Add(name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
                }
            }

            return mangaNameList;
        }

        public abstract Task<string> GetMangaNameAsync(Uri uri, int v);

        public virtual async Task<List<Chapter>> LoadMoreChapterAsync(Uri mangaUri, uint expectedCount)
        {
            if (CurrentMangaUri == null || CurrentMangaUri != mangaUri)
            {
                CurrentMangaChapters = null;
                CurrentMangaChapters = await GetAllChaptersAsync(mangaUri);
            }
            CurrentMangaUri = mangaUri;

            //Get loadedChapter depend on how many CurrentChapter left.
            List<Chapter> loadedChapters = null;
            try
            {
                if (CurrentMangaChapters.Count >= expectedCount)
                {
                    loadedChapters = CurrentMangaChapters.GetRange(0, (int)expectedCount);
                    CurrentMangaChapters.RemoveRange(0, (int)expectedCount);
                }
                else
                {
                    loadedChapters = CurrentMangaChapters.GetRange(0, CurrentMangaChapters.Count);
                    CurrentMangaChapters.RemoveRange(0, CurrentMangaChapters.Count);
                }
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine("TruyenTranhTuanDataService ArgumentException catch, Message: " + ex.Message);
            }
            catch (Exception)
            {
                throw;
            }

            return loadedChapters;
        }

        public abstract Task<List<Manga>> GetMangaByGenreAsync();

        public abstract Task<List<ChapterImage>> LoadChapterImageAsync(Uri chapterUri);

        public abstract Task<List<Chapter>> GetAllChaptersAsync(Uri mangaUri);

        protected abstract Task<Manga> GetMangaAsync(Uri mangaUri, int count);

        public abstract Task<Uri> GetMangaCoverUri(Manga manga);
        #region Get Manga Uri 
        protected abstract Task<List<Uri>> GetAllMangaUriAsync();

        protected abstract Task<List<Uri>> GetMostPopularMangaUriAsync();

        protected abstract Task<List<Uri>> GetLatestUpdatedMangaUriAsync();
        #endregion

        #endregion

        #region Download

        /*
        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga)
        {
            var download = new DownloadManga(this);
            return await download.DownloadMangaWithLocalFolderAsync(manga);
        }

        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga, List<Chapter> chapters)
        {
            var download = new DownloadManga(this);
            return await download.DownloadMangaWithLocalFolderAsync(manga, chapters);
        }

        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga)
        {
            var download = new DownloadManga(this);
            return await download.DownloadMangaWithSpecifyFolderAsync(manga);
        }

        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga, List<Chapter> chapters)
        {
            var download = new DownloadManga(this);
            return await download.DownloadMangaWithSpecifyFolderAsync(manga, chapters);
        }
        */
        #endregion
        

    }

    class DownloadManga
    {
        #region Fields

        private IService _service;

        private int _imageDownloadStartIndex = 0;

        private string _imageDownloadStartName = "image_";

        #endregion

        public DownloadManga(IService service)
        {
            _service = service;
        }

        #region Methods

        /*
        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga)
        {
            var dataServiceDownloadFolder = await GetDataServiceDownloadFolderAsync();
            bool isSucceed = await DownloadMangaAsync(dataServiceDownloadFolder, manga);
            try
            {
                SerializationService serializer = new SerializationService();
                serializer.Serialize(dataServiceDownloadFolder, manga.MangaInfo.MangaName + ".xml", manga, typeof(Manga));
            }
            catch (ArgumentNullException)
            {
                throw new DownloadMangaFailException(_service.DataServiceName, manga.MangaInfo.MangaName, "Serialize manga fail.");
            }
            return isSucceed;

        }

        public async Task<bool> DownloadMangaWithLocalFolderAsync(Manga manga, List<Chapter> chapters)
        {
            var dataServiceDownloadFolder = await GetDataServiceDownloadFolderAsync();
            bool isSucceed = await DownloadMangaAsync(dataServiceDownloadFolder, manga, chapters);
            try
            {
                SerializationService serializer = new SerializationService();
                serializer.Serialize(dataServiceDownloadFolder, manga.MangaInfo.MangaName + ".xml", manga, typeof(Manga));
            }
            catch (ArgumentNullException)
            {
                throw new DownloadMangaFailException(_service.DataServiceName, manga.MangaInfo.MangaName, "Serialize manga fail.");
            }
            return isSucceed;
        }

        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            var chosenFolder = await picker.PickSingleFolderAsync();
            if (chosenFolder != null)
            {
                return await DownloadMangaAsync(chosenFolder, manga);
            }
            return false;
        }
        
        public async Task<bool> DownloadMangaWithSpecifyFolderAsync(Manga manga, List<Chapter> chapters)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            var chosenFolder = await picker.PickSingleFolderAsync();
            if (chosenFolder != null)
            {
                return await DownloadMangaAsync(chosenFolder, manga, chapters);
            }
            return false;
        }
        */
        /*
        private async Task<bool> DownloadMangaAsync(IStorageFolder downloadFolder, Manga manga)
        {
            IStorageFolder mangaFolder = await downloadFolder.CreateFolderAsync(manga.MangaInfo.MangaName, CreationCollisionOption.OpenIfExists);
            manga.ChapterList = await _service.GetAllChaptersAsync(manga.MangaInfo.MangaUri);
            foreach (var chapter in manga.ChapterList)
            {
                bool isSucess = false;
                try
                {
                    isSucess = await DownloadChapterAsync(mangaFolder, chapter);
                    
                }
                catch (ArgumentNullException ex)
                {
                    Debug.WriteLine(ex.Message);
                    isSucess = false;
                }
                chapter.IsDownloaded = isSucess;
                Debug.WriteLine("Download Chapter: " + chapter.ChapterName + " " + isSucess.ToString());
            }

            return true;
        }

        private async Task<bool> DownloadMangaAsync(IStorageFolder downloadFolder, Manga manga, List<Chapter> chapters)
        {
            IStorageFolder mangaFolder = await downloadFolder.CreateFolderAsync(manga.MangaInfo.MangaName, CreationCollisionOption.OpenIfExists);
            manga.ChapterList = chapters;
            foreach (var chapter in manga.ChapterList)
            {
                bool isSucess = false;
                try
                {
                    isSucess = await DownloadChapterAsync(mangaFolder, chapter);

                }
                catch (ArgumentNullException ex)
                {
                    Debug.WriteLine(ex.Message);
                    isSucess = false;
                }
                chapter.IsDownloaded = isSucess;
                Debug.WriteLine("Download Chapter: " + chapter.ChapterName + " " + isSucess.ToString());
            }

            return true;
        }

        private async Task<bool> DownloadChapterAsync(IStorageFolder mangaFolder, Chapter chapter)
        {
            #region Check parameters
            if (chapter == null || mangaFolder == null)
            {
                string parameterName = string.Empty;
                if (chapter == null)
                {
                    parameterName = "chapter";
                }
                else if (mangaFolder == null)
                {
                    parameterName = "folder";
                }
                throw new ArgumentNullException(parameterName, "Can't not download chapter " + chapter.ChapterName);
            }
            #endregion

            try
            {
                var chapterFolder = await mangaFolder.CreateFolderAsync(chapter.ChapterName, CreationCollisionOption.OpenIfExists);
                var chapterImages = await _service.LoadChapterImageAsync(chapter.ChapterUri);
                var imageBuffers = new List<IBuffer>();
                using (HttpClient client = new HttpClient())
                {
                    int index = _imageDownloadStartIndex;

                    foreach (var image in chapterImages)
                    {
                        string desiredFileName = _imageDownloadStartName + index.ToString() + ".jpg";
                        bool isSuceed = false;
                        try
                        {
                            isSuceed = await DownloadImageAsync(chapterFolder, desiredFileName, image.ImageUri);                            
                        }
                        catch (ArgumentException ex)
                        {
                            Debug.WriteLine("DownloadChapterAsync method - " + chapter.ChapterName + ". " + ex.Message);
                            isSuceed = false;
                        }
                        image.IsDownloaded = isSuceed;

                        index++;
                    }
                }
            }
            //catch(MangaOutOfSourceException)
            //{
            //    throw;
            //}
            catch (Exception ex)
            {
                Debug.WriteLine("IDataService - DownloadChapterAsync method - " + chapter.ChapterName + " error: ", ex.Message);
                return false;
            }

            return true;
        }

        private async Task<bool> DownloadImageAsync(IStorageFolder folder, string desiredName, Uri imageUri)
        {
            #region Check parameter
            if (folder == null || desiredName == string.Empty || imageUri == null)
            {
                throw new ArgumentException(_service.DataServiceName + " - DownloadImageAsync methods.");
            }
            #endregion

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    IBuffer buffer = await client.GetBufferAsync(imageUri);
                    IStorageFile file = await folder.CreateFileAsync(desiredName, CreationCollisionOption.ReplaceExisting);

                    try
                    {
                        await FileIO.WriteBufferAsync(file, buffer);
                    }
                    catch (Exception)
                    {
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadImageAsync method - " + desiredName + " error: ", ex.Message);
                return false;
            }

            return true;
        }

        private async Task<IStorageFolder> GetDataServiceDownloadFolderAsync()
        {
            IStorageFolder downloadFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Download", CreationCollisionOption.OpenIfExists);
            IStorageFolder dataServiceFolder = await downloadFolder.CreateFolderAsync(_service.DataServiceName, CreationCollisionOption.OpenIfExists);
            return dataServiceFolder;
        }
        */
        #endregion
    }


}
