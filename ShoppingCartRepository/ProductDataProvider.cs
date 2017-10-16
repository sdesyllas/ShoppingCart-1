using CsvHelper;
using Microsoft.Extensions.FileProviders;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShoppingCart.Repository
{
    public class ProductDataProvider : IDataProvider<Product>
    {
        private readonly IFileProvider fileProvider;
        private readonly string dataFileName;

        public ProductDataProvider(IFileProvider fileProvider, string dataFileName)
        {
            this.fileProvider = fileProvider;
            this.dataFileName = dataFileName;
        }

        public IEnumerable<Product> Provide()
        {
            var file = fileProvider.GetFileInfo(dataFileName);
            using (FileStream stream = (FileStream)file.CreateReadStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(reader))
                    {
                        csvReader.Configuration.HasHeaderRecord = true;
                        csvReader.Configuration.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                        return csvReader.GetRecords<Product>().ToList();
                    }
                }
            }
        }
    }
}
