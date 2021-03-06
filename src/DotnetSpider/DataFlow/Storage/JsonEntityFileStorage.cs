using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DotnetSpider.DataFlow.Storage
{
    /// <summary>
    /// JSON 文件保存解析(实体)结果
    /// 保存路径: [当前程序运行目录]/files/[任务标识]/[request.hash].data
    /// </summary>
    public class JsonEntityFileStorage : EntityFileStorageBase
    {
        private readonly ConcurrentDictionary<string, StreamWriter> _streamWriters =
            new ConcurrentDictionary<string, StreamWriter>();

        /// <summary>
        /// 根据配置返回存储器
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static JsonEntityFileStorage CreateFromOptions(IConfiguration configuration)
        {
            return new JsonEntityFileStorage();
        }

        protected override async Task StorageAsync(DataFlowContext context, TableMetadata tableMetadata, IList data)
        {
            var streamWriter = _streamWriters.GetOrAdd(tableMetadata.TypeName,
                s => OpenWrite(context, tableMetadata, "json"));
            await streamWriter.WriteLineAsync(JsonConvert.SerializeObject(data));
        }

        public override void Dispose()
        {
            foreach (var streamWriter in _streamWriters)
            {
                streamWriter.Value.Dispose();
            }

            base.Dispose();
        }
    }
}
