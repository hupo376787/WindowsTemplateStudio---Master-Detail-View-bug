using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Xaml.Media;

namespace ExifInfo.Models
{
    public class Product
    {
        public string StoreId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Price { get; set; }

        public string ProductImage { get; set; }

    }
}
