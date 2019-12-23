using Nancy;
using Nancy.Helpers;

namespace QJ_FileCenter
{
    public static class ResponseExtension
    {
        public static Response AsFileV1(this IResponseFormatter formatter
            , string applicationRelativeFilePath, string contentType, string fileNameExtension, string fileName)
        {
            var response = new GenericFileResponseEx(applicationRelativeFilePath, contentType);

            return response.WithHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName) + "." + fileNameExtension + "");
            //return response;
        }


        public static Response AsPreviewFile(this IResponseFormatter formatter
            , string applicationRelativeFilePath, string contentType, string fileNameExtension, string fileName)
        {
            var response = new GenericFileResponseEx(applicationRelativeFilePath, contentType);
            //return response.WithHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName) + "." + fileNameExtension + "");
            return response;
        }

        public static Response AsVideoFile(this IResponseFormatter formatter
          , string applicationRelativeFilePath, string contentType, string fileNameExtension, string fileName,string strRange)
        {
            var response = new GenericFileResponseEx(applicationRelativeFilePath, contentType, strRange);
            //return response.WithHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName) + "." + fileNameExtension + "");
           
            return response;
        }
    }
}
