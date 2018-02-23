// <copyright file="Children.cs" company="Rambalac">
// Copyright (c) Rambalac. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1300 // Element must begin with upper-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Azi.Amazon.CloudDrive.JsonObjects
{
    public class Children
    {
        public int result { get; set; }

        public AmazonNode metadata { get; set; }
    }

    public class FileUploadResult
    {
        public int result { get; set; }

        public IList<string> fileids { get; set; }

        public IList<AmazonNode> metadata { get; set; }
    }

    public class DeleteRecursiveResult
    {
        public int result { get; set; }

        public int deletedfiles { get; set; }

        public int deletedfolders { get; set; }
    }

    public class DownloadLinkResult
    {
        public int result { get; set; }

        public string path { get; set; }

        public DateTime expires { get; set; }

        public IList<string> hosts { get; set; }
    }

    public class CreateUploadResult
    {
        public int uploadid { get; set; }
    }
}
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore SA1300 // Element must begin with upper-case letter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
