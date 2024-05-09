using System;
namespace Dannys.Extensions
{
	public static class FileExtension
	{
        public static async Task<string> SaveFileAsync(this IFormFile file, params string[] roots)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            string path = "";

            foreach (var root in roots)
            {
                path = Path.Combine(path, root);
            }

            path = Path.Combine(path, uniqueFileName);

            using FileStream fs = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(fs);

            return uniqueFileName;
        }


        public static bool CheckFileType(this IFormFile file, string fileType)
        {
            if (file.ContentType.Contains(fileType))
            {
                return true;
            }
            return false;
        }

        public static bool CheckFileSize(this IFormFile file, int fileSize)
        {
            if (file.Length > fileSize * 1024 * 1024)
            {
                return false;
            }
            return true;
        }
        public static void DeleteFile(this string fileName, params string[] roots)
        {
            string path = "";

            foreach (var root in roots)
            {
                path = Path.Combine(path, root);
            }

            path = Path.Combine(path, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

