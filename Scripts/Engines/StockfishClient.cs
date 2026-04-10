using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public sealed class StockfishClient : IDisposable
{
	private readonly string _enginePath;
	private Process _proc;
	private StreamWriter _stdin;
	private Task _readerTask;
	private readonly object _lock = new();
	private readonly StringBuilder _outputBuffer = new();

	public StockfishClient(string enginePath)
	{
		_enginePath = enginePath ?? throw new ArgumentNullException(nameof(enginePath));
	}

	public async Task StartAsync(CancellationToken ct = default)
	{
		if (!System.IO.File.Exists(_enginePath))
			throw new FileNotFoundException("Stockfish executable not found", _enginePath);

		_proc = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = _enginePath,
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
			},
			EnableRaisingEvents = true
		};

		if (!_proc.Start())
			throw new InvalidOperationException("Failed to start Stockfish process.");

		_stdin = _proc.StandardInput;

		// Background reader that appends lines to a buffer
		_readerTask = Task.Run(async () =>
		{
			try
			{
				string line;
				while ((line = await _proc.StandardOutput.ReadLineAsync()) != null)
				{
					lock (_lock) _outputBuffer.AppendLine(line);
				}
			}
			catch { /* process closed */ }
		}, ct);

		// Handshake
		await SendAsync("uci", ct);
		await WaitForAsync("uciok", ct);

		// Ready check
		await SendAsync("isready", ct);
		await WaitForAsync("readyok", ct);
	}

	public async Task SetOptionAsync(string name, string value, CancellationToken ct = default)
	{
		await SendAsync($"setoption name {name} value {value}", ct);
	}

	public async Task NewGameAsync(CancellationToken ct = default)
	{
		await SendAsync("ucinewgame", ct);
		await SendAsync("isready", ct);
		await WaitForAsync("readyok", ct);
	}

	/// <summary>
	/// Get best move for a FEN. You can choose by depth or by fixed movetime (ms).
	/// </summary>
	public async Task<string> GetBestMoveAsync(string fen, int? depth = 15, int? moveTimeMs = null, CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(fen))
			throw new ArgumentException("FEN is empty", nameof(fen));

		// Position
		await SendAsync($"position fen {fen}", ct);
		await SendAsync("isready", ct);
		await WaitForAsync("readyok", ct);

		// Search
		if (moveTimeMs.HasValue)
			await SendAsync($"go movetime {moveTimeMs.Value}", ct);
		else if (depth.HasValue)
			await SendAsync($"go depth {depth.Value}", ct);
		else
			await SendAsync("go depth 15", ct); // default

		// Wait for bestmove
		string bestMove = await WaitForBestMoveAsync(ct);
		return bestMove;
	}

	public async Task ResetEngineAsync(CancellationToken ct = default)
	{
		// Clear buffer
		lock (_lock) _outputBuffer.Clear();

		// Reset engine state
		await SendAsync("ucinewgame", ct);
		await SendAsync("isready", ct);
		await WaitForAsync("readyok", ct);
	}

	private async Task SendAsync(string cmd, CancellationToken ct)
	{
		await _stdin.WriteLineAsync(cmd.AsMemory(), ct);
		await _stdin.FlushAsync();
	}

	private async Task WaitForAsync(string token, CancellationToken ct)
	{
		// Spin until token appears in output
		while (true)
		{
			ct.ThrowIfCancellationRequested();
			string snapshot;
			lock (_lock) snapshot = _outputBuffer.ToString();

			if (snapshot.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
				return;

			await Task.Delay(10, ct);
		}
	}

	private async Task<string> WaitForBestMoveAsync(CancellationToken ct)
	{
		string bestMove = null;
		int consumed = 0;

		while (true)
		{
			ct.ThrowIfCancellationRequested();

			string snapshot;
			lock (_lock) snapshot = _outputBuffer.ToString();

			// Look only at new data since last pass
			int start = consumed;
			if (start < snapshot.Length)
			{
				int newline = start;
				while (newline < snapshot.Length)
				{
					int nextNL = snapshot.IndexOf('\n', newline);
					if (nextNL == -1) break;

					string line = snapshot.Substring(newline, nextNL - newline).Trim();
					if (line.StartsWith("bestmove ", StringComparison.Ordinal))
					{
						var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length >= 2)
						{
							bestMove = parts[1]; // long-algebraic UCI move, e.g. "e2e4" or "e7e8q"
							return bestMove;
						}
					}

					newline = nextNL + 1;
				}
				consumed = newline;
			}

			await Task.Delay(10, ct);
		}
	}

	public void Dispose()
	{
		try { _stdin?.WriteLine("quit"); } catch { }
		try { _stdin?.Flush(); } catch { }
		try { if (!_proc?.HasExited ?? false) _proc?.Kill(true); } catch { }
		try { _stdin?.Dispose(); } catch { }
		try { _proc?.Dispose(); } catch { }
	}
}
