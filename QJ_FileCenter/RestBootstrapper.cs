using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using QJ_FileCenter.Repositories;
using QJFile.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace QJ_FileCenter
{
    public class RestBootstrapper : DefaultNancyBootstrapper
    {
        public RestBootstrapper()
            : base()
        {
        }


        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("WEB", @"Web"));
            base.ConfigureConventions(nancyConventions);

        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.DisableErrorTraces = false;
            pipelines.AfterRequest += (ctx) =>
            {
                if (ctx.Response.ContentType == "text/html")
                {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };




            StaticConfiguration.EnableRequestTracing = true;


        }





        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(requestContainer, pipelines, context);
            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }

        protected override byte[] FavIcon
        {
            get
            {
                return base.FavIcon;
            }
        }


    }

    public class RootPathStartup : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
        }

        public RootPathStartup(IRootPathProvider rootPathProvider)
        {

            GenericFileResponseEx.RootPaths.Add(rootPathProvider.GetRootPath());
            GenericFileResponseEx.RootPaths.Add(appsetingB.GetValueByKey("path"));
        }
    }

    public class GenericFileResponseEx : Response
    {
        public static List<string> RootPaths { get; set; }

        static GenericFileResponseEx()
        {
            RootPaths = new List<string>();
        }
        public GenericFileResponseEx(string filePath) :
            this(filePath, MimeTypes.GetMimeType(filePath))
        {
        }


        public GenericFileResponseEx(string filePath, string contentType)
        {
            InitializeGenericFileResonse(filePath, contentType);
        }

        public GenericFileResponseEx(string filePath, string contentType, string range)
        {
            InitializeGenericVideoResonse(filePath, contentType, range);
        }

        public string Filename { get; protected set; }

        static Action<Stream> GetFileContent(string filePath)
        {

            return stream =>
            {
                try
                {
                    using (var file = File.OpenRead(filePath))
                    {
                        //SeekOrigin
                        //file.Seek()
                        file.CopyTo(stream);
                    }
                }
                catch (Exception)
                {

                }

            };
        }

        static Action<Stream> GetVideoContent(string filePath, string range)
        {
            return stream =>
            {
                using (var fs = File.OpenRead(filePath))
                {
                    try
                    {
                        // format: bytes=[start]-[end]
                        // documentation: https://tools.ietf.org/html/rfc7233#section-4
                        long bytes_start = 0,
                        bytes_end = fs.Length;
                        if (range != null)
                        {
                            string[] range_info = range.Split(new char[] { '=', '-' });
                            bytes_start = Convert.ToInt64(range_info[1]);
                            if (!string.IsNullOrEmpty(range_info[2]))
                                bytes_end = Convert.ToInt64(range_info[2]);

                            //  response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", bytes_start, bytes_end - 1, fs.Length));
                        }

                        // determine how many bytes we'll be sending to the client in total
                        // response.ContentLength64 = bytes_end - bytes_start;

                        // go to the starting point of the response
                        fs.Seek(bytes_start, SeekOrigin.Begin);
                        fs.CopyTo(stream);
                    }
                    catch (Exception)
                    {

                    }
                  

                }
            };
        }

        static bool IsSafeFilePath(string rootPath, string filePath)
        {
            if (!Path.HasExtension(filePath))
            {
                return false;
            }

            if (!File.Exists(filePath))
            {
                return false;
            }

            var fullPath = Path.GetFullPath(filePath);

            return fullPath.StartsWith(rootPath, StringComparison.Ordinal);
        }

        void InitializeGenericFileResonse(string filePath, string contentType)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                StatusCode = HttpStatusCode.NotFound;
                return;
            }
            if (RootPaths == null || RootPaths.Count == 0)
            {
                throw new InvalidOperationException("No RootPaths defined.");
            }
            foreach (var rootPath in RootPaths)
            {
                string fullPath;
                if (Path.IsPathRooted(filePath))
                {
                    fullPath = filePath;
                }
                else
                {
                    fullPath = Path.Combine(rootPath, filePath);
                }

                if (IsSafeFilePath(rootPath, fullPath))
                {
                    Filename = Path.GetFileName(fullPath);

                    var fi = new FileInfo(fullPath);
                    // TODO - set a standard caching time and/or public?
                    Headers["ETag"] = fi.LastWriteTimeUtc.Ticks.ToString("x");
                    Headers["Last-Modified"] = fi.LastWriteTimeUtc.ToString("R");
                    Contents = GetFileContent(fullPath);
                    ContentType = contentType;
                    StatusCode = HttpStatusCode.OK;
                    return;
                }
            }

            StatusCode = HttpStatusCode.NotFound;
        }

        void InitializeGenericVideoResonse(string filePath, string contentType, string range)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                StatusCode = HttpStatusCode.NotFound;
                return;
            }
            if (RootPaths == null || RootPaths.Count == 0)
            {
                throw new InvalidOperationException("No RootPaths defined.");
            }
            foreach (var rootPath in RootPaths)
            {
                string fullPath;
                if (Path.IsPathRooted(filePath))
                {
                    fullPath = filePath;
                }
                else
                {
                    fullPath = Path.Combine(rootPath, filePath);
                }

                if (IsSafeFilePath(rootPath, fullPath))
                {
                    Filename = Path.GetFileName(fullPath);
                    var fi = new FileInfo(fullPath);

                  
                    // TODO - set a standard caching time and/or public?
                    Headers["ETag"] = fi.LastWriteTimeUtc.Ticks.ToString("x");
                    Headers["Last-Modified"] = fi.LastWriteTimeUtc.ToString("R");
                    Headers["Accept-Ranges"] = "bytes";
                    using (var fs = File.OpenRead(fullPath))
                    {

                        // format: bytes=[start]-[end]
                        // documentation: https://tools.ietf.org/html/rfc7233#section-4
                        long bytes_start = 0,
                        bytes_end = fs.Length;
                        if (range != null)
                        {
                            string[] range_info = range.Split(new char[] { '=', '-' });
                            bytes_start = Convert.ToInt64(range_info[1]);
                            if (!string.IsNullOrEmpty(range_info[2]))
                                bytes_end = Convert.ToInt64(range_info[2]);

                            Headers["Content-Range"] = string.Format("bytes {0}-{1}/{2}", bytes_start, bytes_end - 1, fs.Length);
                            Headers["Content-Length"] = (bytes_end - bytes_start).ToString();
                        }

                    }


                    Contents = GetVideoContent(fullPath, range);
                    ContentType = contentType;
                    StatusCode = HttpStatusCode.PartialContent;
                    return;
                }
            }

            StatusCode = HttpStatusCode.NotFound;
        }
    }



}
