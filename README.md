```cs
Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseTencentCloudBlobProvider();
    });
});
```