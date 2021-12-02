using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace Volo.Abp.BlobStoring.TencentCloud
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringTencentCloudModule : AbpModule
    {
    }
}
