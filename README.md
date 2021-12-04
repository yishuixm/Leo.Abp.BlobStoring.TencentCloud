# Example
```cs
[BlobContainerName("profile-pictures")]
public class ProfilePictureContainer
{
    public const string AppId = "Your AppId";
    public const string SecretId = "Your SecretId";
    public const string SecretKey = "Your SecretKey";
    public const string Region = "Your Region";
    public const string BucketName = "Your BucketName";
    public const string ContainerName = "Your ContainerName";
    public const bool CDN = true; // if you use yours cdn in cos, that set ture else that set false.

    public static string GetUrlsPrefix()
    {
        return CDN ? $"https://{BucketName}-{AppId}.file.myqcloud.com/" : $"https://{BucketName}-{AppId}.cos.{Region}.myqcloud.com/";
    }
}

options.Containers.Configure<ProfilePictureContainer>(container =>
{
    container.UseTencentCloudBlobProvider(configs =>
    {
        configs.AppId = ProfilePictureContainer.AppId;
        configs.SecretId = ProfilePictureContainer.SecretId;
        configs.SecretKey = ProfilePictureContainer.SecretKey;
        configs.Region = ProfilePictureContainer.Region;
        configs.BucketName = ProfilePictureContainer.BucketName;
        configs.ContainerName = ProfilePictureContainer.ContainerName;
        configs.ConnectionLimit = 512;
        configs.ConnectionTimeout = 45000;
        configs.ReadWriteTimeout = 45000;
        configs.KeyDurationSecond = 600;
        configs.CreateContainerIfNotExists = true;
    });
});
```