using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;

namespace MHR_CR
{
    class Ocr
    {
        private TextHelper textHelper = new TextHelper();

        private string rareWhitelist = "RARE1234567";
        private string skillWhitelist;
        private string levelWhitelist = "Lv123456";

        private Mat _m;
        private Mat _croped;
        public Dictionary<string, string> res = new Dictionary<string, string>
        {
            ["rare"] = "0",
            ["slot"] = "000",
            ["s1"] = "",
            ["l1"] = "",
            ["s2"] = "",
            ["l2"] = "",
        };

        public Ocr()
        {
            skillWhitelist = textHelper.GetSkillUniqueChar();
        }

        public void Proc(Mat m)
        {
            _m = m;
            Proc();
        }

        public void Proc(Bitmap img)
        {
            _m = BitmapConverter.ToMat(img);
            Proc();
        }

        public void Proc(string path)
        {
            _m = new Mat(path);
            Proc();
        }

        private bool IsArmorPage()
        {
            using (Mat background = new Mat(_m, new OpenCvSharp.Rect(980, 200, 100, 100)))
            {
                Cv2.MeanStdDev(background, out Scalar mean, out Scalar std);
                if (mean.Val0 < 10 && std.Val0 < 10)
                {
                    return true;
                }
                return false;
            }
        }

        private void Proc()
        {
            Cv2.CvtColor(_m, _m, ColorConversionCodes.BGR2GRAY);

            bool armorPage = IsArmorPage();
            if (armorPage)
            {
                _croped = new Mat(_m, new OpenCvSharp.Rect(1521, 268, 356, 279));
            }
            else
            {
                _croped = new Mat(_m, new OpenCvSharp.Rect(1127, 286, 356, 279));
            }

            _m.Dispose();
            Anlys();
        }

        private void Anlys()
        {
            Mat rare = 255 - new Mat(_croped, new OpenCvSharp.Rect(266, 0, 356 - 266, 32));
            Cv2.CopyMakeBorder(rare, rare, 5, 5, 5, 5, BorderTypes.Replicate);

            Mat slot = new Mat(_croped, new OpenCvSharp.Rect(228, 35, 42 * 3, 70 - 35));

            Mat skill1 = 255 - new Mat(_croped, new OpenCvSharp.Rect(30, 130, 220, 32));
            Cv2.CopyMakeBorder(skill1, skill1, 5, 5, 5, 5, BorderTypes.Replicate);
            Mat level1 = 255 - new Mat(_croped, new OpenCvSharp.Rect(285, 168, 356 - 285, 32));
            Cv2.CopyMakeBorder(level1, level1, 5, 5, 5, 5, BorderTypes.Replicate);

            Mat skill2 = 255 - new Mat(_croped, new OpenCvSharp.Rect(30, 207, 220, 32));
            Cv2.CopyMakeBorder(skill2, skill2, 5, 5, 5, 5, BorderTypes.Replicate);
            Mat level2 = 255 - new Mat(_croped, new OpenCvSharp.Rect(285, 245, 356 - 285, 32));
            Cv2.CopyMakeBorder(level2, level2, 5, 5, 5, 5, BorderTypes.Replicate);

            _croped.Dispose();

            Cv2.MinMaxIdx(skill2, out double m2, out _);

            if (m2 > 180)
            {
                res["rare"] = textHelper.LevelCorr(ApplyOcr(BitmapConverter.ToBitmap(rare), "en", rareWhitelist));
                rare.Dispose();
                res["slot"] = Proc_Slot(slot);
                slot.Dispose();
                res["s1"] = textHelper.SkillCorr(ApplyOcr(BitmapConverter.ToBitmap(skill1), "ch", skillWhitelist));
                skill1.Dispose();
                res["l1"] = textHelper.LevelCorr(ApplyOcr(BitmapConverter.ToBitmap(level1), "en", levelWhitelist));
                level1.Dispose();
                res["s2"] = "";
                skill2.Dispose();
                res["l2"] = "";
                level2.Dispose();
            }
            else
            {
                res["rare"] = textHelper.LevelCorr(ApplyOcr(BitmapConverter.ToBitmap(rare), "en", rareWhitelist));
                rare.Dispose();
                res["slot"] = Proc_Slot(slot);
                slot.Dispose();
                res["s1"] = textHelper.SkillCorr(ApplyOcr(BitmapConverter.ToBitmap(skill1), "ch", skillWhitelist));
                skill1.Dispose();
                res["l1"] = textHelper.LevelCorr(ApplyOcr(BitmapConverter.ToBitmap(level1), "en", levelWhitelist));
                level1.Dispose();
                res["s2"] = textHelper.SkillCorr(ApplyOcr(BitmapConverter.ToBitmap(skill2), "ch", skillWhitelist));
                skill2.Dispose();
                res["l2"] = textHelper.LevelCorr(ApplyOcr(BitmapConverter.ToBitmap(level2), "en", levelWhitelist));
                level2.Dispose();
            }

            //Cv2.ImShow("rare", rare);
            //Cv2.ImShow("slot", slot);
            //Cv2.ImShow("s1", skill1);
            //Cv2.ImShow("l1", level1);
            //Cv2.ImShow("s2", skill2);
            //Cv2.ImShow("l2", level2);

            //foreach (KeyValuePair<string, string> item in res)
            //{
            //    Console.WriteLine(item);
            //}
        }

