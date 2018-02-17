using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Entities
{
    public class Image : TableEntity
    {
        public Image(string name, Guid rowKey) : base(rowKey.ToString(), rowKey.ToString())
        {
            CreatedDate = DateTime.UtcNow;

            Name = name;
        }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Uri { get; set; }
    }
}
