using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace MCA
{
    internal class Program
    {
        private static string FormatPrice(float price) => "$" + price.ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));

        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://interview-task-api.mca.dev/")
            };

            var products = (await httpClient.
                GetFromJsonAsync<List<Product>>($"qr-scanner-codes/alpha-qr-gFpwhsQ8fkY1"));

            if (products == null || !products.Any())
            {
                return;
            }

            products = products.OrderBy(x => x.Name).ToList();

            var productDictionary = products.GroupBy(x => x.Domestic)
                .ToDictionary(x => x.Key, x =>
                new
                {
                    Products = x.OrderBy(x => x.Name).ToList(),
                    Cost = x.Sum(x => x.Price)
                });

            var domesticProductGroup = productDictionary[true];
            var importedProductGroup = productDictionary[false];

            Console.WriteLine(".Domestic");
            PrintProducts(domesticProductGroup.Products);
            Console.WriteLine(".Imported");
            PrintProducts(importedProductGroup.Products);

            Console.WriteLine($"Domestic cost: {FormatPrice(domesticProductGroup.Cost)}");
            Console.WriteLine($"Imported cost: {FormatPrice(importedProductGroup.Cost)}");

            Console.WriteLine($"Domestic count: {domesticProductGroup.Products.Count}");
            Console.WriteLine($"Imported count: {importedProductGroup.Products.Count}");
        }

        private static void PrintProducts(List<Product> products)
        {
            string spaceSeparator = "   ";

            foreach (var product in products)
            {
                Console.WriteLine($"...{product.Name}");

                Console.WriteLine($"{spaceSeparator}{FormatPrice(product.Price)}");

                var descriptionToPrint = product.Description.Length < 10 ? product.Description : product.Description.Substring(0, 10) + "...";
                Console.WriteLine($"{spaceSeparator}{descriptionToPrint}");

                var weightToPrint = product.Weight.HasValue ? product.Weight.Value.ToString() : "N/A";
                Console.WriteLine($"{spaceSeparator}{weightToPrint}");
            }
        }
    }
}