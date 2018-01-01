using System;

namespace ExifInfo.Models
{
    public class SharedDataWebLinkModel : SharedDataModelBase
    {
        public Uri Uri { get; set; }

        public SharedDataWebLinkModel()
            : base()
        {
        }
    }
}
