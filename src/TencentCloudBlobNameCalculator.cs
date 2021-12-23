using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Leo.Abp.BlobStoring.TencentCloud
{
    public class TencentCloudBlobNameCalculator : ITencentCloudBlobNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        public TencentCloudBlobNameCalculator(ICurrentTenant currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            return CurrentTenant.Id == null
                ? $"host/{args.ContainerName}/{args.BlobName}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{args.ContainerName}/{args.BlobName}";
        }

        public virtual string Calculate(string containerName, string blobName)
        {
            return CurrentTenant.Id == null
                ? $"host/{containerName}/{blobName}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{containerName}/{blobName}";
        }
    }
}
