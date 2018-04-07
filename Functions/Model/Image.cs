using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Funcs.Model
{
    public class Image : TableEntity
    {
        public Image() { }

        public Image(string name, Guid rowKey) : base(rowKey.ToString(), rowKey.ToString())
        {
            CreatedDate = DateTime.UtcNow;

            Name = name;
        }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Uri { get; set; }

        public string ThumbUri { get; set; }

        public string Description { get; set; }

        public string Analyse { get; set; }

        public string OCR { get; set; }

        public string Tag { get; set; }

        public string Handwriting { get; set; }
    }
}