        private string Proc_Slot(Mat slot)
        {
            int h = slot.Height;
            Mat slot1 = new Mat(slot, new OpenCvSharp.Rect(42 * 0, 0, 42, h));
            Mat slot2 = new Mat(slot, new OpenCvSharp.Rect(42 * 1, 0, 42, h));
            Mat slot3 = new Mat(slot, new OpenCvSharp.Rect(42 * 2, 0, 42, h));

            Cv2.Threshold(slot1, slot1, 80, 255, ThresholdTypes.Binary);
            Cv2.Threshold(slot2, slot2, 80, 255, ThresholdTypes.Binary);
            Cv2.Threshold(slot3, slot3, 80, 255, ThresholdTypes.Binary);

            //Cv2.ImShow("slot1", slot1);
            //Cv2.ImShow("slot2", slot2);
            //Cv2.ImShow("slot3", slot3);

            int s1 = (int)Cv2.Sum(slot1)[0];
            int s2 = (int)Cv2.Sum(slot2)[0];
            int s3 = (int)Cv2.Sum(slot3)[0];

            slot1.Dispose();
            slot2.Dispose();
            slot3.Dispose();

            int ss1 = Sub_Slot(s1);
            if (ss1 == 0)
            {
                return "000";
            }
            int ss2 = Math.Min(ss1, Sub_Slot(s2));
            if (ss2 == 0)
            {
                return "" + ss1 + "00";
            }
            int ss3 = Math.Min(ss2, Sub_Slot(s3));

            return "" + ss1 + ss2 + ss3;
        }

        private int Sub_Slot(int sum)
        {
            sum /= 10000;
            if (sum < 10)
            {
                return 0;
            }

            int d1 = Math.Abs(sum - 21);
            int d2 = Math.Abs(sum - 16);
            int d3 = Math.Abs(sum - 13);

            if (d1 < d2)
            {
                if (d1 < d3)
                {
                    return 1;
                }
                return 3;
            }
            if (d2 < d3)
            {
                return 2;
            }
            return 3;
        }


        private string ApplyOcr(Bitmap image, string lang, string whitelist)
        {
            TesseractEngine engine;
            switch (lang)
            {
                case "en":
                    engine = new TesseractEngine("tessdata", "eng", EngineMode.LstmOnly);
                    break;
                case "ch":
                default:
                    engine = new TesseractEngine("tessdata", "chi_sim", EngineMode.LstmOnly);
                    break;
            }
            engine.SetVariable("tessedit_char_whitelist", whitelist);
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
            Pix img = PixConverter.ToPix(image);
            Page page = engine.Process(img);
            return page.GetText().Trim();
        }
    }

    class TextHelper
    {
        // [...document.getElementsByTagName('select')[0].getElementsByTagName('option')].slice(1,-3).map(e=>e.textContent).join('\n');
        private string unique;
        private List<string> skillList = new List<string>();
        private HashSet<string> skillSet;
        private SymSpell symSpell = new SymSpell();

        public TextHelper(string path = "./Skills.txt")
        {
            HashSet<char> set = new HashSet<char>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    skillList.Add(line);
                    symSpell.CreateDictionaryEntry(line, 1);
                    foreach (char item in line)
                    {
                        set.Add(item);
                    }
                }
            }

            skillSet = new HashSet<string>(skillList);
            unique = string.Join("", set.ToArray());
        }

        public string GetSkillUniqueChar()
        {
            return unique;
        }

        public string SkillCorr(string s)
        {
            if (skillSet.Contains(s))
            {
                return s;
            }
            List<SymSpell.SuggestItem> sug = symSpell.Lookup(s, SymSpell.Verbosity.Closest);
            if (sug.Count > 0)
            {
                return sug[0].term;
            }
            return s;
        }

        public string LevelCorr(string s)
        {
            char last = s[s.Length - 1];
            if (last > '0' && last <= '9')
            {
                return last.ToString();
            }
            return "1";
        }
    }
}
