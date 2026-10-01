using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Sponsors
{
    internal sealed class GoldSponsorService
    {
        private const int GoldSponsorAmount = 200;
        private static readonly Uri SponsorsJsonUri = 
            new Uri(
                "https://raw.githubusercontent.com/" +
                "MobiFlight/mobiflight-website/main/" +
                "astro/src/data/sponsors.json");

        private static readonly Uri PublicAssetsBaseUri =
            new Uri(
                "https://raw.githubusercontent.com/" +
                "MobiFlight/mobiflight-website/main/" +
                "astro/public/");

        private static readonly HttpClient SharedHttpClient =
            CreateHttpClient();

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        private readonly HttpClient httpClient;
        private readonly string cacheFilePath;

        public GoldSponsorService()
            : this(
                SharedHttpClient,
                GetDefaultCacheFilePath())
        {
        }

        internal GoldSponsorService(
            HttpClient httpClient,
            string cacheFilePath)
        {
            this.httpClient =
                httpClient
                ?? throw new ArgumentNullException(
                    nameof(httpClient));

            this.cacheFilePath =
                cacheFilePath
                ?? throw new ArgumentNullException(
                    nameof(cacheFilePath));
        }

        public IReadOnlyList<GoldSponsor> GetCachedSponsors()
        {
            var cache = LoadCache();

            if (cache?.Sponsors != null)
            {
                return cache.Sponsors;
            }

            return Array.Empty<GoldSponsor>();
        }

        public async Task<IReadOnlyList<GoldSponsor>> RefreshAsync(
            CancellationToken cancellationToken = default)
        {
            var sourceJson =
                await httpClient.GetStringAsync(
                    SponsorsJsonUri,
                    cancellationToken);

            var websiteSponsors =
                JsonSerializer.Deserialize<List<WebsiteSponsor>>(
                    sourceJson,
                    JsonOptions)
                ?? new List<WebsiteSponsor>();

            var goldSponsors =
                websiteSponsors
                    .Where(sponsor =>
                        sponsor.Amount == GoldSponsorAmount
                        && !string.IsNullOrWhiteSpace(
                            sponsor.Name)
                        && !string.IsNullOrWhiteSpace(
                            sponsor.Logo))
                    .OrderBy(
                        sponsor => sponsor.Name,
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (goldSponsors.Count == 0)
            {
                throw new InvalidDataException(
                    "The sponsor feed did not contain any Gold sponsors.");
            }

            var sourceHash =
                CreateSourceHash(goldSponsors);

            var cache =
                LoadCache();

            // Only the Gold sponsor subset is hashed.
            //
            // A Silver/Bronze sponsor change therefore does not cause
            // all Gold logos to be downloaded again.
            if (cache != null
                && cache.SourceHash == sourceHash
                && cache.Sponsors != null
                && cache.Sponsors.Count > 0)
            {
                return cache.Sponsors;
            }

            var sponsorTasks =
                goldSponsors.Select(
                    sponsor =>
                        CreateSponsorAsync(
                            sponsor,
                            cancellationToken));

            var refreshedSponsors =
                (await Task.WhenAll(sponsorTasks))
                    .ToList();

            var refreshedCache =
                new GoldSponsorCache
                {
                    SourceHash = sourceHash,
                    Sponsors = refreshedSponsors
                };

            await SaveCacheAsync(
                refreshedCache,
                cancellationToken);

            return refreshedSponsors;
        }

        private async Task<GoldSponsor> CreateSponsorAsync(
            WebsiteSponsor sponsor,
            CancellationToken cancellationToken)
        {
            var logoUri =
                CreateLogoUri(sponsor.Logo);

            var sourceLogo =
                await httpClient.GetByteArrayAsync(
                    logoUri,
                    cancellationToken);

            var optimizedLogo =
                SponsorLogoOptimizer.Optimize(
                    sourceLogo,
                    sponsor.Logo);

            var dataUri =
                $"data:{optimizedLogo.MimeType};base64," +
                Convert.ToBase64String(
                    optimizedLogo.Data);

            return new GoldSponsor
            {
                Name = sponsor.Name.Trim(),
                LogoDataUri = dataUri,
                Url = NormalizeSponsorUrl(
                    sponsor.Href)
            };
        }

        private static Uri CreateLogoUri(string logo)
        {
            if (Uri.TryCreate(
                    logo,
                    UriKind.Absolute,
                    out var absoluteUri))
            {
                return absoluteUri;
            }

            return new Uri(
                PublicAssetsBaseUri,
                logo.TrimStart('/'));
        }

        private static string NormalizeSponsorUrl(
            string href)
        {
            if (string.IsNullOrWhiteSpace(href))
            {
                return null;
            }

            if (!Uri.TryCreate(
                    href,
                    UriKind.Absolute,
                    out var uri))
            {
                return null;
            }

            if (uri.Scheme != Uri.UriSchemeHttp
                && uri.Scheme != Uri.UriSchemeHttps)
            {
                return null;
            }

            return uri.ToString();
        }

        private static string CreateSourceHash(
            IEnumerable<WebsiteSponsor> sponsors)
        {
            var fingerprint =
                string.Join(
                    "\n",
                    sponsors.Select(
                        sponsor =>
                            string.Join(
                                "\u001f",
                                sponsor.Name?.Trim()
                                    ?? string.Empty,
                                sponsor.Logo?.Trim()
                                    ?? string.Empty,
                                sponsor.Href?.Trim()
                                    ?? string.Empty,
                                sponsor.Amount.ToString())));

            var hash =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(
                        fingerprint));

            return Convert.ToHexString(hash);
        }

        private GoldSponsorCache LoadCache()
        {
            try
            {
                if (!File.Exists(cacheFilePath))
                {
                    return null;
                }

                var json =
                    File.ReadAllText(
                        cacheFilePath);

                return JsonSerializer.Deserialize<GoldSponsorCache>(
                    json,
                    JsonOptions);
            }
            catch
            {
                // A broken or old cache must never prevent
                // MobiFlight from starting.
                return null;
            }
        }

        private async Task SaveCacheAsync(
            GoldSponsorCache cache,
            CancellationToken cancellationToken)
        {
            var directory =
                Path.GetDirectoryName(
                    cacheFilePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json =
                JsonSerializer.Serialize(
                    cache,
                    JsonOptions);

            var temporaryPath =
                cacheFilePath + ".tmp";

            try
            {
                await File.WriteAllTextAsync(
                    temporaryPath,
                    json,
                    Encoding.UTF8,
                    cancellationToken);

                File.Move(
                    temporaryPath,
                    cacheFilePath,
                    true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static string GetDefaultCacheFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder
                        .LocalApplicationData),
                "MobiFlight",
                "cache",
                "gold-sponsors.json");
        }

        private static HttpClient CreateHttpClient()
        {
            var client =
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromSeconds(10)
                };

            client.DefaultRequestHeaders
                .UserAgent
                .ParseAdd(
                    "MobiFlight-Connector");

            return client;
        }

        private sealed class WebsiteSponsor
        {
            public string Name { get; set; }

            public string Logo { get; set; }

            public string Href { get; set; }

            public int Amount { get; set; }
        }

        private sealed class GoldSponsorCache
        {
            public string SourceHash { get; set; }

            public List<GoldSponsor> Sponsors { get; set; }
                = new List<GoldSponsor>();
        }
    }
}