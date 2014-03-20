using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace interview_log.Helpers
{
    public struct MyFileInfo
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public MyFileInfo(FileInfo fileInfo) : this()
        {
            Name = fileInfo.Name;
            Size = FilesHelper.BytesToString(fileInfo.Length);
        }
    }

    /// <summary>
    /// A helper to encapsulate the work with files. We do not need to have separate instance of it
    /// because controllers created for each request.
    /// </summary>
    public class FilesHelper
    {
        private static string imageFolder = "~/uploads/images";
        private static string fileFolder = "~/uploads/files";
        
       /// <summary>
       /// Uploads file to server. Used by controllers
       /// </summary>
       /// <param name="file">The file to be uploaded</param>
       /// <param name="userId">The current user's id</param>
        internal static void UploadFile(ref HttpPostedFileBase file, string userId)
        {
            string directoryPath;
            if (file.ContentLength > 0)
            {
                directoryPath = GetPath(
                    (IsImage(file.ContentType) ? imageFolder : fileFolder),
                    userId
                    );
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string fileName = Path.GetFileName(file.FileName);
                string path = Path.Combine(directoryPath, fileName);
                file.SaveAs(path);
            }
        }

        internal static IEnumerable<string> GetImages(string userId)
        {
            IEnumerable<string> result = GetItems(userId, imageFolder);
            return result;                 
        }
        
        internal static IEnumerable<MyFileInfo> GetFileInfos(string userId)
        {
            IEnumerable<string> result = GetItems(userId, fileFolder);
            return result.Select(file => 
            {
                FileInfo fileInfo = new FileInfo(GetPath(fileFolder, userId, file));
                return new MyFileInfo(fileInfo);
            }
            ); 
        } 

        private static bool IsImage(string MIMEType)
        {
            Regex image = new Regex("image");
            return image.IsMatch(MIMEType);
        }

        private static string GetPath(params string[] path)
        {
            return HttpContext.Current.Server.MapPath(Path.Combine(path));
        }

        private static IEnumerable<string> GetItems(string userId, string folder)
        {
            string directoryPath = GetPath(folder, userId);
            return Directory.Exists(directoryPath) ?
                Directory.GetFiles(directoryPath)
                .Select(path =>  Path.GetFileName(path)) : new List<string>();
        }

        internal static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

    }
}