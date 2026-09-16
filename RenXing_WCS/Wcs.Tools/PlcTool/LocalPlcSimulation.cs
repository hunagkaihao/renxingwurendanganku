using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wcs.PlcTool;

/// <summary>
/// WCS 本地 PLC 点位。与真实 PLC Redis 完全隔离，仍返回原协议的命令号、JobId 和执行状态。
/// </summary>
public sealed class LocalPlcSimulation : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, PlcTagValue> _tags = new();
    private readonly Dictionary<string, List<PlcTagValueChanged>> _subscribers = new();
    private readonly Dictionary<string, PendingCommand> _pending = new();
    private readonly Timer _timer;
    private readonly int _delayMs;
    private readonly Action<Exception> _onError;
    private bool _disposed;

    private sealed class PendingCommand
    {
        public string Tag;
        public byte[] Frame;
        public DateTime Due;
        public Queue<(int Section, int Column)> Cells;
        public bool AwaitingCell;
    }

    public LocalPlcSimulation(int delayMs, Action<Exception> onError)
    {
        _delayMs = Math.Max(50, delayMs);
        _onError = onError;
        Add("Lm_Cmd", EnumPlcTagType.U8Array, Frame(26));
        Add("Lm_Response", EnumPlcTagType.U8Array, Frame(12));
        foreach (string name in new[] { "Lm_State", "Lm_Zero", "Lm_SafePos", "Mjj_SafePos" })
            Add(name, EnumPlcTagType.U16, "1");
        Add("Mover_Cmd", EnumPlcTagType.U8Array, Frame(8));
        Add("Mover_Response", EnumPlcTagType.U8Array, Frame(8));
        Add("Mover_Pos", EnumPlcTagType.U16, "2");
        foreach (string name in new[] { "CellChkFinished", "AllCheckFinished", "SectionNoChked", "ColNoChked", "BarcodeChked", "WcsReceived" })
            Add(name, EnumPlcTagType.U32, "0");
        foreach (string name in new[] { "EmergencyStop", "HeartBeatFromPlc", "HeartBeatToPlc" })
            Add(name, EnumPlcTagType.Bit, "False");
        for (int i = 1; i <= 8; i++)
        {
            Add($"Door{i}_Cmd", EnumPlcTagType.U8Array, Frame(12));
            Add($"Door{i}_Response", EnumPlcTagType.U8Array, Frame(12));
            Add($"Cmd_1200{i}", EnumPlcTagType.Bit, "False");
            Add($"Status_1200{i}", EnumPlcTagType.Bit, "False");
        }
        _timer = new Timer(Tick, null, 50, 50);
    }

    private static string Frame(int length) => Encoding.Latin1.GetString(new byte[length]);
    private void Add(string name, EnumPlcTagType type, string value) => _tags.Add(name,
        new PlcTagValue(new PlcTag(name, $"Simulation:{name}", type, EnumTagAccess.ReadWrite, true))
        { Value = value, Quality = EnumQuality.Good });

    public bool Exists(string plc, string tag)
    {
        lock (_gate) return !_disposed && plc == "Plc1" && _tags.ContainsKey(tag);
    }

    public PlcTagValue Read(string plc, string tag)
    {
        lock (_gate) return Exists(plc, tag) ? Copy(_tags[tag]) : null;
    }

    private static PlcTagValue Copy(PlcTagValue value) => new(value.Tag)
    { Value = value.Value, Quality = value.Quality, TimeStamp = value.TimeStamp };

    public bool Subscribe(string plc, string tag, PlcTagValueChanged handler)
    {
        lock (_gate)
        {
            if (!Exists(plc, tag)) return false;
            if (!_subscribers.TryGetValue(tag, out var handlers))
                _subscribers[tag] = handlers = new List<PlcTagValueChanged>();
            handlers.Add(handler);
            return true;
        }
    }

    // 点位更新和通知在同一锁内按序进行；读返回副本，避免消费者改写设备状态。
    private void Set(string tag, string value)
    {
        if (_tags[tag].Value == value) return;
        _tags[tag].Value = value;
        _tags[tag].TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (!_subscribers.TryGetValue(tag, out var handlers)) return;
        foreach (var handler in handlers.ToArray())
        {
            try { handler("Plc1", tag, Copy(_tags[tag])); }
            catch (Exception ex) { _onError?.Invoke(ex); }
        }
    }

    public bool Write(string plc, string tag, string value, IEnumerable<(int Section, int Column)> cells = null)
    {
        lock (_gate)
        {
            if (!Exists(plc, tag) || value == null) return false;
            if (tag.EndsWith("_Cmd", StringComparison.Ordinal))
            {
                byte[] frame = Encoding.Latin1.GetBytes(value);
                int expected = tag == "Lm_Cmd" ? 26 : tag == "Mover_Cmd" ? 8 : 12;
                if (frame.Length != expected) return false;
                int command = (frame[0] << 8) | frame[1];
                bool supported = tag == "Lm_Cmd"
                    ? new[] { 1, 2, 3, 4, 5, 7, 8, 11, 12 }.Contains(command)
                    : tag == "Mover_Cmd" ? command == 11 || command == 12 : command == 10;
                if (!supported) return false;
                var scan = command == 4 && tag == "Lm_Cmd" ? cells?.ToList() : null;
                if (command == 4 && tag == "Lm_Cmd" && (scan == null || scan.Count == 0)) return false;
                if (_pending.ContainsKey(tag)) return _tags[tag].Value == value;
                Set(tag, value);
                var pending = new PendingCommand
                {
                    Tag = tag, Frame = frame, Due = DateTime.UtcNow.AddMilliseconds(_delayMs),
                    Cells = scan == null ? null : new Queue<(int, int)>(scan)
                };
                _pending[tag] = pending;
                if (tag == "Lm_Cmd")
                {
                    Set("Lm_State", "0");
                    Set("Lm_Zero", "0");
                    Set("Lm_SafePos", "0");
                }
                Respond(pending, 1);
                return true;
            }
            if (tag.StartsWith("Cmd_", StringComparison.Ordinal))
            {
                if (!bool.TryParse(value, out bool open)) return false;
                Set(tag, open.ToString());
                if (open)
                    _pending[tag] = new PendingCommand { Tag = tag, Due = DateTime.UtcNow.AddMilliseconds(_delayMs) };
                return true;
            }
            if (_tags[tag].Tag.TagType == EnumPlcTagType.Bit && !bool.TryParse(value, out _)) return false;
            Set(tag, value);
            return true;
        }
    }

    private void Respond(PendingCommand command, byte status)
    {
        byte[] response = new byte[command.Tag == "Mover_Cmd" ? 8 : 12];
        Array.Copy(command.Frame, response, 4);
        response[5] = status;
        if (command.Tag == "Lm_Cmd" && command.Frame[1] != 4)
            Array.Copy(command.Frame, 20, response, 6, 4);
        Set(command.Tag.Replace("_Cmd", "_Response"), Encoding.Latin1.GetString(response));
    }

    private void Tick(object state)
    {
        lock (_gate)
        {
            if (_disposed) return;
            try
            {
                // 模拟设备心跳随时间跳变，无需 PLCServer。
                Set("HeartBeatFromPlc", ((DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) % 2 == 0).ToString());
                foreach (var command in _pending.Values.ToArray())
                {
                    if (command.AwaitingCell || command.Due > DateTime.UtcNow) continue;
                    if (command.Tag.StartsWith("Cmd_", StringComparison.Ordinal))
                    {
                        Set(command.Tag.Replace("Cmd_", "Status_"), "True");
                        Set(command.Tag, "False");
                        _pending.Remove(command.Tag);
                        continue;
                    }
                    if (command.Cells != null && command.Cells.Count > 0)
                    {
                        var cell = command.Cells.Dequeue();
                        Set("SectionNoChked", cell.Section.ToString());
                        Set("ColNoChked", cell.Column.ToString());
                        Set("BarcodeChked", "0"); // 明确的模拟空位，不冒用 WMS 账面库存。
                        Set("CellChkFinished", _tags["CellChkFinished"].Value == "1" ? "2" : "1");
                        command.AwaitingCell = true;
                        continue;
                    }
                    Respond(command, 2);
                    if (command.Tag == "Lm_Cmd")
                    {
                        Set("Lm_State", "1");
                        if (command.Frame[1] == 1) Set("Lm_Zero", "1");
                        if (command.Frame[1] == 1 || command.Frame[1] == 5) Set("Lm_SafePos", "1");
                        if (command.Cells != null)
                            Set("AllCheckFinished", _tags["AllCheckFinished"].Value == "1" ? "2" : "1");
                    }
                    else if (command.Tag == "Mover_Cmd") Set("Mover_Pos", command.Frame[1] == 11 ? "1" : "2");
                    else if (command.Tag.StartsWith("Door", StringComparison.Ordinal))
                        Set($"Status_1200{command.Tag[4]}", "True");
                    _pending.Remove(command.Tag);
                }
            }
            catch (Exception ex) { _onError?.Invoke(ex); }
        }
    }

    // 盘点结果落库后才推进下一库位，避免轮询慢时覆盖未消费的扫描结果。
    public void AcknowledgeCheckCell()
    {
        lock (_gate)
        {
            if (_pending.TryGetValue("Lm_Cmd", out var command) && command.AwaitingCell)
            {
                command.AwaitingCell = false;
                command.Due = DateTime.UtcNow.AddMilliseconds(_delayMs);
            }
        }
    }

    public void StopCheck()
    {
        lock (_gate)
        {
            if (_pending.TryGetValue("Lm_Cmd", out var command) && command.Cells != null)
            {
                _pending.Remove("Lm_Cmd");
                Set("Lm_State", "1");
            }
        }
    }

    public void Dispose()
    {
        lock (_gate) { _disposed = true; _pending.Clear(); _subscribers.Clear(); }
        _timer.Dispose();
    }
}
