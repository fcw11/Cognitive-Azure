using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Entities
{
    public class ImageComment : TableEntity
    {
        public Guid ImageId { get; set; }

        public string Comment { get; set; }

        public double Score { get; set; }


        public ImageComment()
        {
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
