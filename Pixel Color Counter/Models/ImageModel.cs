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
        public Hashtable tempColors = new Hashtable();
        public Hashtable currColors = new Hashtable();

        public SortedDictionary<string, int> dict = new SortedDictionary<string, int>();

        public bool Simplify { get; set; }
        
        public int Threshold { get; set; }

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

                    if (tempColors.ContainsKey(pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()))
                    {
                        //increment value
                        tempColors[pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()] = (int)tempColors[pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString()] + 1;
                    }
                    else
                    {
                        tempColors.Add(pxlRed.ToString() + "," + pxlGreen.ToString() + "," + pxlBlue.ToString(), colorcounter);
                        colorcounter++;
                    }
                }
            }

            //try to combine similar colors
            if (Simplify)
            {
                SimplifyColors();
            }
            else
            {
                currColors = tempColors;
            }

            //more slowdowns while i make another loop, i want to get as many similar colors near each other so i can visually combine at least
            foreach (DictionaryEntry d in currColors)
            {
                dict.Add(d.Key.ToString(), (int)d.Value);
            }
        }

        public void SimplifyColors()
        {
            foreach (DictionaryEntry de in tempColors)
            {
                //get initial color
                int red = int.Parse(de.Key.ToString().Split(',')[0]);
                int green = int.Parse(de.Key.ToString().Split(',')[1]);
                int blue = int.Parse(de.Key.ToString().Split(',')[2]);

                bool addnew = false;
                string combinekey = "";

                foreach (DictionaryEntry c in currColors)
                {
                    int cred = int.Parse(c.Key.ToString().Split(',')[0]);
                    int cgreen = int.Parse(c.Key.ToString().Split(',')[1]);
                    int cblue = int.Parse(c.Key.ToString().Split(',')[2]);

                    //this gets them into buckets if the rgb values are the same +- the threshold but not if the colors are the same looking even though the values vary. 
                    //eg 15,15,15 buckets with 20,20,20 but 0,0,100 does not bucket with 0,0,150) both being pretty dark blue
                    if ((red - Threshold <= cred && red + Threshold >= cred) && (green - Threshold <= cgreen && green + Threshold >= cgreen) && (blue - Threshold <= cblue && blue + Threshold >= cblue))
                    {
                        combinekey = c.Key.ToString();
                        addnew = false;
                        break;
                    }
                    else
                    {
                        //if the difference between rg, gb, rb values is little then test again and combine if needed
                        //if the difference between rgb and current is less than 50 then retest against the remaining individually
                        //eg 0,100, 200 vs 21, 100,200 - failed above check because just one value was out of range
                        //50 chosen arbitrarily because i can barely tell 0,0,150 from 0,0,100 and the palette of colors i need in the end is as simple as possible
                        //still won't catch some but good enough to reduce a bit more
                        if (Math.Abs(red - cred) < 50)
                        {
                            if ((green - Threshold <= cgreen && green + Threshold >= cgreen) && (blue - Threshold <= cblue && blue + Threshold >= cblue))
                            {
                                combinekey = c.Key.ToString();
                                addnew = false;
                                break;
                            }
                        }
                        else if (Math.Abs(green - cgreen) < 50)
                        {
                            if ((red - Threshold <= cred && red + Threshold >= cred) && (blue - Threshold <= cblue && blue + Threshold >= cblue))
                            {
                                combinekey = c.Key.ToString();
                                addnew = false;
                                break;
                            }
                        }
                        else if (Math.Abs(blue - cblue) < 50)
                        {
                            if ((red - Threshold <= cred && red + Threshold >= cred) && (green - Threshold <= cgreen && green + Threshold >= cgreen))
                            {
                                combinekey = c.Key.ToString();
                                addnew = false;
                                break;
                            }
                        }


                        addnew = true;
                    }
                }

                if (addnew)
                {
                    currColors.Add(de.Key.ToString(), de.Value);
                }
                else if (addnew == false && combinekey != "")
                {
                    currColors[combinekey] = (int)currColors[combinekey] + (int)de.Value;
                }
                else
                {
                    currColors.Add(de.Key.ToString(), de.Value);
                }

            }
        }
    }
}