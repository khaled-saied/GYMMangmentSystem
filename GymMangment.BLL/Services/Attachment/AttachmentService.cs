using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymMangment.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly List<string> _allowedExtensions = [".jpg", ".jpeg", ".png"];

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
        {
            this._logger = logger;
            this._env = env;
        }

        public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead)
                return Result<string>.Fail("File stream is null or cannot be read");
            if (fileStream.Length == 0)
                return Result<string>.Fail("File stream is empty");

            //1=> Check Extension And Size
            if (fileStream.Length > _maxFileSize)// 5 MB{
            {
                _logger.LogError("File size exceeds the maximum limit of 5 MB. File size: {FileSize} bytes", fileStream.Length);
                return Result<string>.Fail("File size exceeds the maximum limit of 5 MB");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError("File extension {Extension} is not allowed. Allowed extensions: {AllowedExtensions}", extension, string.Join(", ", _allowedExtensions));
                return Result<string>.Fail($"File extension {extension} is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
            }

            //2=> Locate the folder path && Create the folder if it doesn't exist
            var uploadFolderPath = Path.Combine(_env.ContentRootPath, folderName);

            Directory.CreateDirectory(uploadFolderPath);

            //3=> Make the file name unique Using a GUID
            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            //4=> Combine the folder path and the unique file name to get the full file path
            var filePath = Path.Combine(uploadFolderPath, storedFileName);

            //5=> Open a file Stream to write the file to disk
            try
            {
                using var fileWriteStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fileWriteStream, ct);
                return Result<string>.Ok(storedFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file {FileName} to folder {FolderName}", fileName, folderName);
                return Result<string>.Fail("An error occurred while uploading the file");
            }



        }

        public Result DeleteFile(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

            try
            {
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("File {FileName} not found in folder {FolderName}", fileName, folderName);
                    return Result.NotFound($"File {fileName} not found in folder {folderName}");
                }
                File.Delete(fullPath);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the file {FileName} from folder {FolderName}", fileName, folderName);
                return Result.Fail("An error occurred while deleting the file");
            }

        }

        public Result<(Stream stream, string ContentType)> GetFile(string fileName, string folderName)
        {
            if(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                return Result<(Stream stream, string ContentType)>.Fail("File name or folder name is null or empty");

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
           
            if(!File.Exists(fullPath)) 
                return Result<(Stream stream, string ContentType)>.NotFound($"File {fileName} not found in folder {folderName}");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            var extension = Path.GetExtension(fileName).ToLower();

            var contentType = extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return Result<(Stream stream, string ContentType)>.Ok((stream, contentType));
        }

    }
}
