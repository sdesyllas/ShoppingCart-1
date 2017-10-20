using CsvHelper;
using Microsoft.Extensions.FileProviders;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class ProductDataProvider : IDataProvider<Product>
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _dataFileName;

        public ProductDataProvider(IFileProvider fileProvider, string dataFileName)
        {
            this._fileProvider = fileProvider;
            this._dataFileName = dataFileName;
        }

        public async Task<IEnumerable<Product>> ProvideAsync()
        {
            var file = _fileProvider.GetDirectoryContents("App_data").First(x => x.Name == _dataFileName);
            using (FileStream stream = (FileStream)file.CreateReadStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(reader))
                    {
                        csvReader.Configuration.HasHeaderRecord = true;
                        csvReader.Configuration.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                        return await Task.Run(() => csvReader.GetRecords<Product>().ToList());
                    }
                }
            }
        }
    }
}
