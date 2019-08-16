using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using glTech.Log4netWrapper;

namespace QJ_FileCenter
{
    public class AliyunHelp
    {
        private static string accessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
        private static string accessKeySecret = ConfigurationManager.AppSettings["accessKeySecret"];
        private static string endpoint = ConfigurationManager.AppSettings["endpoint"];
        private static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        private static string bucketName = ConfigurationManager.AppSettings["bucketName"];


        public static void UploadToOSS(string fileMD5, string fileExt,Stream fs)
        {

            fs.Position = 0;
            var key = fileMD5 + "." + fileExt;
            //var uploadFile = fileLocalPath + @"\" + date + @"\" + key;
            try
            {
                bool UploadStatus = false;
                #region 从本地读取视频文件并上传

                var content = fs;
                //using (var content = File.Open(uploadFile, FileMode.Open))
                //{
                    if (content.Length < 50 * 1024 * 1024) //50M
                    {
                        //Common.WriteLog(string.Format("文件{0}上传开始", key));
                        var resultS = client.PutObject(bucketName, key, content);
                        UploadStatus = true;
                        //Common.WriteLog(string.Format("文件{0}上传成功，返回信息为{1}", key, resultS.ETag));
                    }
                    else
                    {
                        //初始化分片上传
                        //Common.WriteLog(string.Format("文件{0}开始分片上传", key));
                        var request1 = new InitiateMultipartUploadRequest(bucketName, key);
                        var UploadId = client.InitiateMultipartUpload(request1).UploadId;

                        int partCount = 0;
                        var fileSize = content.Length;
                        int partSize = 10 * 1024 * 1024;
                        partCount = (int)(fileSize / partSize + (fileSize % partSize == 0 ? 0 : 1));


                        // 开始分片上传
                        var partETags = new List<PartETag>();
                        for (var i = 0; i < partCount; i++)
                        {
                            var skipBytes = (long)partSize * i;

                            //定位到本次上传片应该开始的位置
                            content.Seek(skipBytes, 0);

                            //计算本次上传的片大小，最后一片为剩余的数据大小，其余片都是part size大小。
                            var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                            var request = new UploadPartRequest(bucketName, key, UploadId)
                            {
                                InputStream = content,
                                PartSize = size,
                                PartNumber = i + 1
                            };

                            //调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值
                            var result2 = client.UploadPart(request);
                            partETags.Add(result2.PartETag);
                        }
                        //完成分片上传
                        var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(bucketName, key, UploadId);
                        foreach (var partETag in partETags)
                        {
                            completeMultipartUploadRequest.PartETags.Add(partETag);
                        }
                        var resultEnd = client.CompleteMultipartUpload(completeMultipartUploadRequest);

                        UploadStatus = true;


                        //Common.WriteLog(string.Format("文件{0}分片上传结束", key));
                    }

                //}
                #endregion

            }
            catch (Exception ex)
            {
                Logger.LogError("阿里云上传问题："+ex.Message);
            }

        }

    }
}
