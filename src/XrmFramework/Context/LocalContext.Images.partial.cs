using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework;

partial class LocalContext
{
    public bool HasPreImage(string imageName)
    {
        return ExecutionContext.PreEntityImages.ContainsKey(imageName);
    }

    public virtual Entity GetPreImage(string imageName)
    {
        VerifyPreImage(imageName);
        return ExecutionContext.PreEntityImages[imageName];
    }

    public virtual Entity GetPreImageOrDefault(string imageName)
    {
        if (!ExecutionContext.PreEntityImages.ContainsKey(imageName))
        {
            return null;
        }

        return ExecutionContext.PreEntityImages[imageName];
    }

    public bool HasPostImage(string imageName)
    {
        return ExecutionContext.PostEntityImages.ContainsKey(imageName);
    }

    public virtual Entity GetPostImage(string imageName)
    {
        VerifyPostImage(imageName);
        return ExecutionContext.PostEntityImages[imageName];
    }

    protected void VerifyPreImage(string imageName)
    {
        VerifyImage(ExecutionContext.PreEntityImages, imageName, true);
    }

    protected void VerifyPostImage(string imageName)
    {
        VerifyImage(ExecutionContext.PostEntityImages, imageName, false);
    }

    private void VerifyImage(EntityImageCollection collection, string imageName, bool isPreImage)
    {
        if (!collection.Contains(imageName)
            || collection[imageName] == null)
        {
            throw new ArgumentNullException(imageName,
                $@"{(isPreImage ? "PreImage" : "PostImage")} {imageName} does not exist in this context");
        }
    }
}