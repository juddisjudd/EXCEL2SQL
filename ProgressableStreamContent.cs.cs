using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class ProgressableStreamContent : HttpContent
{
    private readonly HttpContent _content;
    private readonly int _bufferSize;
    private readonly Action<long, long> _progress;

    public ProgressableStreamContent(HttpContent content, int bufferSize, Action<long, long> progress)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
        _bufferSize = bufferSize;
        _progress = progress;

        foreach (var header in content.Headers)
        {
            Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        var buffer = new byte[_bufferSize];
        long totalBytesRead = 0;

        using (var inputStream = await _content.ReadAsStreamAsync())
        {
            int bytesRead;
            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                totalBytesRead += bytesRead;
                _progress?.Invoke(totalBytesRead, inputStream.Length);
                await stream.WriteAsync(buffer, 0, bytesRead);
            }
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }
}
