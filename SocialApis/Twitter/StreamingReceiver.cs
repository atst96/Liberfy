using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using formatters = SocialApis.Twitter.StreamJsonFormatters;

namespace SocialApis.Twitter
{
    internal class StreamingReceiver : IDisposable
    {
        private const int BufferSize = 4096;

        private Stream _stream;
        private IObserver<IStreamResponse> _observer;

        public StreamingReceiver(Stream stream, IObserver<IStreamResponse> observer)
        {
            this._stream = stream;
            this._observer = observer;
        }

        public void Start()
        {
            using (var str = this._stream)
            {
                const byte UTF8_LF = 0x0A;
                const byte UTF8_CR = 0x0D;
                const byte UTF8_NULL = 0x00;

                var ms = CreateMemoryStream();

                bool continueStream = true;

                byte[] buffer = new byte[BufferSize];
                byte cByte;

                int copySize, bufferOffset, bufferIndex;
                int bufferedCount;

                while (continueStream)
                {
                    bufferOffset = 0;
                    bufferedCount = str.Read(buffer, 0, BufferSize);

                    for (bufferIndex = 0; bufferIndex < bufferedCount; ++bufferIndex)
                    {
                        cByte = buffer[bufferIndex];

                        if (cByte == UTF8_NULL)
                        {
                            bufferOffset = bufferIndex + 1;
                            continue;
                        }
                        else if (cByte == UTF8_LF || cByte == UTF8_CR)
                        {
                            copySize = bufferIndex - bufferOffset;
                            if (copySize > 0)
                                ms.Write(buffer, bufferOffset, copySize);
                            else if (copySize != 0)
                                throw new IndexOutOfRangeException();

                            if (ms.Length > 0)
                            {
                                using (ms)
                                    this.OnNext(ms.ToArray());

                                ms = CreateMemoryStream();
                            }

                            bufferOffset = bufferIndex + 1;
                            continue;
                        }
                    }

                    copySize = bufferedCount - bufferOffset;
                    if (continueStream && copySize > 0)
                    {
                        ms.Write(buffer, bufferOffset, copySize);
                    }
                }
            }
        }

        private void OnNext(byte[] data)
        {
            IStreamResponse res = null;

            var reader = new JsonReader(data);

            if (reader.ReadIsBeginObject())
            {
                int count = 0;

                if (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    try
                    {
                        var propName = reader.ReadPropertyName();

                        if (propName == "delete")
                            res = DeserializeJsonData(ref reader, formatters.Delete);
                        else if (propName == "scrub_geo")
                            res = DeserializeJsonData(ref reader, formatters.ScrubGeo);
                        else if (propName == "status_withheld")
                            res = DeserializeJsonData(ref reader, formatters.StatusWithheld);
                        else if (propName == "user_withheld")
                            res = DeserializeJsonData(ref reader, formatters.UserWithheld);
                        else if (propName == "disconnect")
                            res = DeserializeJsonData(ref reader, formatters.Disconnect);
                        else if (propName == "warning")
                            res = DeserializeJsonData(ref reader, formatters.Warning);
                        else if (propName == "friends")
                            res = DeserializeJsonData<FriendsStreamResponse>(data);
                        else if (propName == "for_user")
                            res = DeserializeJsonData<EnvelopesStreamResponse>(data);
                        else if (propName == "control")
                            res = DeserializeJsonData(ref reader, formatters.Control);
                        else if (propName == "direct_message")
                            res = DeserializeJsonData(ref reader, formatters.DirectMessage);
                        else if (propName == "event")
                        {
                            reader.ReadNextBlock();

                            var eRes = DeserializeJsonData<EventStreamResponse>(data);

                            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                            {
                                if (reader.ReadPropertyName() == "target_object")
                                {
                                    if (eRes.EventType.HasFlag(EventType.Tweet))
                                        eRes.TargetStauts = DeserializeJsonData(ref reader, formatters.StatusObject);
                                    else if (eRes.EventType.HasFlag(EventType.List))
                                        eRes.TargetList = DeserializeJsonData(ref reader, formatters.ListObject);

                                    break;
                                }

                                reader.ReadNextBlock();
                            }

                            res = eRes;
                        }
                        else
                        {
                            reader.ReadNextBlock();

                            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                            {
                                propName = reader.ReadPropertyName();

                                if (propName == "text")
                                    res = DeserializeJsonData<StatusStreamResponse>(data);
                                else if (propName == "event")
                                    res = DeserializeJsonData<EventStreamResponse>(data);

                                if (res != null)
                                    break;

                                reader.ReadNextBlock();
                            }

                            res = res ?? new RawJsonStreamResponse(Encoding.UTF8.GetString(data));
                        }
                    }
                    catch (Exception ex)
                    {
                        var str = Encoding.UTF8.GetString(data);
                        Console.WriteLine(str);
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            if (res != null)
                _observer.OnNext(res);

            reader = default(JsonReader);
        }

        private static T DeserializeJsonData<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, formatters.Resolver);
        }

        private static T DeserializeJsonData<T>(ref JsonReader reader, IJsonFormatter<T> jsonFormatter)
        {
            return jsonFormatter.Deserialize(ref reader, formatters.Resolver);
        }

        private static bool IsArrayBegin(byte[] source, byte[] signal)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if (source.Length < signal.Length)
                return false;

            for (int i = 0; i < signal.Length; ++i)
            {
                if (source[i] != signal[i])
                    return false;
            }

            return true;
        }

        private static bool IsArrayWithin(byte[] source, byte[] signal)
        {
            if (source?.Length == 0)
                throw new ArgumentException(nameof(source));

            if (signal?.Length == 0)
                throw new ArgumentException(nameof(signal));

            for (int sourceIndex = 0, m = source.Length - signal.Length; sourceIndex < m; ++sourceIndex)
            {
                if (source[sourceIndex] == signal[0])
                {
                    bool found = true;

                    for (int signalIndex = 0; signalIndex < signal.Length; ++signalIndex)
                    {
                        if (source[sourceIndex + signalIndex] != signal[sourceIndex])
                        {
                            found = false;
                            continue;
                        }
                    }

                    if (found)
                        return true;
                }
            }

            return false;
        }

        private static MemoryStream CreateMemoryStream() => new MemoryStream(BufferSize);

        public void Dispose() => _stream?.Dispose();
    }
}
