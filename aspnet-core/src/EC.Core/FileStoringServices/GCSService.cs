using Abp.Application.Services;
using Abp.Dependency;
using EC.Configuration;
using EC.Constants.FileStoring;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.FileStorageServices
{
    public class GCSService : ApplicationService, IFileStoringService, ITransientDependency
    {
        private StorageClient _storageClient;
        private string _bucketName;

        public GCSService()
        {
            // StorageClient.Create() will automatically use the Google Application Default Credentials
            // which are present in the Cloud Run environment.
            _storageClient = StorageClient.Create();
        }

        private async Task LoadConfig()
        {
            var tenantId = AbpSession.TenantId;
            if (tenantId == null)
            {
                _bucketName = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSBucketName);
            }
            else
            {
                _bucketName = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSBucketName, tenantId.Value);
            }
        }

        public async Task UploadFile(IFormFile file, string tenantName, FileCategory fileCategory, string guid, int? index)
        {
            await LoadConfig();
            string key = await MakeKey(tenantName, fileCategory, guid, index, file.FileName);
            
            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, key, file.ContentType, stream);
            }
        }

        public async Task<byte[]> DownloadFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            await LoadConfig();
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            
            using (var ms = new MemoryStream())
            {
                await _storageClient.DownloadObjectAsync(_bucketName, key, ms);
                return ms.ToArray();
            }
        }

        public async Task<List<byte[]>> DownloadMultipleFiles(string tenantName, FileCategory fileCategory, string guid)
        {
            await LoadConfig();
            string prefix = await MakeKey(tenantName, fileCategory, guid, null, null);
            var fileList = new List<byte[]>();

            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);
            await foreach (var obj in objects)
            {
                using (var ms = new MemoryStream())
                {
                    await _storageClient.DownloadObjectAsync(_bucketName, obj.Name, ms);
                    fileList.Add(ms.ToArray());
                }
            }
            return fileList;
        }

        public async Task DeleteFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            await LoadConfig();
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            await _storageClient.DeleteObjectAsync(_bucketName, key);
        }

        public async Task DeleteMultipleFiles(string tenantName, FileCategory fileCategory, string guid)
        {
            await LoadConfig();
            string prefix = await MakeKey(tenantName, fileCategory, guid, null, null);
            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);
            await foreach (var obj in objects)
            {
                await _storageClient.DeleteObjectAsync(_bucketName, obj.Name);
            }
        }

        public async Task<string> GetDirectDownloadUrl(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            await LoadConfig();
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            
            // Note: Native signed URLs in GCS require some extra setup or a service account key file.
            // For now, we provide a public-ish URL if the bucket policy allows, 
            // or we could implement a custom controller to proxy the download.
            // In a real production app, we'd use UrlSigner.
            return $"https://storage.googleapis.com/{_bucketName}/{key}";
        }

        public async Task<List<string>> SearchForFiles(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            await LoadConfig();
            string prefix = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            var result = new List<string>();

            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);
            await foreach (var obj in objects)
            {
                result.Add(obj.Name);
            }
            return result;
        }

        private async Task<string> MakeKey(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            // We reuse the prefix from settings if available
            string settingPrefix = "";
            var tenantId = AbpSession.TenantId;
            if (tenantId == null)
            {
                settingPrefix = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSPrefix);
            }
            else
            {
                settingPrefix = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSPrefix, tenantId.Value);
            }

            string key = settingPrefix.TrimEnd('/') + '/' + tenantName;

            switch (fileCategory)
            {
                case FileCategory.Attachment:
                    key += '/' + FileStoringConstants.AttachmentFolder.TrimEnd('/');
                    break;
                case FileCategory.UnsignedContract:
                    key += '/' + FileStoringConstants.ContractFolder.TrimEnd('/');
                    key += '/' + FileStoringConstants.UnsignedFolder.TrimEnd('/');
                    break;
                case FileCategory.SignedContract:
                    key += '/' + FileStoringConstants.ContractFolder.TrimEnd('/');
                    key += '/' + FileStoringConstants.SignedFolder.TrimEnd('/');
                    break;
                case FileCategory.Signature:
                    key += '/' + FileStoringConstants.SignatureFolder.TrimEnd('/');
                    break;
                case FileCategory.Download:
                    key += '/' + FileStoringConstants.DownloadFolder.TrimEnd('/');
                    break;
            }

            key += '/' + guid;
            if (index != null)
            {
                key += '_' + index.ToString();
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                key += "_" + fileName;
            }
            return key;
        }
    }
}
