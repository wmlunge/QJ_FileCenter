using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Core;

namespace QJ_FileCenter
{
    public static class ZipUtil
    {
        internal static string Compress(IEnumerable<dynamic> documents)
        {
            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create("D:\\1.zip")))
            {
                zipOutputStream.SetLevel(9);
                var abyBuffer = new byte[4096];

                foreach (var document in documents)
                {
                    string filename = document.file;
                    string name = document.name;
                    string extension = document.extension;
                    using (FileStream filestream = File.OpenRead(filename))
                    {
                        var zipEntry = new ZipEntry(name + "." + extension);
                        zipEntry.DateTime = DateTime.Now;
                        zipEntry.Size = filestream.Length;

                        zipOutputStream.PutNextEntry(zipEntry);
                        StreamUtils.Copy(filestream, zipOutputStream, abyBuffer);
                    }
                }
            }

            return "";
        }
    }
}
