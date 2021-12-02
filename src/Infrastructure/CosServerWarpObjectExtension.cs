using COSXML.CosException;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using COSXML.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Volo.Abp.BlobStoring.TencentCloud.Infrastructure
{
    public static class CosServerWarpObjectExtension
    {
        public static async Task<bool> CheckBucketIsExistAsync(this CosServerWrapObject client, string bucketName)
        {
            try
            {
                var result = await client.CosXmlServer.ExecuteAsync<HeadBucketResult>(new HeadBucketRequest(bucketName));

                return result.IsSuccessful();
            }
            catch (CosServerException serverEx)
            {
                if (serverEx.statusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }

        public static async Task<bool> CheckObjectIsExistAsync(this CosServerWrapObject client, string bucketName, string objectKey)
        {
            try
            {
                var result = await client.CosXmlServer.ExecuteAsync<HeadObjectResult>(new HeadObjectRequest(bucketName, objectKey));

                return result.IsSuccessful();
            }
            catch (CosServerException exception)
            {
                if (exception.statusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }

        public static async Task CreateBucketAsync(this CosServerWrapObject client, string bucketName)
        {
            await client.CosXmlServer.ExecuteAsync<PutBucketResult>(new PutBucketRequest(bucketName));
        }

        public static async Task UploadObjectAsync(this CosServerWrapObject client,
            string bucketName,
            string objectKey,
            byte[] data)
        {
            await client.CosXmlServer.ExecuteAsync<PutObjectResult>(new PutObjectRequest(bucketName, objectKey, data));
        }

        public static async Task UploadObjectAsync(this CosServerWrapObject client,
            string bucketName,
            string objectKey,
            Stream data)
        {
            await client.CosXmlServer.ExecuteAsync<PutObjectResult>(new PutObjectRequest(bucketName, objectKey, data));
        }

        public static async Task<Stream> DownloadObjectAsync(this CosServerWrapObject client,
            string bucketName,
            string objectKey)
        {
            var result = await client.CosXmlServer.ExecuteAsync<GetObjectBytesResult>(new GetObjectBytesRequest(bucketName, objectKey));

            return new MemoryStream(result.content);
        }

        public static async Task DeleteObjectAsync(this CosServerWrapObject client,
            string bucketName,
            string objectKey)
        {
            await client.CosXmlServer.ExecuteAsync<DeleteObjectResult>(new DeleteObjectRequest(bucketName, objectKey));
        }

        public static async Task DeleteObjectsAsync(this CosServerWrapObject client,
            string bucketName,
            List<string> objectKeys)
        {
            DeleteMultiObjectRequest request = new DeleteMultiObjectRequest(bucketName);
            //设置返回结果形式
            request.SetDeleteQuiet(false);
            request.SetObjectKeys(objectKeys);

            await client.CosXmlServer.ExecuteAsync<DeleteMultiObjectResult>(request);
        }

        public static async Task<GetBucketResult> GetObjectsAsync(this CosServerWrapObject client,
            string bucketName, 
            string nextMarker = null)
        {
            GetBucketRequest request = new GetBucketRequest(bucketName);
            if (!string.IsNullOrWhiteSpace(nextMarker))
            {
                //上一次拉取数据的下标
                request.SetMarker(nextMarker);
            }

            return await client.CosXmlServer.ExecuteAsync<GetBucketResult>(request);
        }

        public static async Task<COSXMLUploadTask.UploadTaskResult> TransferUploadFileAsync(this CosServerWrapObject client,
            string objectKey,
            string bucketName, 
            string filePath, Action<long, long> progressCallback = null)
        {
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(client.CosXmlServer, transferConfig);

            // 上传对象
            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucketName, objectKey);
            uploadTask.SetSrcPath(filePath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                progressCallback?.Invoke(completed, total);
            };

            return await transferManager.UploadAsync(uploadTask);
        }

        public static async Task<COSXMLUploadTask.UploadTaskResult> TransferUploadAsync(this CosServerWrapObject client, 
            string objectKey,
            string bucketName, 
            byte[] data, 
            Action<long, long> progressCallback = null)
        {

            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(client.CosXmlServer, transferConfig);

            // 上传对象
            COSXMLUploadTask uploadTask = new COSXMLUploadTask(new PutObjectRequest(bucketName, objectKey, data));

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                progressCallback?.Invoke(completed, total);
            };

            return await transferManager.UploadAsync(uploadTask);
        }

        public static string GetCosPathUrl(this CosServerWrapObject client, 
            string appid,
            string region,
            string bucketName,
            string objectKey, 
            bool isSign = false)
        {
            if (isSign)
            {
                PreSignatureStruct preSignatureStruct = new PreSignatureStruct();
                // APPID 获取参考 https://console.cloud.tencent.com/developer
                preSignatureStruct.appid = appid;
                // 存储桶所在地域, COS 地域的简称请参照 https://cloud.tencent.com/document/product/436/6224
                preSignatureStruct.region = region;
                // 存储桶名称，此处填入格式必须为 bucketname-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer
                preSignatureStruct.bucket = $"{bucketName}-{appid}";
                preSignatureStruct.key = objectKey; //对象键
                preSignatureStruct.httpMethod = "GET"; //HTTP 请求方法
                preSignatureStruct.isHttps = true; //生成 HTTPS 请求 URL
                preSignatureStruct.signDurationSecond = 600; //请求签名时间为600s
                preSignatureStruct.headers = null;//签名中需要校验的 header
                preSignatureStruct.queryParameters = null; //签名中需要校验的 URL 中请求参数

                return client.CosXmlServer.GenerateSignURL(preSignatureStruct);
            }
            else
                return client.CosXmlServer.GetObjectUrl(bucketName, objectKey);
        }
    }
}
