using Volo.Abp.BlobStoring;

namespace Volo.Abp.BlobStoring.TencentCloud
{
    public interface ITencentCloudBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
