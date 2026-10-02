using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using Microsoft.AspNetCore.Http;

namespace GymMangment.BLL.Services.Attachment
{
    public interface IAttachmentService
    {
        Task<Result<string>> UploadFileAsync(Stream fileStream,string fileName, string folderName, CancellationToken ct = default);
    }
}
