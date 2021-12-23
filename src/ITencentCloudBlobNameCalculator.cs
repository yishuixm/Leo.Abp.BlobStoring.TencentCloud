using Volo.Abp.BlobStoring;

namespace Leo.Abp.BlobStoring.TencentCloud
{
    public interface ITencentCloudBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
        string Calculate(string containerName, string blobName);
    }
}
