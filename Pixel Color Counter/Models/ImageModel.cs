using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Pixel_Color_Counter.Models
{
    public class ImageModel
    {
        public Hashtable currColors = new Hashtable();

        public ImageModel() { }
        
        public void ProcessImage(HttpPostedFileBase file)
        {
            Bitmap img = new Bitmap(file.InputStream);
            int colorcounter = 0;

            //the slow part. go through each pixel, one at a time...
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color pxlColor = img.GetPixel(x, y);

                    //using this get an approximation so that there aren't all 16+ million combinations
                    int pxlRed = pxlColor.R;
                    int pxlBlue = pxlColor.B;
                    int pxlGreen = pxlColor.G;

                    if (currColors.ContainsKey(pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()))
                    {
                        //increment value
                        currColors[pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()] = (int)currColors[pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()] + 1;
                    }
                    else
                    {
                        currColors.Add(pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString(), colorcounter);
                        colorcounter++;                        
                    }
                }
            }            
        }
    }
}