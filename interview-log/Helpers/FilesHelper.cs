using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace interview_log.Helpers
{
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
                    folder: (IsImage(file.ContentType) ? imageFolder : fileFolder),
                    userId: userId
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
            IEnumerable<string> result = GetItems(userId, "~/uploads/images");
            return result;                  
        }
        
        internal static IEnumerable<string> GetFiles(string userId)
        {
            string directoryPath = GetPath(fileFolder, userId);
            IEnumerable<string> result = GetItems(userId, directoryPath);
            return result;
        } 

        private static bool IsImage(string MIMEType)
        {
            Regex image = new Regex("image");
            return image.IsMatch(MIMEType);
        }

        private static string GetPath(string folder, string userId)
        {
            return HttpContext.Current.Server.MapPath(Path.Combine(folder, userId));
        }

        private static IEnumerable<string> GetItems(string userId, string folder)
        {
            string directoryPath = GetPath(folder, userId);
            return Directory.Exists(directoryPath) ?
                Directory.GetFiles(directoryPath)
                .Select(path => userId + "/" + Path.GetFileName(path)) : new List<string>();
        }
    }
}