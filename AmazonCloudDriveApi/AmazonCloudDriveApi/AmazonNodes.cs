// <copyright file="AmazonNodes.cs" company="Rambalac">
// Copyright (c) Rambalac. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azi.Amazon.CloudDrive.JsonObjects;
using Azi.Tools;
using System.Web;
using Newtonsoft.Json;

namespace Azi.Amazon.CloudDrive
{
    /// <summary>
    /// Part to work with files tree nodes
    /// </summary>
    public partial class AmazonDrive
    {
        private static readonly Regex FilterEscapeChars = new Regex("[ \\+\\-&|!(){}[\\]^'\"~\\*\\?:\\\\]");
        private AmazonNode root;

        /// <inheritdoc/>
        async Task IAmazonNodes.Add(string parentid, string nodeid)
        {
            var url = string.Format("{0}/nodes/{1}/children/{2}", await GetMetadataUrl().ConfigureAwait(false), parentid, nodeid);
            await http.Send<object>(HttpMethod.Put, url).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.CreateFolder(string parentId, string name)
        {
            var url = BuildMethodUrl("createfolder");
            name = HttpUtility.UrlEncode(name.Replace(@"\", "/"));
            var result = await http.GetJsonAsync<dynamic>($"{url}&folderid={parentId}&name={name}").ConfigureAwait(false);

            if (result.error != null)
            {
                if (result.result == 2004)
                {
                    throw new HttpWebException((string)result.error + " (while trying to create " + name + ")", System.Net.HttpStatusCode.Conflict);
                }

                throw new HttpWebException((string)result.error + " (while trying to create " + name + ")", System.Net.HttpStatusCode.InternalServerError);
            }

            var json = JsonConvert.SerializeObject(result);
            Children typedResult = JsonConvert.DeserializeObject<Children>(json);

            return typedResult.metadata;
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.GetChild(string parentid, string name)
        {
            if (parentid == null)
            {
                parentid = (await GetRoot().ConfigureAwait(false)).id;
            }

            var url = string.Format("{0}nodes?filters={1} AND {2}", await GetMetadataUrl().ConfigureAwait(false), MakeParentFilter(parentid), MakeNameFilter(name));
            var result = await http.GetJsonAsync<Children>(url).ConfigureAwait(false);
            if (result.result == 0)
            {
                return null;
            }

            if (result.result != 1)
            {
                throw new InvalidOperationException("Duplicated node name");
            }

            // if (!result.data[0].parents.Contains(parentid))
            // {
            //     return null; // Hack for wrong Amazon output when file location was changed recently
            // }

            // return result.data[0];
            return null;
        }

        /// <inheritdoc/>
        async Task<IList<AmazonNode>> IAmazonNodes.GetChildren(string id)
        {
            if (id == null)
            {
                id = (await GetRoot().ConfigureAwait(false)).id;
            }

            var baseurl = string.Format("{0}nodes/{1}/children", await GetMetadataUrl().ConfigureAwait(false), id);
            var result = new List<AmazonNode>();
            string nextToken = null;
            do
            {
                var url = string.IsNullOrWhiteSpace(nextToken) ? baseurl : baseurl + "?startToken=" + nextToken;
                try
                {
                    var children = await http.GetJsonAsync<Children>(url).ConfigureAwait(false);
                    // result.AddRange(children.data.Where(n => n.parents.Contains(id))); // Hack for wrong Amazon output when file location was changed recently
                    // nextToken = children.nextToken;
                }
                catch (HttpWebException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        break;
                    }

                    throw;
                }
            }
            while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.GetNode(string id)
        {
            var url = "{0}nodes/{1}";
            var result = await http.GetJsonAsync<AmazonNode>(string.Format(url, await GetMetadataUrl().ConfigureAwait(false), id)).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.GetNodeByMD5(string md5)
        {
            var url = string.Format("{0}nodes?filters={1}", await GetMetadataUrl().ConfigureAwait(false), MakeMD5Filter(md5));
            var result = await http.GetJsonAsync<Children>(url).ConfigureAwait(false);
            // if (result.count == 0)
            // {
                return null;
            // }

            // return result.data[0];
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.GetNodeExtended(string id)
        {
            var url = "{0}nodes/{1}?asset=ALL&tempLink=true";
            var result = await http.GetJsonAsync<AmazonNode>(string.Format(url, await GetMetadataUrl().ConfigureAwait(false), id)).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.GetRoot()
        {
            return await GetRoot().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.Move(string nodeid, string newDirId, bool isDir)
        {
            AmazonNode result;

            if (isDir)
            {
                var url = BuildMethodUrl("renamefolder");
                var apiResult = await http.GetJsonAsync<Children>($"{url}&folderid={nodeid}&tofolderid={newDirId}").ConfigureAwait(false);
                result = apiResult.metadata;
            }
            else
            {
                var url = BuildMethodUrl("renamefile");
                var apiResult = await http.GetJsonAsync<Children>($"{url}&fileid={nodeid}&tofolderid={newDirId}").ConfigureAwait(false);
                result = apiResult.metadata;
            }

            return result;
        }

        /// <inheritdoc/>
        async Task IAmazonNodes.Remove(string nodeid, bool isDir)
        {
            if (isDir)
            {
                var url = BuildMethodUrl("deletefolderrecursive");
                await http.GetJsonAsync<DeleteRecursiveResult>($"{url}&folderid={nodeid}").ConfigureAwait(false);
            }
            else
            {
                var url = BuildMethodUrl("deletefile");
                await http.GetJsonAsync<Children>($"{url}&fileid={nodeid}").ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        async Task<AmazonNode> IAmazonNodes.Rename(string nodeid, string newName, bool isDir)
        {
            AmazonNode result;

            if (isDir)
            {
                var url = BuildMethodUrl("renamefolder");
                var apiResult = await http.GetJsonAsync<Children>($"{url}&folderid={nodeid}&toname={newName}").ConfigureAwait(false);
                result = apiResult.metadata;
            }
            else
            {
                var url = BuildMethodUrl("renamefile");
                var apiResult = await http.GetJsonAsync<Children>($"{url}&fileid={nodeid}&toname={newName}").ConfigureAwait(false);
                result = apiResult.metadata;
            }

            return result;
        }

        /// <inheritdoc/>
        async Task IAmazonNodes.Trash(string nodeid, bool isDir)
        {
            if (isDir)
            {
                var url = BuildMethodUrl("deletefolderrecursive");
                await http.GetJsonAsync<DeleteRecursiveResult>($"{url}&folderid={nodeid}").ConfigureAwait(false);
            }
            else
            {
                var url = BuildMethodUrl("deletefile");
                await http.GetJsonAsync<Children>($"{url}&fileid={nodeid}").ConfigureAwait(false);
            }
        }

        private static string MakeMD5Filter(string md5) => "contentProperties.md5:" + md5;

        private static string MakeNameFilter(string name) => "name:" + Uri.EscapeDataString(FilterEscapeChars.Replace(name, "\\$0"));

        private static string MakeParentFilter(string id) => "parents:" + id;

        private async Task<AmazonNode> GetRoot()
        {
            if (root != null)
            {
                return root;
            }

            var url = BuildMethodUrl("listfolder");
            var result = await http.GetJsonAsync<Children>($"{url}&folderid=0").ConfigureAwait(false);

            if (result.result != 0 || result.metadata == null)
            {
                throw new InvalidOperationException("Could not retrieve root");
            }

            root = result.metadata;

            return root;
        }

        async Task<AmazonNode> IAmazonNodes.GetNodeByPath(string path, bool isDir)
        {
            string url;
            if (isDir)
            {
                url = BuildMethodUrl("listfolder");
            }
            else
            {
                url = BuildMethodUrl("checksumfile");
            }

            path = HttpUtility.UrlEncode(path.Replace(@"\", "/"));
            var result = await http.GetJsonAsync<Children>($"{url}&path={path}").ConfigureAwait(false);

            if (result.result != 0 || result.metadata == null)
            {
                // throw new InvalidOperationException("Could not retrieve node information");
                return null;
            }

            return result.metadata;
        }
    }
}