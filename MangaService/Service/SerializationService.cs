using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MangaLight.Service
{
    public class SerializationService
    {
        /*
        public async Task<object> Deserialize(IStorageFolder folder, string documentName, Type dataType)
        {
            try
            {
                var documentFile = await folder.GetFileAsync(documentName);

                if (documentFile != null)
                {
                    using (var stream = await documentFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (var inputStream = stream.AsStreamForRead())
                        {
                            var serializer = new DataContractSerializer(dataType);
                            return serializer.ReadObject(inputStream);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Serialize data to specify file 
        /// </summary>
        /// <param name="folder">The folder that create the xml file</param>
        /// <param name="documentName">Xml document file name</param>
        /// <param name="graph">the object that contain the data want to write</param>
        public async Task Serialize(IStorageFolder folder, string documentName, object graph, Type type)
        {
            #region Check parameter
            if (folder == null || documentName == null || graph == null || type
                 == null)
            {
                throw new ArgumentNullException("Serialize method. One or more arguments is null");
            }
            #endregion

            StorageFile file = await folder
                .CreateFileAsync(documentName, CreationCollisionOption.ReplaceExisting);


            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (var outputStream = stream.AsStreamForWrite())
                {
                    var serializer = new DataContractSerializer(type);
                    serializer.WriteObject(outputStream, graph);
                }
            }
        }
        */
    }
}
