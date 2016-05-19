using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaService.Test.TTTService;

namespace MangaService.Test
{
    [TestClass]
    public class TruyenTranhTuanTest
    {
        MangaServiceClient client = new MangaServiceClient();

        [TestMethod]
        public void LoadSomeManga()
        {
            var task = client.LoadMoreMangaAsync(Category.LatestUpdated, 5);
            var mangas = task.Result;
            Assert.IsTrue(mangas.Count() == 5);
        }
    }
}
