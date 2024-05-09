using ChatApp.API.Helpers;
using ChatApp.API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace ChatApp.API.Services;
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<ColudinarySettings> config)
    {
        // add config to account
        var account = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        // add passing account to cloudinary
        _cloudinary = new Cloudinary(account);

    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            // read file 
            using var stream = file.OpenReadStream();
            // get upload params file
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(500)
                    .Width(500).
                    Crop("fill").
                    Gravity("face"),
                Folder = "da-net8"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}