using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Entities
{
    public class ImageComment : TableEntity
    {
        public Guid ImageId { get; set; }

        public string Comment { get; set; }

        public double Score { get; set; }

        public string Image
        {
            get
            {
                var score = Score * 100;

                return score > 80
                    ? "/img/emoji/Great.png"
                    : (score > 60
                        ? "/img/emoji/Happy.png"
                        : (score > 40
                            ? "/img/emoji/Neutral.png"
                            : (score > 20
                                ? "/img/emoji/Bad.png"
                                : "/img/emoji/Terrible.png")));
            }
        }

        public string Phrases { get; set; }


        public ImageComment()
        {
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
