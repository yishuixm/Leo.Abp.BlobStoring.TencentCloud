using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Abp.BlobStoring.TencentCloud.Infrastructure
{
    public class CosServerConfiguration
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
        /// <summary>
        /// 设置默认的区域, COS 地域的简称请参照 https://cloud.tencent.com/document/product/436/6224
        /// </summary>
        public string Region { get; protected set; }
        /// <summary>
        /// 每次请求签名有效时长，单位为秒
        /// </summary>
        public long KeyDurationSecond { get; protected set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int ConnectionLimit { get; protected set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        public int ConnectionTimeout { get; protected set; }
        /// <summary>
        /// 读写超时
        /// </summary>
        public int ReadWriteTimeout { get; protected set; }

        protected CosServerConfiguration() { }

        public CosServerConfiguration(string appId, string secretId, string secretKey, string region = "", int connectionLimit = 512, int connectionTimeout = 45, int readWriteTimeout = 45, long keyDurationSecond = 600L)
        {
            AppId = appId;
            SecretId = secretId;
            SecretKey = secretKey;
            Region = region;
            ConnectionLimit = connectionLimit;
            ConnectionTimeout = connectionTimeout;
            ReadWriteTimeout = readWriteTimeout;
            KeyDurationSecond = keyDurationSecond;
        }
    }
}
