using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Diagnostics;

using WinNetworkProcessMonitor.Models;

namespace WinNetworkProcessMonitor.Handlers
{
    public class ProcessEnumeration
    {

        /// Initialize process pid path cache.
        private static readonly MemoryCache _processCache = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromSeconds(30)
        });

        /// Initialize FileHash hash object cach.
        private static readonly MemoryCache _hashCache = new MemoryCache(new MemoryCacheOptions()
        {
            ExpirationScanFrequency = TimeSpan.FromSeconds(300)
        });

        /// <summary>
        /// Retrives a path for a pid from _process cache
        /// Enumeates the pid if cache entry does not exist
        /// Sets a cache entry with 30 seconds
        /// </summary>
        /// <param name="pid">Process Id</param>
        /// <returns>string</returns>
        public static string GetProcessPath(int pid)
        {
            //Console.WriteLine("processcache count: {0}", _processCache.Count);

            if (!_processCache.TryGetValue(pid, out string cacheEntry))// Look for cache key.
            {
                cacheEntry = EnumProcessPath(pid);

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30)
                };
                _processCache.Set(pid, cacheEntry, options);

            }
            return cacheEntry;
        }

        /// <summary>
        /// Returns a path string, or "" if inaccessiable for any reason
        /// </summary>
        /// <param name="pid">Process Id</param>
        /// <returns>string</returns>
        public static string EnumProcessPath(int pid)
        {
            string processName;

            try
            {
                var processModule = Process.GetProcessById(pid).MainModule;

                if (processModule is not null)
                {
                    var filename = processModule.FileName;
                    if (filename is not null)
                    {
                        processName = filename;
                    }
                    else
                    {
                        processName = "";
                    }
                }
                else
                {
                    processName = "";
                }
            }
            catch
            {
                processName = "";
            }

            return processName;

        }


        // Initize private hashing algorithms
#pragma warning disable CS8601 // Possible null reference assignment.
        private static readonly HashAlgorithm _md5Hasher = HashAlgorithm.Create("MD5");
        private static readonly HashAlgorithm _sha256Hasher = HashAlgorithm.Create("SHA256");
        private static readonly HashAlgorithm _sha512Hasher = HashAlgorithm.Create("SHA512");
#pragma warning restore CS8601 // Possible null reference assignment.


        /// <summary>
        /// Enumerates a filepath for cryptographic hashes
        /// </summary>
        /// <param name="path"></param>
        /// <returns>NetProcMon.Models.FileHash</returns>
        public static Filename EnumHashes(string path)
        {

            Filename FileInfo = new Filename
            {
                Path = path
            };

            try
            {
                using var stream = System.IO.File.OpenRead(path);

                var md5 = _md5Hasher.ComputeHash(stream);
                var sha256 = _sha256Hasher.ComputeHash(stream);
                var sha512 = _sha512Hasher.ComputeHash(stream);

                Hashes hashes = new Hashes
                {
                    Md5 = BitConverter.ToString(md5).Replace("-", ""),
                    Sha256 = BitConverter.ToString(sha256).Replace("-", ""),
                    Sha512 = BitConverter.ToString(sha512).Replace("-", "")
                };
                FileInfo.Hashes = hashes;
            }
            catch { }

            return FileInfo;
        }

        /// <summary>
        /// Returns a FileHash Object
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Filename GetFilepathHashes(string filePath)
        {
            if (!_hashCache.TryGetValue(filePath, out Filename cacheEntry))
            {
                var entry = EnumHashes(filePath);

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(300)
                };

                _hashCache.Set(filePath, entry, options);

            }
            return cacheEntry;
        }

    }
}

