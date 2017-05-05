using Pixel_Color_Counter.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Pixel_Color_Counter.Controllers
{
    public class ImageController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult GetImageForm()
        {
            ImageModel im = (ImageModel)TempData["img"] ?? new ImageModel();

            return PartialView("ImageView", im);
        }

        [NotChildAction]
        public ActionResult SubmitImage(ImageModel m)
        {
            //validate image is a bitmap
            //can't control the file size but if it's more than say...100k pixels then kick back
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                HttpPostedFileBase fuImg = Request.Files[0];

                if (!(fuImg.ContentType == "image/bmp" || fuImg.ContentType == "image/jpeg" || fuImg.ContentType == "image/gif" || fuImg.ContentType == "image/png"))
                {
                    ModelState.AddModelError("IncorrectFileType", "The file type did not pass.");
                }

                //put a max of 1m pixels to keep it from being too ridiculous
                Bitmap img = new Bitmap(fuImg.InputStream);

                if (img.Width > 1000 || img.Height > 1000)
                {
                    ModelState.AddModelError("TooMuch", "The image had too many pixels. 1 million tops.");
                }
                
            }
            else
            {
                ModelState.AddModelError("WTF", "No file submitted.");
            }

            

            if (ModelState.IsValid)
            {
                HttpPostedFileBase fuImg = Request.Files[0];

                ImageModel im = new ImageModel();
                im.Simplify = m.Simplify;
                im.Threshold = (m.Threshold > 255) ? 255 : m.Threshold;
                im.SecondThreshold = (m.SecondThreshold > 255) ? 255: (m.SecondThreshold < 50) ? 50 : m.SecondThreshold;

                im.ProcessImage(fuImg);

                TempData["img"] = im;
            }

            return CurrentUmbracoPage();

        }
    }
}