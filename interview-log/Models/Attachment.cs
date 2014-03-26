using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace interview_log.Models
{
    public enum Type : byte
    {
        File, Image
    };

    public class Attachment
    {
        private static string imageFolder = "~/uploads/images";
        private static string fileFolder = "~/uploads/files";

        public Attachment() { }
        public Attachment(ref HttpPostedFileBase file, string userId)
        {
            this.UserId = userId;
            UploadFile(ref file);
        }
        [Key]
        public Guid Id {get; private set; }
        public int DownloadsCount { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string UserId { get; set; }
        public string FullPath { get; set; }

        private void UploadFile(ref HttpPostedFileBase file)
        {
            string directoryPath;
            if (file.ContentLength > 0)
            {
                Type = GetType(file.ContentType);
                directoryPath = GetPath(Type == Type.Image ? imageFolder : fileFolder);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string path = Path.Combine(directoryPath, file.FileName);
                FullPath = GenerateUniqueFileName(path);
                file.SaveAs(FullPath);
                Name = Path.GetFileName(FullPath);
                Size = ReadableSize(file.ContentLength);
                Id = Guid.NewGuid();
            }
        }

        private string ReadableSize(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        private string GetPath(string path)
        {
            return HttpContext.Current.Server.MapPath(Path.Combine(path, UserId));
        }

        private Type GetType(string MIMEType)
        {
            Regex image = new Regex("image");
            return image.IsMatch(MIMEType) ? Type.Image : Type.File;
        }

        private string GenerateUniqueFileName(string fullPath){
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string newFullPath = fullPath;
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }

        public void IncDlCounter()
        {
            DownloadsCount++;
        }

        public void Delete()
        {
            if (File.Exists(FullPath)) File.Delete(FullPath);
        }

    }

}