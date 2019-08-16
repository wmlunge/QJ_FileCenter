using Nancy;
using Nancy.Helpers;

namespace QJ_FileCenter
{
    public static class ResponseExtension
    {
        public static Response AsFile(this IResponseFormatter formatter
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
    }
}
