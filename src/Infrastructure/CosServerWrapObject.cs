using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using COSXML.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Volo.Abp.BlobStoring.TencentCloud.Infrastructure
{
    public class CosServerWrapObject
    {
        public CosXmlServer CosXmlServer { get; protected set; }

        /// <summary>
        /// 对象存储服务器对象
        /// </summary>
        public CosServerWrapObject(CosServerConfiguration configuration)
        {
            CosXmlServer = new CosXmlServer(
                BuildConfig(configuration),
                GetCredentialProvider(configuration));
        }

        private CosXmlConfig BuildConfig(CosServerConfiguration configuration)
        {
            return new CosXmlConfig.Builder()
                .SetConnectionLimit(configuration.ConnectionLimit)
                .SetConnectionTimeoutMs(configuration.ConnectionTimeout)
                .SetReadWriteTimeoutMs(configuration.ReadWriteTimeout)
                .IsHttps(true)
                .SetAppid(configuration.AppId)
                .SetRegion(configuration.Region)
                .SetDebugLog(false)
                .Build();
        }

        private QCloudCredentialProvider GetCredentialProvider(CosServerConfiguration configuration)
        {
            return new DefaultQCloudCredentialProvider(
                configuration.SecretId,
                configuration.SecretKey,
                configuration.KeyDurationSecond);
        }
    }
}
