using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using type_lookup_service.Utils;

namespace type_lookup_service.Repositories
{
    public class JsonRepository : IJsonRepository
    {
        private readonly string _fileLocation;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IRequestContextAccessor _requestAccessor;

        public JsonRepository(
            IConfiguration config,
            IContextLogger<JsonRepository> logger,
            IMemoryCache memoryCache,
            IRequestContextAccessor requestContextAccessor)
        {
            _fileLocation = config.GetValue<string>("TypeLookup:FileLocation");
            _memoryCache = memoryCache;
            _logger = logger;
            _requestAccessor = requestContextAccessor;
        }

        public (JArray jsonArray, string typeName) GetSegmentData(string typeName)
        {
            _logger.LogTrace("JsonRepository:GetSegmentData - Method Begins");

            var fileName = $"{Path.DirectorySeparatorChar}{typeName}.json";
            var fileNames = GetFileNamesFromCache();
            var file = fileNames.FirstOrDefault(a => a.ToLower().Contains(fileName.ToLower()));

            if (file != null)
            {
                var result = GetFileData(file);
                return (result, ToCamelCase(Path.GetFileNameWithoutExtension(fileName)));
            }

            return (new JArray(), ToCamelCase(typeName));
        }

        private List<string> GetFileNamesFromCache()
        {
            var request = _requestAccessor?.RequestContext;
            var tenantId = request?.TenantId.ToString() ?? string.Empty;
            var segmentId = request?.SegmentId.ToString() ?? string.Empty;

            if (_memoryCache.TryGetValue(segmentId.ToLower(), out List<string> fileNames))
            {
                return fileNames;
            }
            else
            {
                var directoryPath = Path.Combine(_fileLocation, tenantId.ToLower(), segmentId.ToLower());
                var allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).ToList();
                _memoryCache.Set(segmentId.ToLower(), allFiles);

                return allFiles;
            }
        }

        private JArray GetFileData(string filePath)
        {
            _logger.LogTrace("JsonRepository:GetFileData - Method Begins: {File}", filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogInformation("Type file '{File}' does not exist.", filePath);
                return JArray.Parse("[]");
            }

            _logger.LogTrace("JsonRepository:GetFileData - Method Successfully Completed");

            using (var s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        var jsonResult = JArray.Load(reader);
                        return jsonResult;
                    }
                }
            }

            return JArray.Parse("[]");
        }

        private static string ToCamelCase(string oldStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return oldStr;
            }

            var str = oldStr;
            if (oldStr.Length == 1)
            {
                return char.ToLowerInvariant(str[0]).ToString();
            }

            str = char.ToLowerInvariant(str[0]) + str.Substring(1);
            str = str.Replace(' ', '_');
            return str;

        }

        public (JArray jsonArray, string typeName) GetTypeLookupData(string typeName, string primaryKeyName, string primaryKeyValue)
        {
            _logger.LogTrace("JsonRepository:GetTypeLookupData - Method Begins");

            var fileName = $"{Path.DirectorySeparatorChar}{typeName}.json";
            var fileNames = GetFileNamesFromCache();
            var file = fileNames.FirstOrDefault(a => a.ToLower().Contains(fileName.ToLower()));

            if (file != null)
            {
                var command = $"jq '[.data[]  | select(.{primaryKeyName}== \"{primaryKeyValue}\")]' {file}";

                var result = ShellHelper.ExecuteBashCommand(command);

                if (result.ExitCode != 0)
                {
                    _logger.LogError(result.ErrorMessage);
                    return (new JArray(), ToCamelCase(typeName));
                }
                else
                {
                    return (JArray.Parse(result.Json), ToCamelCase(typeName));
                }
            }

            return (new JArray(), ToCamelCase(typeName));
        }
    }
}