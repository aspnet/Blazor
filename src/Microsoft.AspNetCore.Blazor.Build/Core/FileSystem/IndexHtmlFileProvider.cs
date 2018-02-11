﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Blazor.Internal.Common.FileProviders;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileProviders;
using System.Linq;
using Mono.Cecil;

namespace Microsoft.AspNetCore.Blazor.Build.Core.FileSystem
{
    internal class IndexHtmlFileProvider : InMemoryFileProvider
    {
        public IndexHtmlFileProvider(string htmlTemplate, string assemblyName, IEnumerable<IFileInfo> binFiles, string assemblyPath = null)
            : base(ComputeContents(htmlTemplate, assemblyName, binFiles, assemblyPath))
        {
        }

        private static IEnumerable<(string, byte[])> ComputeContents(string htmlTemplate, string assemblyName, IEnumerable<IFileInfo> binFiles, string assemblyPath = null)
        {
            if (htmlTemplate != null)
            {
                var html = GetIndexHtmlContents(htmlTemplate, assemblyName, binFiles, assemblyPath);
                var htmlBytes = Encoding.UTF8.GetBytes(html);
                yield return ("/index.html", htmlBytes);
            }
        }

        private static string GetIndexHtmlContents(string htmlTemplate, string assemblyName, IEnumerable<IFileInfo> binFiles, string assemblyPath = null)
        {
            // TODO: Instead of inserting the script as the first element in <body>,
            // consider either:
            // [1] Inserting it just before the first <script> in the <body>, so that
            //     developers can still put other <script> elems after (and therefore
            //     reference Blazor JS APIs from them) but we don't block the page
            //     rendering while fetching that script. Note that adding async/defer
            //     alone isn't enough because that doesn't help older browsers that
            //     don't suppor them.
            // [2] Or possibly better, don't insert the <script> magically at all.
            //     Instead, just insert a block of configuration data at the top of
            //     <body> (e.g., <script type='blazor-config'>{ ...json... }</script>)
            //     and then let the developer manually place the tag that loads blazor.js
            //     wherever they want (adding their own async/defer if they want).
            return htmlTemplate
                .Replace("<body>", "<body>\n" + CreateBootMarkup(assemblyName, binFiles, assemblyPath));
        }

        private static string CreateBootMarkup(string assemblyName, IEnumerable<IFileInfo> binFiles, string assemblyPath = null)
        {
            var assemblyNameWithExtension = $"{assemblyName}.dll";
            var referenceNames = binFiles
                .Where(file => !string.Equals(file.Name, assemblyNameWithExtension))
                .Select(file => file.Name);
            var referencesAttribute = string.Join(",", referenceNames.ToArray());

            var entryPoint = "";
            if (assemblyPath != null)
            {
                var asmPath = Path.Combine(assemblyPath);
                var asmDef = AssemblyDefinition.ReadAssembly(asmPath);
                var epMethod = asmDef.EntryPoint;
                if (epMethod != null)
                {
                    var ns = epMethod.DeclaringType.Namespace;
                    var cn = epMethod.DeclaringType.Name;
                    var mn = epMethod.Name;
                    entryPoint = $"@{ns},{cn},{mn}";
                }

            }

            return $"<script src=\"/_framework/blazor.js\"" +
                   $" main=\"{assemblyNameWithExtension}{entryPoint}\"" +
                   $" references=\"{referencesAttribute}\"></script>";
        }
    }
}
