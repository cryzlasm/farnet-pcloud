// <copyright file="AmazonNode.cs" company="Rambalac">
// Copyright (c) Rambalac. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1300 // Element must begin with upper-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Azi.Amazon.CloudDrive.JsonObjects
{
    /// <summary>
    /// Cloud drive node status
    /// </summary>
    public enum AmazonNodeStatus
    {
        /// <summary>
        /// Amazon node AVAILABLE
        /// </summary>
        AVAILABLE,

        /// <summary>
        /// Amazon node TRASHed
        /// </summary>
        TRASH,

        /// <summary>
        /// Amazon node PURGED
        /// </summary>
        PURGED
    }

    /// <summary>
    /// Cloud drive node kind
    /// </summary>
    public enum AmazonNodeKind
    {
        /// <summary>
        /// File node
        /// </summary>
        FILE,

        /// <summary>
        /// Asset node
        /// </summary>
        ASSET,

        /// <summary>
        /// Folder node
        /// </summary>
        FOLDER
    }

    /// <summary>
    /// Cloud drive node information. See REST API
    /// </summary>
    public class AmazonNode
    {
        /// <summary>
        /// Gets creation time
        /// </summary>
        // public DateTime FetchTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets file size, 0 for folders.
        /// </summary>
        /// public long Length => contentProperties?.size ?? 0;

        public string path { get; set; }

        public string name { get; set; }

        public DateTime created { get; set; }

        public DateTime modified { get; set; }

        public bool ismine { get; set; }

        public bool thumb { get; set; }

        public long comments { get; set; }

        public bool encrypted { get; set; }

        public string id { get; set; }

        public bool isshared { get; set; }

        public string icon { get; set; }

        public bool isfolder { get; set; }

        public long parentfolderid { get; set; }

        public long? folderid { get; set; }

        public long? fileid { get; set; }

        public long? size { get; set;  }

        public string hash { get; set; }

        public IList<AmazonNode> contents { get; set; }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1300 // Element must begin with upper-case letter
#pragma warning restore SA1600 // Elements must be documented
