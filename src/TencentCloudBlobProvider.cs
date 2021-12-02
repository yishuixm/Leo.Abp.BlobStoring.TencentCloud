using COSXML.Model.Tag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.TencentCloud.Infrastructure;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.BlobStoring.TencentCloud
{
    public class TencentCloudBlobProvider : BlobProviderBase, ITransientDependency
    {
        protected ITencentCloudBlobNameCalculator TencentCloudBlobNameCalculator { get; }

        public TencentCloudBlobProvider(ITencentCloudBlobNameCalculator tencentCloudBlobNameCalculator)
        {
            TencentCloudBlobNameCalculator = tencentCloudBlobNameCalculator;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var blobName = TencentCloudBlobNameCalculator.Calculate(args);
            var configuration = args.Configuration.GetTencentCloudBlobProviderConfiguration();
            var client = GetClient(args);

            if (!args.OverrideExisting && await BlobExistsAsync(args, blobName))
            {
                throw new BlobAlreadyExistsException(
                    $"Saving BLOB '{args.BlobName}' does already exists in the container '{GetContainerName(args)}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExistsAsync(args);
            }

            await client.UploadObjectAsync(GetContainerName(args), blobName, args.BlobStream);
        }

        public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var blobName = TencentCloudBlobNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);
            var client = GetClient(args);

            if (!await BlobExistsAsync(args, blobName))
            {
                return false;
            }

            await client.DeleteObjectAsync(containerName, blobName);

            return true;
        }

        public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            return BlobExistsAsync(args, TencentCloudBlobNameCalculator.Calculate(args));
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var blobName = TencentCloudBlobNameCalculator.Calculate(args);
            var containerName = GetContainerName(args);
            var client = GetClient(args);

            if (!await client.CheckObjectIsExistAsync(containerName, blobName))
            {
                return null;
            }

            return await client.DownloadObjectAsync(containerName, blobName);
        }

        #region Base
        protected virtual CosServerWrapObject GetClient(BlobProviderArgs args)
        {
            var configuration = args.Configuration.GetTencentCloudBlobProviderConfiguration();
            return new CosServerWrapObject(new CosServerConfiguration(
                configuration.AppId, 
                configuration.SecretId, 
                configuration.SecretKey, 
                region: configuration.Region,
                connectionLimit: 512,
                connectionTimeout: configuration.ConnectionTimeout,
                readWriteTimeout: configuration.ReadWriteTimeout,
                keyDurationSecond: configuration.KeyDurationSecond));
        }

        protected virtual string GetContainerName(BlobProviderArgs args)
        {
            var configuration = args.Configuration.GetTencentCloudBlobProviderConfiguration();
            return configuration.ContainerName.IsNullOrWhiteSpace()
                ? args.ContainerName
                : $"{configuration.ContainerName}-{configuration.AppId}";
        }

        protected virtual async Task<bool> BlobExistsAsync(BlobProviderArgs args, string blobName)
        {
            var client = GetClient(args);
            var containerName = GetContainerName(args);

            return await client.CheckBucketIsExistAsync(containerName) &&
                   await client.CheckObjectIsExistAsync(containerName, blobName);
        }

        protected virtual async Task CreateContainerIfNotExistsAsync(BlobProviderArgs args)
        {
            var client = GetClient(args);
            var containerName = GetContainerName(args);

            if (!await client.CheckBucketIsExistAsync(containerName))
            {
                await client.CreateBucketAsync(containerName);
            }
        }
        #endregion
    }
}
