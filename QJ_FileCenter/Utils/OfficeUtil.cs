using glTech.Log4netWrapper;
using QJFile.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QJ_FileCenter
{

    #region 将word文档转换为图片
    public class OfficeConverter
    {



        public void ConverFile(Document File)
        {
            OfficeConverter Converter = new OfficeConverter();
            if (new List<string>() { ".doc", ".docx" }.Contains(File.Extension.ToLower()))
            {
                Converter.WordToImage(File, File.FullPath.Substring(0, File.FullPath.LastIndexOf('.')), 0, 0, null, 200);
            }
            if (new List<string>() { ".pdf" }.Contains(File.Extension.ToLower()))
            {
                Converter.PdfToImage(File, File.FullPath.Substring(0, File.FullPath.LastIndexOf('.')), 0, 0, 200);
            }
            if (new List<string>() { ".ppt", ".pptx" }.Contains(File.Extension.ToLower()))
            {
                Converter.PPTToImage(File, File.FullPath.Substring(0, File.FullPath.LastIndexOf('.')), 0, 0, 200);
            }
            if (new List<string>() { ".mp4", ".flv", ".ogg", ".avi", ".mov", ".rmvb", ".mkv"}.Contains(File.Extension.ToLower()))
            {
                Converter.CovVideo(File);
            }

        }



        /// <summary>
        /// 将Word文档转换为图片的方法      
        /// </summary>
        /// <param name="wordInputPath">Word文件路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认值为Word所在路径</param>      
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换，如果为0，默认值为Word总页数</param>
        /// <param name="imageFormat">设置所需图片格式，如果为null，默认格式为PNG</param>
        /// <param name="resolution">设置图片的像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        public void WordToImage(Document file, string imageOutputDirPath, int startPageNum, int endPageNum, ImageFormat imageFormat, int resolution = 128)
        {
            file.isyl = "1";
            new DocumentB().Update(file);
            Task<string> TaskCover = Task.Factory.StartNew<string>(() =>
            {


                Aspose.Words.Document doc = new Aspose.Words.Document(file.FullPath);

                if (doc == null)
                {
                    Logger.LogError("Word文件无效或者Word文件被加密！");

                }
                if (imageOutputDirPath.Trim().Length == 0)
                {
                    imageOutputDirPath = Path.GetDirectoryName(file.FullPath);
                }

                if (!Directory.Exists(imageOutputDirPath))
                {
                    Directory.CreateDirectory(imageOutputDirPath);
                }

                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }

                if (endPageNum > doc.PageCount || endPageNum <= 0)
                {
                    endPageNum = doc.PageCount;
                }

                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum; startPageNum = endPageNum; endPageNum = startPageNum;
                }

                if (imageFormat == null)
                {
                    imageFormat = ImageFormat.Png;
                }
                string imageName = Path.GetFileNameWithoutExtension(file.FullPath);
                Aspose.Words.Saving.ImageSaveOptions imageSaveOptions = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Png);
                imageSaveOptions.Resolution = resolution;
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    MemoryStream stream = new MemoryStream();
                    imageSaveOptions.PageIndex = i - 1;
                    string imgPath = Path.Combine(imageOutputDirPath, imageName) + "_" + i.ToString() + "." + imageFormat.ToString();
                    doc.Save(stream, imageSaveOptions);
                    Image img = Image.FromStream(stream);
                    Bitmap bm = new Bitmap(img);
                    bm.Save(imgPath, imageFormat);
                    img.Dispose();
                    stream.Dispose();
                    bm.Dispose();
                }
                file.ylinfo = endPageNum.ToString();
                file.isyl = "2";
                new DocumentB().Update(file);
                return "success";
            });
        }


        public void PdfToImage(Document file, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution = 128)
        {
            file.isyl = "1";
            new DocumentB().Update(file);
            Task<string> TaskCover = Task.Factory.StartNew<string>(() =>
            {

                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file.FullPath);
                if (doc == null)
                {
                    throw new Exception("pdf文件无效或者pdf文件被加密！");
                }

                if (imageOutputDirPath.Trim().Length == 0)
                {
                    imageOutputDirPath = Path.GetDirectoryName(file.FullPath);
                }

                if (!Directory.Exists(imageOutputDirPath))
                {
                    Directory.CreateDirectory(imageOutputDirPath);
                }

                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }

                if (endPageNum > doc.Pages.Count || endPageNum <= 0)
                {
                    endPageNum = doc.Pages.Count;
                }

                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum; startPageNum = endPageNum; endPageNum = startPageNum;
                }
                string imageNamePrefix = Path.GetFileNameWithoutExtension(file.FullPath);
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    MemoryStream stream = new MemoryStream();
                    string imgPath = Path.Combine(imageOutputDirPath, imageNamePrefix) + "_" + i.ToString() + ".png";
                    Aspose.Pdf.Devices.Resolution reso = new Aspose.Pdf.Devices.Resolution(resolution);
                    Aspose.Pdf.Devices.JpegDevice jpegDevice = new Aspose.Pdf.Devices.JpegDevice(reso, 100);
                    jpegDevice.Process(doc.Pages[i], stream);

                    Image img = Image.FromStream(stream);
                    Bitmap bm = new Bitmap(img);
                    bm.Save(imgPath, ImageFormat.Jpeg);
                    img.Dispose();
                    stream.Dispose();
                    bm.Dispose();

                }
                file.ylinfo = endPageNum.ToString();
                file.isyl = "2";
                new DocumentB().Update(file);
                return "success";
            });
        }




        public string PdfToImageByPath(string FilePath, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution = 128)
        {

            Aspose.Pdf.Document doc = new Aspose.Pdf.Document(FilePath);
            if (doc == null)
            {
                throw new Exception("pdf文件无效或者pdf文件被加密！");
            }

            if (imageOutputDirPath.Trim().Length == 0)
            {
                imageOutputDirPath = Path.GetDirectoryName(FilePath);
            }

            if (!Directory.Exists(imageOutputDirPath))
            {
                Directory.CreateDirectory(imageOutputDirPath);
            }

            if (startPageNum <= 0)
            {
                startPageNum = 1;
            }

            if (endPageNum > doc.Pages.Count || endPageNum <= 0)
            {
                endPageNum = doc.Pages.Count;
            }

            if (startPageNum > endPageNum)
            {
                int tempPageNum = startPageNum; startPageNum = endPageNum; endPageNum = startPageNum;
            }
            string imageNamePrefix = Path.GetFileNameWithoutExtension(FilePath);
            for (int i = startPageNum; i <= endPageNum; i++)
            {
                MemoryStream stream = new MemoryStream();
                string imgPath = Path.Combine(imageOutputDirPath, imageNamePrefix) + "_" + i.ToString() + ".png";
                Aspose.Pdf.Devices.Resolution reso = new Aspose.Pdf.Devices.Resolution(resolution);
                Aspose.Pdf.Devices.JpegDevice jpegDevice = new Aspose.Pdf.Devices.JpegDevice(reso, 100);
                jpegDevice.Process(doc.Pages[i], stream);

                Image img = Image.FromStream(stream);
                Bitmap bm = new Bitmap(img);
                bm.Save(imgPath, ImageFormat.Jpeg);
                img.Dispose();
                stream.Dispose();
                bm.Dispose();

            }
            return endPageNum.ToString();

        }


        public void PPTToImage(Document file, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution = 128)
        {
            file.isyl = "1";
            new DocumentB().Update(file);
            Task<string> TaskCover = Task.Factory.StartNew<string>(() =>
            {

                Aspose.Slides.Presentation doc = new Aspose.Slides.Presentation(file.FullPath);

                if (doc == null)
                {
                    throw new Exception("ppt文件无效或者ppt文件被加密！");
                }

                if (imageOutputDirPath.Trim().Length == 0)
                {
                    imageOutputDirPath = Path.GetDirectoryName(file.FullPath);
                }

                if (!Directory.Exists(imageOutputDirPath))
                {
                    Directory.CreateDirectory(imageOutputDirPath);
                }

                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }

                if (endPageNum > doc.Slides.Count || endPageNum <= 0)
                {
                    endPageNum = doc.Slides.Count;
                }

                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum; startPageNum = endPageNum; endPageNum = startPageNum;
                }

                //先将ppt转换为pdf临时文件
                string tmpPdfPath = imageOutputDirPath + ".pdf";
                doc.Save(tmpPdfPath, Aspose.Slides.Export.SaveFormat.Pdf);
                //再将pdf转换为图片
                OfficeConverter toimg = new OfficeConverter();
                string strFileLen = toimg.PdfToImageByPath(tmpPdfPath, imageOutputDirPath, 0, 0, resolution);
                //删除pdf临时文件
                // File.Move(tmpPdfPath, imageOutputDirPath);
                file.isyl = "2";
                file.ylinfo = strFileLen;
                new DocumentB().Update(file);
                return "success";
            });




        }



        public string YLExcel(string originFilePath)
        {
            string ExcelHtml = originFilePath.Substring(0, originFilePath.LastIndexOf('.')) + ".html";
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(originFilePath);
            workbook.Save(ExcelHtml, Aspose.Cells.SaveFormat.Html);
            return ExcelHtml;
        }

        public void CovVideo(Document file)
        {
            file.isyl = "1";
            new DocumentB().Update(file);
            string path = AppDomain.CurrentDomain.BaseDirectory;

            Task<string> TaskCover = Task.Factory.StartNew<string>(() =>
            {
                //p.StartInfo.UseShellExecute = false;
                //string srcFileName = "";
                //string destFileName = "";
                //string newFileName = "";
                //string mbgs = "." + comboBox2.SelectedItem.ToString();

                //srcFileName = VideoFile.FullPath;
                //newFileName = lv.Items[i].SubItems[0].Text.Split('.')[0];

                //destFileName = "\"" + label3.Text + "\\" + newFileName + DateTime.Now.ToString("yyyyMMddhhmmss");
                ////FFMPEG - i  D://SP.mp4 - c:v libx264 -strict - 2 D://SPZ.mp4
                ///ffmpeg -i D://SP.mp4 -vcodec h264 "D://SPZ.mp4"

                //p.StartInfo.Arguments = "-i " + srcFileName + " -y  -vcodec h264 -b 500000 " + destFileName + mbgs + "\"";    //执行参数
                // p.StartInfo.Arguments = "-i " + srcFileName + " -vcodec h264 " + destFileName ;    //执行参数
                //F:\Code\LotusFileCenter\QJ_FileCenter\bin>ffmpeg -i D://1.mp4 -b 64k -vcodec h264 "D://1Z.mp4"
                //ffmpeg -i input -vf scale=iw/2:-1 output 调整分辨率
                // ffmpeg - i D://1.mp4 -vf scale=iw/2:-1  -vcodec h264 "D://1Z.mp4"
                // ffmpeg - i D://1.mp4 -vf scale=540:960 -b:a 48K  -vcodec h264 "D://1Z2.mp4"
                //F:\Code\LotusFileCenter\QJ_FileCenter\bin\ffmpeg 

                //ffmpeg - i D:\\2.MP4 - y - f image2 - t 0.001 - s 352x240 D:\\2.jpg
                string strFolder = file.Directory.TrimEnd('\\') + "\\" + file.Md5;
                string tempfile = strFolder + "\\" + file.Md5;
                if (!Directory.Exists(strFolder))
                {
                    Directory.CreateDirectory(strFolder);
                }
                #region 截图
                Process p1 = new Process();
                p1.StartInfo.FileName = path + @"\ffmpeg.exe";
                p1.StartInfo.Arguments = "-i " + file.FullPath + " -y -f image2 -t 0.001 -s 352x240 " + tempfile + ".jpg";
                p1.StartInfo.UseShellExecute = false;  ////不使用系统外壳程序启动进程
                p1.StartInfo.CreateNoWindow = true;  //不显示dos程序窗口
                p1.StartInfo.RedirectStandardInput = true;
                p1.StartInfo.RedirectStandardOutput = true;
                p1.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中
                p1.ErrorDataReceived += new DataReceivedEventHandler(Error);
                p1.OutputDataReceived += new DataReceivedEventHandler(Output);
                p1.StartInfo.UseShellExecute = false;
                p1.Start();
                p1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p1.BeginErrorReadLine();//开始异步读取
                p1.WaitForExit();//阻塞等待进程结束
                p1.Close();//关闭进程
                p1.Dispose();//释放资源
                #endregion



                #region 转换视频
                Process p = new Process();
                p.StartInfo.FileName = path + @"\ffmpeg.exe";
                p.StartInfo.Arguments = "-i " + file.FullPath + " -vf scale=960:540 -b:a 48K   -vcodec h264 \"" + tempfile + "_temp.mp4\"";
                p.StartInfo.UseShellExecute = false;  ////不使用系统外壳程序启动进程
                p.StartInfo.CreateNoWindow = true;  //不显示dos程序窗口
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中
                p.ErrorDataReceived += new DataReceivedEventHandler(Error);
                p.OutputDataReceived += new DataReceivedEventHandler(Output);
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.BeginErrorReadLine();//开始异步读取
                p.WaitForExit();//阻塞等待进程结束
                p.Close();//关闭进程
                p.Dispose();//释放资源
                #endregion






                #region 边看边播
                Process p2 = new Process();
                p2.StartInfo.FileName = path + @"\qt-faststart.exe ";
                p2.StartInfo.Arguments = tempfile + "_temp.mp4  " + tempfile + "_low.mp4";
                p2.StartInfo.UseShellExecute = false;  ////不使用系统外壳程序启动进程
                p2.StartInfo.CreateNoWindow = true;  //不显示dos程序窗口
                p2.StartInfo.RedirectStandardInput = true;
                p2.StartInfo.RedirectStandardOutput = true;
                p2.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中
                p2.ErrorDataReceived += new DataReceivedEventHandler(Error);
                p2.OutputDataReceived += new DataReceivedEventHandler(Output);
                p2.StartInfo.UseShellExecute = false;
                p2.Start();
                p2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p2.BeginErrorReadLine();//开始异步读取
                p2.WaitForExit();//阻塞等待进程结束
                p2.Close();//关闭进程
                p2.Dispose();//释放资源
                #endregion

                file.ylinfo = "";
                file.isyl = "2";
                new DocumentB().Update(file);
                return "success";
            });

        }

        private void Output(object sendProcess, System.Diagnostics.DataReceivedEventArgs output)
        {
            if (!String.IsNullOrEmpty(output.Data))
            {
                //处理方法...
                string message = output.Data;
                CommonHelp.WriteLOG("Output" + message);

                ////去获取时长
                //string partitio1 = @"Duration: \d{2}:\d{2}:\d{2}.\d{2}";
                //if (RegexHelper.IsMatch(partitio1, output.Data))
                //{
                //    string partition = @"(?<=Duration: )\d{2}:\d{2}:\d{2}.\d{2}";
                //    string timespan = RegexHelper.Matchs(output.Data, partition).FirstOrDefault();
                //    TimeSpan span;
                //    if (TimeSpan.TryParse(timespan, out span))
                //    {
                //        Console.WriteLine(span.TotalMilliseconds);
                //    }
                //}

                ////获取时刻
                //string partitio2 = @"time=\d{2}:\d{2}:\d{2}.\d{2}";
                //if (RegexHelper.IsMatch(partitio2, output.Data))
                //{
                //    string partition = @"(?<=time=)\d{2}:\d{2}:\d{2}.\d{2}";

                //    string timespan = RegexHelper.Matchs(output.Data, partition).FirstOrDefault();
                //    TimeSpan span;
                //    if (TimeSpan.TryParse(timespan, out span))
                //    {
                //        Console.WriteLine(span.TotalMilliseconds);
                //    }
                //}
            }
        }
        private void Error(object sendProcess, System.Diagnostics.DataReceivedEventArgs output)
        {
            if (!String.IsNullOrEmpty(output.Data))
            {
                //处理方法...
               // string message = output.Data;
              //  CommonHelp.WriteLOG("Error" + message);

            }
        }
    }
    #endregion
}
