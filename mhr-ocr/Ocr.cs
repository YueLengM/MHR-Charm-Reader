using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;

namespace mhr_ocr
{
    class Ocr
    {
        private Mat _m;
        public Dictionary<string, string> res = new Dictionary<string, string>
        {
            ["rare"] = "0",
            ["slot"] = "000",
            ["s1"] = "",
            ["l1"] = "",
            ["s2"] = "",
            ["l2"] = "",
        };

        public Ocr(Bitmap bitmap)
        {
            _m = BitmapConverter.ToMat(bitmap);
            Proc();
        }

        public Ocr(string path)
        {
            _m = new Mat(path);
            Proc();
        }

        private void Proc()
        {
            Cv2.CvtColor(_m, _m, ColorConversionCodes.BGR2GRAY);

            int width = _m.Width;
            Cv2.Resize(_m, _m, new OpenCvSharp.Size(0, 0), 480.0 / width, 480.0 / width, InterpolationFlags.Cubic);
            if (_m.Height < 320 + 48)
            {
                Cv2.CopyMakeBorder(_m, _m, 0, 320 + 48 - _m.Height, 0, 0, BorderTypes.Constant, 0);
            }
            //Cv2.ImShow("resized", _m);

            Mat rare = 255 - new Mat(_m, new OpenCvSharp.Rect(480 - 130, 0, 125, 48));
            Cv2.Threshold(rare, rare, 200, 255, ThresholdTypes.Otsu);
            Mat slot = new Mat(_m, new OpenCvSharp.Rect(480 - 174, 48, 174, 48));
            Mat skill1 = 255 - new Mat(_m, new OpenCvSharp.Rect(44, 170, 320, 48));
            Mat level1 = 255 - new Mat(_m, new OpenCvSharp.Rect(480 - 110, 220, 105, 48));
            Cv2.Threshold(level1, level1, 200, 255, ThresholdTypes.Otsu);
            Mat skill2 = 255 - new Mat(_m, new OpenCvSharp.Rect(44, 270, 320, 48));
            Mat level2 = 255 - new Mat(_m, new OpenCvSharp.Rect(480 - 110, 320, 105, 48));
            Cv2.Threshold(level2, level2, 200, 255, ThresholdTypes.Otsu);

            Cv2.ImShow("rare", rare);
            //Cv2.ImShow("slot", slot);
            Cv2.ImShow("s1", skill1);
            Cv2.ImShow("l1", level1);
            Cv2.ImShow("s2", skill2);
            Cv2.ImShow("l2", level2);


            res["rare"] = Start_Ocr(BitmapConverter.ToBitmap(rare), "en");
            res["slot"] = Proc_Slot(slot);
            res["s1"] = Start_Ocr(BitmapConverter.ToBitmap(skill1), "ch");
            res["l1"] = Start_Ocr(BitmapConverter.ToBitmap(level1), "en");
            res["s2"] = Start_Ocr(BitmapConverter.ToBitmap(skill2), "ch");
            res["l2"] = Start_Ocr(BitmapConverter.ToBitmap(level2), "en");
        }

        private string Proc_Slot(Mat slot)
        {
            Mat slot1 = new Mat(slot, new OpenCvSharp.Rect(0, 0, 60, 48));
            Mat slot2 = new Mat(slot, new OpenCvSharp.Rect(60, 0, 54, 48));
            Mat slot3 = new Mat(slot, new OpenCvSharp.Rect(114, 0, 54, 48));

            Cv2.Threshold(slot1, slot1, 80, 255, ThresholdTypes.Binary);
            Cv2.Threshold(slot2, slot2, 80, 255, ThresholdTypes.Binary);
            Cv2.Threshold(slot3, slot3, 80, 255, ThresholdTypes.Binary);

            //Cv2.ImShow("slot1", slot1);
            //Cv2.ImShow("slot2", slot2);
            //Cv2.ImShow("slot3", slot3);
            //Console.WriteLine("1 " + Cv2.Sum(slot1)[0]);
            //Console.WriteLine("2 " + Cv2.Sum(slot2)[0]);
            //Console.WriteLine("3 " + Cv2.Sum(slot3)[0]);

            double s1 = Cv2.Sum(slot1)[0];
            double s2 = Cv2.Sum(slot2)[0];
            double s3 = Cv2.Sum(slot3)[0];

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

        private int Sub_Slot(double sum)
        {
            if (sum > 300000)
            {
                return 1;
            }
            else if (sum > 260000)
            {
                return 2;
            }
            else if (sum < 100000)
            {
                return 0;
            }
            else
            {
                return 3;
            }
        }


        private string Start_Ocr(Bitmap image, string lang = "ch")
        {
            TesseractEngine engine;
            switch (lang)
            {
                case "en":
                    engine = new TesseractEngine("tessdata", "eng", EngineMode.LstmOnly);
                    engine.SetVariable("tessedit_char_whitelist", "RARELv1234567");
                    break;
                case "ch":
                default:
                    engine = new TesseractEngine("tessdata", "chi_sim", EngineMode.LstmOnly);
                    engine.SetVariable("tessedit_char_whitelist", "耐力急速回复跳跃铁人跑者翔虫使墙面移动体术地质学植生学滑走强化饥饿耐性剥取铁人剥取名人幸运捕获名人快吃体力回复量提升道具使用强化最爱蘑菇满足感广域化炸弹客泡沫之舞回避性能回避距离提升飞身跃入防御性能防御强化精灵加护纳刀术减轻胆怯回复速度耳塞风压耐性耐震昏厥耐性麻痹耐性毒耐性睡眠耐性泥雪耐性爆破异常状态的耐性属性异常状态的耐性鬼火缠攻击防御火耐性水耐性冰耐性雷耐性龙耐性火属性攻击强化水属性攻击强化冰属性攻击强化雷属性攻击强化龙属性攻击强化毒属性强化爆破属性强化睡眠属性强化麻痹属性强化击晕术破坏王夺取耐力骑乘名人佯动看破弱点特效精神抖擞拔刀术【技】拔刀术【力】超会心会心击【属性】攻击守势火场怪力龙气活性怨恨逆袭死里逃生不屈无伤力量解放挑战者钝器能手集中强化持续炮术匠利刃达人艺心眼砥石使用高速化刚刃打磨高速变形炮弹装填吹笛名人通常弹・连射箭强化散弹・扩散箭强化贯穿弹・贯穿箭强化弹丸节约弹道强化特殊射击强化减轻后坐力装填速度装填扩充抑制偏移速射强化解放弓的蓄力阶段雷纹一致风纹一致霞皮的恩惠钢壳的恩惠炎鳞的恩惠风雷合一");
                    engine.SetVariable("lstm_choice_mode", "2");
                    break;
            }
            engine.DefaultPageSegMode = PageSegMode.SingleLine;
            Pix img = PixConverter.ToPix(image);
            Page page = engine.Process(img);
            return page.GetText();
        }
    }
}
