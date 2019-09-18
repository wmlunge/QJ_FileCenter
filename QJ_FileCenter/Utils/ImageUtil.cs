using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace QJ_FileCenter
{
    public class ImageCLass
    {


        #region 生成缩略图

        /// <summary>
        /// 根据大图找到缩略图路径???
        /// </summary>
        /// <param name="imgPath">大图路径</param>
        /// <returns></returns>
        public static string GetSmallImg(string imgPath)
        {
            return imgPath.Substring(0, imgPath.LastIndexOf('.') - 2) + System.IO.Path.GetExtension(imgPath);
        }

        /// <summary>
        /// 根据大图生成缩略图
        /// </summary>
        /// <param name="savePath">原始图片路径</param>
        /// <param name="picFilePath">原始图片</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static string GenSmallImg(string imgPath, int width, int height)
        {
            //判断是否有缩略图
            string smallPath = imgPath.Substring(0, imgPath.LastIndexOf('.')) + "_" + width.ToString() + height.ToString() + System.IO.Path.GetExtension(imgPath);

            try
            {
                if (System.IO.File.Exists(smallPath))
                {
                    return smallPath;
                }

                System.Drawing.Image img = System.Drawing.Image.FromFile(imgPath);

                int sourceW = img.Width;
                int sourceH = img.Height;
                int leftX = 0;
                int leftY = 0;
                //width是0时，使用原图尺寸
                if (width == 0)
                {
                    width = img.Width;
                    height = img.Height;
                }
                else
                {
                    //控制缩略图不变形，需要截取
                    if (width < img.Width || height < img.Height)
                    {
                        //计算倍数
                        decimal w = (decimal)img.Width / (width * 1.0M);
                        decimal h = (decimal)img.Height / (height * 1.0M);

                        //找最小的倍数
                        if (w > h)
                        {
                            w = h;
                        }

                        //根据缩略图等比例放大图片
                        sourceW = (int)(width * w);
                        sourceH = (int)(height * w);

                        //左右中间截取
                        if (img.Width - sourceW > 0)
                        {
                            leftX = (img.Width - sourceW) / 2;
                        }

                        //上下中间截取
                        if (img.Height - sourceH > 0)
                        {
                            leftY = (img.Height - sourceH) / 2;
                        }
                    }
                }
                Bitmap tempBitmap = new Bitmap(width, height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tempBitmap);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, width, height);


                g.DrawImage(img, rectDestination, leftX, leftY, sourceW, sourceH, GraphicsUnit.Pixel);
                KiSaveAsJPEG(tempBitmap, smallPath, 90);
                if (img != null)
                    img.Dispose();
                if (tempBitmap != null)
                    tempBitmap.Dispose();
            }
            catch (Exception)
            {

                
            }
            
            return smallPath;
        }
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }

        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="FileName"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        private static bool KiSaveAsJPEG(Bitmap bmp, string FileName, int Qty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;

                ps = new EncoderParameters(1);

                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;
                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string md5(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            string result = string.Empty;
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(source));

            result = BitConverter.ToString(s).Replace("-", string.Empty);
            return result.ToLower();
        }


        /// <summary>
        /// 移除html标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            string result = "";
            Regex regex = new Regex("<.+?>");
            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                html = html.Replace(match.Value, "");
            }
            return result;
        }


    }
}
