﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities.JSON;
using Services.Entities.JSON.Analyse;
using Services.Entities.JSON.Describe;
using Services.Entities.JSON.Handwriting;
using Services.Entities.JSON.OCR;
using Services.Entities.JSON.Tags;
using Face = Services.Entities.JSON.Faces.Face;

namespace Services.Entities
{
    public class Image : TableEntity
    {
        public Image() { }

        public Image(Guid rowKey) : base(rowKey.ToString(), rowKey.ToString())
        {
            CreatedDate = DateTime.UtcNow;
        }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Uri { get; set; }

        public string ThumbUri { get; set; }

        public string FaceUri { get; set; }

        public string Description { get; set; }

        public Describe DeseralisedDescription => !string.IsNullOrEmpty(Description) ? JSONHelper.FromJson<Describe>(Description) : null;

        public string Analyse { get; set; }

        public Analyse DeseralisedAnalyse => !string.IsNullOrEmpty(Analyse) ? JSONHelper.FromJson<Analyse>(Analyse) : null;

        public string OCR { get; set; }

        public OCR DeseralisedOCR => !string.IsNullOrEmpty(OCR) ? JSONHelper.FromJson<OCR>(OCR) : null;

        public string Tag { get; set; }

        public Tags DeseralisedTag => !string.IsNullOrEmpty(Tag) ? JSONHelper.FromJson<Tags>(Tag) : null;

        public string Handwriting { get; set; }

        public Handwriting DeseralisedHandwriting => !string.IsNullOrEmpty(Handwriting) ? JSONHelper.FromJson<Handwriting>(Handwriting) : null;

        public string Faces { get; set; }

        public Face[] DeseralisedFaces => !string.IsNullOrEmpty(Faces) ? JSONHelper.FromJson<Face[]>(Faces) : null;

        public string ImageThumbnailUri
        {
            get
            {
                if (string.IsNullOrEmpty(ThumbUri) || DeseralisedAnalyse == null) return "/img/Processing.gif";

                if(DeseralisedAnalyse.Adult.IsAdultContent || DeseralisedAnalyse.Adult.IsRacyContent)
                {
                    return "/img/nsfw.jpg";
                }

                return ThumbUri;
            }
        }

        public string FullImageUri => !string.IsNullOrEmpty(FaceUri) ? FaceUri : Uri;
        public IQueryable<ImageComment> Comments { get; set; }
    }
}