using HtmlAgilityPack;
using MangaService.CustomException;
using MangaService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MangaService
{
    internal class TruyenTranhTuan : IService
    {

        #region Fields
        private DataExtracter _dataExtracter = new DataExtracter();

        #endregion
        #region Properties

        public override string DataServiceName
        {
            get
            {
                return "TruyenTranhTuan";
            }
        }

        #region Uri
        private Uri _allMangaPageUri = new Uri("http://truyentranhtuan.com/danh-sach-truyen");
        private Uri _mostPopularMangaPageUri = new Uri("http://truyentranhtuan.com/danh-sach-truyen/top/top-50");
        private Uri _latestUpdatedMangaPageUri = new Uri("http://truyentranhtuan.com/");
        #endregion

        #endregion

        #region Contructor

        #endregion

        #region Important Methods

        #region Don't Need Methods
        //public override async Task<List<Manga>> LoadMoreAllMangaAsync(int count)
        //{
        //    if (AllMangaUri == null)
        //    {
        //        AllMangaUri = await GetAllMangaUriFromPageAsync(_allMangaPageUri);
        //    }

        //    List<Task<Manga>> mangaTasks = new List<Task<Manga>>();
        //    try
        //    {
        //        List<Uri> loadMoreMangaUriList = AllMangaUri.GetRange(0, count);
        //        AllMangaUri.RemoveRange(0, count);

        //        foreach (var uri in loadMoreMangaUriList)
        //        {
        //            mangaTasks.Add(GetMangaAsync(uri, 0));
        //            Debug.WriteLine("{0}> " + uri, ++_prepareLoadedManga);
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException ex)
        //    {
        //        Debug.WriteLine("TruyenTranhTuan DataService Exception: Load more manga than exist." + ex.Message);
        //    }
        //    List<Manga> mangaList = new List<Manga>();
        //    while (mangaTasks.Count > 0)
        //    {
        //        try
        //        {
        //            var finishedTask = await Task.WhenAny(mangaTasks.ToArray());
        //            mangaTasks.Remove(finishedTask);

        //            Manga manga = finishedTask.Result;
        //            if (manga != null)
        //            {
        //                Debug.WriteLine(String.Format("{0}> " + manga.MangaInfo.MangaName, ++_loadedManga));
        //                mangaList.Add(manga);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
        //        }
        //    }

        //    return mangaList;
        //}
        //public override async Task<List<Manga>> LoadMoreLatestUpdatedMangaAsync(int count)
        //{
        //    throw new NotImplementedException();
        //}
        //public override async Task<List<Manga>> GetMoreMostPopularMangaAsync(int count)
        //{
        //    if (MostPopularMangaUri == null)
        //    {
        //        MostPopularMangaUri = await GetAllMangaUriFromPageAsync(_mostPopularMangaPageUri);
        //    }
        //    List<Task<Manga>> mangaTasks = new List<Task<Manga>>();
        //    try
        //    {
        //        List<Uri> loadMoreMangaUriList = MostPopularMangaUri.GetRange(0, count);
        //        MostPopularMangaUri.RemoveRange(0, count);

        //        foreach (var uri in loadMoreMangaUriList)
        //        {
        //            mangaTasks.Add(GetMangaAsync(uri, 0));
        //            Debug.WriteLine("{0}> " + uri, ++_prepareLoadedManga);
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException ex)
        //    {
        //        Debug.WriteLine("TruyenTranhTuan DataService Exception: Load more manga than exist." + ex.Message);
        //    }
        //    List<Manga> mangaList = new List<Manga>();
        //    while (mangaTasks.Count > 0)
        //    {
        //        try
        //        {
        //            var finishedTask = await Task.WhenAny(mangaTasks.ToArray());
        //            mangaTasks.Remove(finishedTask);

        //            Manga manga = finishedTask.Result;
        //            if (manga != null)
        //            {
        //                Debug.WriteLine(String.Format("{0}> " + manga.MangaInfo.MangaName, ++_loadedManga));
        //                mangaList.Add(manga);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("TruyenTranhTuan DataService Exception: " + ex.Message);
        //        }
        //    }

        //    return mangaList;
        //}
        #endregion

        public override async Task<List<Manga>> GetMangaByGenreAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<Chapter>> GetAllChaptersAsync(Uri mangaUri)
        {
            //HtmlWeb htmlWeb = new HtmlWeb();
            //var htmlDocument = await htmlWeb.LoadFromWebAsync(mangaUri.OriginalString);

            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(mangaUri.OriginalString);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            HtmlNode documentNode = htmlDocument.DocumentNode;
            HtmlNode mangaChapterNode = documentNode.Descendants("div")
                .Where(p => p.GetAttributeValue("id", "") == "manga-chapter").FirstOrDefault();

            var query = from chapter in mangaChapterNode.Descendants("span")
                        .Where(p => p.GetAttributeValue("class", "") == "chapter-name")
                        select chapter;

            var chapters = new List<Chapter>();
            foreach (var item in query)
            {
                chapters.Add(new Chapter
                {
                    ChapterName = item.Descendants("a").FirstOrDefault().InnerText,
                    ChapterUri = new Uri(item.Descendants("a").FirstOrDefault().GetAttributeValue("href", "")),
                });
            }
            return chapters;
        }

        public override async Task<List<Chapter>> LoadMoreChapterAsync(Uri mangaUri, uint count)
        {
            if (CurrentMangaUri == null || CurrentMangaUri != mangaUri)
            {
                CurrentMangaChapters = null;
                CurrentMangaChapters = await GetAllChaptersAsync(mangaUri);
            }

            CurrentMangaUri = mangaUri;

            List<Chapter> loadedChapters = null;
            try
            {
                if (CurrentMangaChapters.Count >= count)
                {
                    loadedChapters = CurrentMangaChapters.GetRange(0, (int)count);
                    CurrentMangaChapters.RemoveRange(0, (int)count);
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
        #endregion

        #region Methods

        public override async Task<List<ChapterImage>> LoadChapterImageAsync(Uri chapterUri)
        {
            if (chapterUri == null)
                throw new ArgumentNullException("chapterUri", "TruyenTranhTuan - LoadChapterImageAsync method.");

            List<ChapterImage> images = null;
            using (var webClient = new HttpClient())
            {
                var html = await webClient.GetStringAsync(chapterUri);
                var imagesUriStringList = await GetImagesUriStringAsync(html);

                images = new List<ChapterImage>();
                int index = 1;
                foreach (var uriString in imagesUriStringList)
                {
                    var MangaImage = new ChapterImage
                    {
                        ImageUri = new Uri(uriString),
                        ImageIndex = index++,
                    };
                    images.Add(MangaImage);
                }
            }
            return images;
        }

        protected async Task<List<string>> GetImagesUriStringAsync(string html)
        {
            if (html == null)
                throw new ArgumentNullException("html", "TruyenTranhTuan - LoadChapterImageAsync method.");

            #region Get Indexes
            int index1 = html.IndexOf("var slides_page_url_path");
            int index2 = html.IndexOf("[", index1);
            int index3 = html.IndexOf("\"];", index2);

            if (index3 == -1)
            {
                index1 = html.IndexOf("var slides_page_path");
                index2 = html.IndexOf("[", index1);
                index3 = html.IndexOf("\"];", index2);
            }
            #endregion

            string pieceOfHtml = string.Empty;
            try
            {
                pieceOfHtml = html.Substring(index2, index3 - index2);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new MangaOutOfSourceException("Can't get image from site.");
            }

            List<string> uriStringList = new List<string>();

            try
            {
                string[] uriStringArr = pieceOfHtml.Split(new string[] { "[\"", "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                var reponse = await GetResponse(new Uri(uriStringArr.FirstOrDefault()));
                reponse.EnsureSuccessStatusCode();
                uriStringList = uriStringArr.ToList();
                if (reponse.RequestMessage.RequestUri.Host == "truyentranhtuan.com")
                {
                    uriStringList.Sort();
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("GetImagesUriStringAsync methods - Split html Exception.");
            }
            catch (Exception)
            {

                List<String> alterUriStringList = new List<string>();
                foreach (var uriString in uriStringList)
                {
                    string[] twoUriString = uriString.Split(new string[] { "url=" }, StringSplitOptions.RemoveEmptyEntries);
                    alterUriStringList.Add(twoUriString[1]);
                }
                uriStringList.Clear();
                uriStringList = alterUriStringList.ToList();
                uriStringList.Sort();
                return uriStringList;
            }
            return uriStringList;
        }

        protected override async Task<Manga> GetMangaAsync(Uri mangaUri, int count)
        {
            Manga manga = null;
            MangaInfo mangaInfo = null;
            HtmlNode nameNode;
            HtmlNode authorNode;
            IEnumerable<HtmlNode> genreNodes;
            try
            {
                string originalUrl = mangaUri.OriginalString;
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync(mangaUri);
                string html = await response.Content.ReadAsStringAsync();

                //IBuffer buffer = await httpClient.GetBufferAsync(mangaUri);
                //var html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)buffer.Length - 1);

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                nameNode = htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault(p => p.GetAttributeValue("itemprop", "") == "name");
                var coverUri = htmlDocument.GetElementbyId("infor-box").ChildNodes[1].Element("img").GetAttributeValue("src", "");
                authorNode = htmlDocument.GetElementbyId("infor-box").Descendants("span").FirstOrDefault(p => p.GetAttributeValue("itemprop", "") == "name");
                string[] latestChapterAndStatus = GetLatestChapterAndStatus(htmlDocument.GetElementbyId("infor-box"));
                genreNodes = htmlDocument.GetElementbyId("infor-box").Descendants("a");
                HtmlNode mangaSummaryNode = htmlDocument.GetElementbyId("manga-summary");
                HtmlNode nodeSummary = null;
                if (mangaSummaryNode != null)
                {
                    try
                    {
                        nodeSummary = htmlDocument.GetElementbyId("manga-summary").ChildNodes[1];
                    }
                    catch (IndexOutOfRangeException indexOutOfRangeException)
                    {
                        Debug.WriteLine("Catch IndexOutOfRangeException " + indexOutOfRangeException.Message);
                        authorNode = htmlDocument.GetElementbyId("manga-summary").ChildNodes[0];
                    }

                }


                mangaInfo = new MangaInfo();
                mangaInfo.MangaUri = mangaUri;
                mangaInfo.GenreListName = new List<string>();
                if (genreNodes != null)
                    foreach (var nodeGenre in genreNodes)
                    {
                        if (nodeGenre.GetAttributeValue("itemprop", "") == "genre")
                            mangaInfo.GenreListName.Add(nodeGenre.InnerText.Trim());
                    }

                if (nodeSummary != null)
                    mangaInfo.Summary = nodeSummary.InnerText.Trim();
                if (nameNode != null)
                    mangaInfo.MangaName = nameNode.InnerText.Trim();
                if (authorNode != null)
                    mangaInfo.Author = authorNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(coverUri))
                    mangaInfo.CoverImage = new Uri(coverUri);
                mangaInfo.LatestChapter = latestChapterAndStatus[0];
                mangaInfo.Status = latestChapterAndStatus[1];

                manga = new Manga(mangaInfo);
            }
            catch (HtmlWebException)
            {
                if (count == 0)
                {
                    Debug.WriteLine("Reload: " + mangaUri.OriginalString);
                    return await GetMangaAsync(mangaUri, 1);
                }
            }
            catch (ArgumentOutOfRangeException argumentEx)
            {
                Debug.WriteLine("ArgumentException_objectName: " + argumentEx.ParamName);
            }
            catch (NullReferenceException nullReferenceEx)
            {
                Debug.WriteLine("NullReferenceException_objectName: " + nullReferenceEx.Source);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Catch Exception: " + ex.Message);

            }
            return manga;
        }

        #region Search Manga
        public List<Manga> SearchMangaAsync(string query)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region SupportMethods

        protected string[] GetLatestChapterAndStatus(HtmlNode inforNode)
        {
            string innerText = inforNode.Descendants("p").ElementAt(3).InnerText;
            string[] splitText = innerText.Split(new string[] { ":", "-" }, StringSplitOptions.RemoveEmptyEntries);
            return new string[] { splitText.ElementAt(1).Trim(), splitText.ElementAt(2).Trim() };
        }

        #endregion


        #region Get MangaUri Methods

        protected override async Task<List<Uri>> GetAllMangaUriAsync()
        {
            String html = String.Empty;
            var httpClient = new HttpClient();
            try
            {
                html = await httpClient.GetStringAsync(_allMangaPageUri);

                //IBuffer buffer = await httpClient.GetBufferAsync(_allMangaPageUri);
                //html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TruyenTranhTuanDataService Exception: " + ex.Message);
                throw;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var newChapterNode = doc.DocumentNode.Descendants("div").Where(p => p.GetAttributeValue("id", "") == "new-chapter").FirstOrDefault();
            var mangaFocusNodes = newChapterNode.Descendants("div").Where(p => p.GetAttributeValue("class", "") == "manga-focus");
            string uriString = "";
            List<Uri> mangaUriList = new List<Uri>();
            foreach (var node in mangaFocusNodes)
            {
                var mangaNode = node.Descendants("span")
                    .Where(p => (p.GetAttributeValue("class", "") == "manga") || (p.GetAttributeValue("class", "") == "manga easy-tooltip"))
                    .FirstOrDefault().Descendants("a").FirstOrDefault();

                uriString = mangaNode.GetAttributeValue("href", "");
                mangaUriList.Add(new Uri(uriString));
            }
            return mangaUriList;
        }

        protected override async Task<List<Uri>> GetMostPopularMangaUriAsync()
        {
            String html = String.Empty;
            var httpClient = new HttpClient();

            try
            {
                html = await httpClient.GetStringAsync(_mostPopularMangaPageUri);

                //IBuffer buffer = await httpClient.GetBufferAsync(_mostPopularMangaPageUri);
                //html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TruyenTranhTuanDataService Exception: " + ex.Message);
                throw;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var newChapterNode = doc.DocumentNode.Descendants("div").Where(p => p.GetAttributeValue("id", "") == "new-chapter").FirstOrDefault();
            var mangaFocusNodes = newChapterNode.Descendants("div").Where(p => p.GetAttributeValue("class", "") == "manga-focus");
            string uriString = "";
            List<Uri> mangaUriList = new List<Uri>();
            foreach (var node in mangaFocusNodes)
            {
                var mangaNode = node.Descendants("span")
                    .Where(p => (p.GetAttributeValue("class", "") == "manga") || (p.GetAttributeValue("class", "") == "manga easy-tooltip"))
                    .FirstOrDefault().Descendants("a").FirstOrDefault();

                uriString = mangaNode.GetAttributeValue("href", "");
                mangaUriList.Add(new Uri(uriString));
            }
            return mangaUriList;
        }

        protected override async Task<List<Uri>> GetLatestUpdatedMangaUriAsync()
        {
            String html = String.Empty;
            var httpClient = new HttpClient();
            try
            {
                html = await httpClient.GetStringAsync(_latestUpdatedMangaPageUri);

                //IBuffer buffer = await httpClient.GetBufferAsync(_latestUpdatedMangaPageUri);
                //html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TruyenTranhTuanDataService Exception: " + ex.Message);
                throw;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var newChapterNode = doc.DocumentNode.Descendants("div").Where(p => p.GetAttributeValue("id", "") == "new-chapter").FirstOrDefault();
            var mangaFocusNodes = newChapterNode.Descendants("div").Where(p => p.GetAttributeValue("class", "") == "manga-focus");
            string uriString = "";
            List<Uri> mangaUriList = new List<Uri>();
            foreach (var node in mangaFocusNodes)
            {
                var mangaNode = node.Descendants("span")
                    .Where(p => (p.GetAttributeValue("class", "") == "manga") || (p.GetAttributeValue("class", "") == "manga easy-tooltip"))
                    .FirstOrDefault().Descendants("a").FirstOrDefault();

                uriString = mangaNode.GetAttributeValue("href", "");
                mangaUriList.Add(new Uri(uriString));
            }
            return mangaUriList;
        }

        public override async Task<string> GetMangaNameAsync(Uri uri, int v)
        {
            string name = string.Empty;
            HtmlNode nameNode;
            try
            {
                string originalUrl = uri.OriginalString;
                HttpClient httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(uri);
                //IBuffer buffer = await httpClient.GetBufferAsync(uri);
                //var html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)buffer.Length - 1);

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                nameNode = htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault(p => p.GetAttributeValue("itemprop", "") == "name");

                if (nameNode != null)
                    name = nameNode.InnerText.Trim();

            }
            catch (HtmlWebException)
            {
                if (v == 0)
                {
                    Debug.WriteLine("Reload: " + uri.OriginalString);
                    return await GetMangaNameAsync(uri, 1);
                }
            }
            catch (ArgumentOutOfRangeException argumentEx)
            {
                Debug.WriteLine("ArgumentException_objectName: " + argumentEx.ParamName);
            }
            catch (NullReferenceException nullReferenceEx)
            {
                Debug.WriteLine("NullReferenceException_objectName: " + nullReferenceEx.Source);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Catch Exception: " + ex.Message);

            }
            return name;
        }

        /*
        public override async Task<List<Manga>> LoadAllMangaFastAsync()
        {
            String html = String.Empty;
            var httpClient = new HttpClient();
            try
            {
                html = await httpClient.GetStringAsync(_allMangaPageUri);
                //IBuffer buffer = await httpClient.GetBufferAsync(_allMangaPageUri);
                //html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TruyenTranhTuanDataService Exception: " + ex.Message);
                throw;
            }

            var fileService = new FileService(this);
            List<Manga> mangaList = _dataExtracter.ExtractMangaInfoList(html);
            try
            {
                List<Manga> mangaListFromFile = await fileService.GetAllManga();
                var exceptMangaEnumerable = mangaList.Except(mangaListFromFile, new MangaEqualityComparer());
                var exceptMangaList = exceptMangaEnumerable.ToList();
                var simpleTaskLoader = new SimpleTaskLoader<Manga, bool>();
                await simpleTaskLoader.DoListTask(exceptMangaList, AddMangaImage);
                mangaList.Clear();
                var mangaEnumerable = exceptMangaList.Union(mangaListFromFile, new MangaEqualityComparer());
                mangaList = mangaEnumerable.ToList();

            }
            catch (FileNotFoundException)
            {
                //Get Image and Add to File...
                var simpleTaskLoader = new SimpleTaskLoader<Manga, bool>();
                List<bool> successList = await simpleTaskLoader.DoListTask(mangaList, AddMangaImage);

            }
            catch(Exception ex)
            {

            }
            await fileService.SaveAllManga(mangaList);
            return mangaList;

        }
        */

        public override async Task<List<Manga>> LoadAllMangaFastAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<Uri> GetMangaCoverUri(Manga manga)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync(manga.MangaInfo.MangaUri);

                    //IBuffer buffer = await client.GetBufferAsync(manga.MangaInfo.MangaUri);
                    //string html = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));

                    var uri = _dataExtracter.ExtractMangaCoverImage(html);
                    return uri;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<bool> AddMangaImage(Manga manga)
        {
            var imageUri = await this.GetMangaCoverUri(manga);
            if (imageUri != null)
            {
                manga.MangaInfo.CoverImage = imageUri;
                return true;
            }
            return false;
        }

        #endregion


    }

    class DataExtracter
    {
        private Manga ExtractMangaInfo(HtmlNode mangaFocus)
        {
            var mangaNode = mangaFocus.Descendants("span")
                .Where(p => (p.GetAttributeValue("class", "") == "manga") || (p.GetAttributeValue("class", "") == "manga easy-tooltip"))
                .FirstOrDefault().Descendants("a").FirstOrDefault();
            string name = mangaNode.InnerText.Trim();
            var uri = new Uri(mangaNode.GetAttributeValue("href", ""));

            var chapterNode = mangaFocus.Descendants("span")
                .Where(p => (p.GetAttributeValue("class", "") == "chapter"))
                .FirstOrDefault().Descendants("a").FirstOrDefault();
            string latestChapter = chapterNode.InnerText.Trim();

            var currentDateNode = mangaFocus.Descendants("span")
                .Where(p => (p.GetAttributeValue("class", "") == "current-date"))
                .FirstOrDefault();

            string currentDate = currentDateNode.InnerText.Trim();
            DateTime latestUpdatedDate = DateTime.Now;
            //DateTime.TryParse(currentDate, out latestUpdatedDate,);
            //System.Globalization.DateTimeStyles style = new System.Globalization.DateTimeStyles();
            //style.
            var mangaInfo = new MangaInfo
            {
                MangaName = name,
                MangaUri = uri,
                LatestChapter = latestChapter,
                LatestUpdatedDate = latestUpdatedDate,
            };
            var manga = new Manga(mangaInfo);
            return manga;

        }




        internal List<Manga> ExtractMangaInfoList(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var newChapterNode = doc.DocumentNode.Descendants("div").Where(p => p.GetAttributeValue("id", "") == "new-chapter").FirstOrDefault();
            var mangaFocusNodeList = newChapterNode.Descendants("div").Where(p => p.GetAttributeValue("class", "") == "manga-focus");
            string uriString = string.Empty;
            List<Manga> mangas = new List<Manga>();
            foreach (var mangaFocus in mangaFocusNodeList)
            {
                var manga = ExtractMangaInfo(mangaFocus);
                mangas.Add(manga);
            }
            return mangas;
        }

        internal Uri ExtractMangaCoverImage(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            Uri coverUri = null;
            var uriString = doc.GetElementbyId("infor-box").ChildNodes[1].Element("img").GetAttributeValue("src", "");
            if (uriString != string.Empty)
                coverUri = new Uri(uriString);
            return coverUri;
        }
    }

    class FileService
    {
        private IService _dataService;
        private string _allMangaFileName = "all_manga_list.xml";

        public FileService(IService dataService)
        {
            this._dataService = dataService;
        }

        public async Task<Uri> GetMangaInfo(Manga manga)
        {

            throw new NotImplementedException();
        }

        /*
        public async Task<List<Manga>> GetAllManga()
        {
            var serviceFolder = await GetServiceFolder();
            SerializationService serializer = new SerializationService();

            var listManga = await serializer.Deserialize(serviceFolder, _allMangaFileName, typeof(List<Manga>)) as List<Manga>;
            return listManga;

        }

        private async Task<StorageFolder> GetServiceFolder()
        {
            var dataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            var serviceFolder = await dataFolder.CreateFolderAsync(_dataService.DataServiceName, CreationCollisionOption.OpenIfExists);
            return serviceFolder;


        }

        public async Task SaveAllManga(List<Manga> mangaList)
        {
            var serviceFolder = await GetServiceFolder();
            SerializationService serializer = new SerializationService();
            await serializer.Serialize(serviceFolder, _allMangaFileName, mangaList, typeof(List<Manga>));
        }
        */
    }

}
