using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.BlobStoring.TencentCloud.Infrastructure
{
    public class CAPIConfiguration
    {
        /// <summary>
        /// 云 API 密钥 AppId, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
        /// </summary>
        public string AppId { get; protected set; }
        /// <summary>
        /// 云 API 密钥 SecretId, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
        /// </summary>
        public string SecretId { get; protected set; }
        /// <summary>
        /// 云 API 密钥 SecretKey, 获取 API 密钥请参照 https://console.cloud.tencent.com/cam/capi
        /// </summary>
        public string SecretKey { get; protected set; }

        protected CAPIConfiguration() { }
        public CAPIConfiguration(string appId, string secretId, string secretKey)
        {
            AppId = appId;
            SecretId = secretId;
            SecretKey = secretKey;
        }
    }
}
