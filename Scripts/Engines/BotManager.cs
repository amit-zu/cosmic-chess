using System.Threading.Tasks;
using Godot;

public partial class BotManager : Node
{
	private int depth = 15; // default depth
	private static readonly string STOCKFISH_PATH =
	 "C:\\Users\\user\\Desktop\\Projects\\Private\\Cosmic Chess\\Engines\\Stockfish\\stockfish-windows-x86-64-avx2.exe"; //TODO add linux support

	private StockfishClient _stockfishClient;

	public override async void _Ready()
	{
		_stockfishClient = new StockfishClient(STOCKFISH_PATH);
		await _stockfishClient.StartAsync();

		GD.Print("Stockfish is ready.");
	}

	public async void SetEloLevel(int eloLevel)
	{
		if (eloLevel >= 1320 && eloLevel <= 3190)
		{
			await _stockfishClient.SetOptionAsync("UCI_LimitStrength", "true");
			await _stockfishClient.SetOptionAsync("UCI_Elo", eloLevel.ToString());
		}
		else if (eloLevel < 1320)
		{
			await _stockfishClient.SetOptionAsync("UCI_LimitStrength", "true");

			if (eloLevel < 800)
			{
				SetDepth(1);
			}
			else if (eloLevel >= 800 && eloLevel < 1100)
			{
				SetDepth(2);
			}
			else if (eloLevel >= 1100 && eloLevel < 1320)
			{
				SetDepth(3);
			}
			else
			{
				GD.Print("Error: Elo level is out of range.");
			}
		}
		else
		{
			GD.Print("Error: Elo level is out of range.");
		}
	}

	public async Task<string> GetMove(string fenString)
	{
		await _stockfishClient.ResetEngineAsync();
		var bestMove = await _stockfishClient.GetBestMoveAsync(fenString, depth: depth);
		return bestMove;
	}

	public override void _ExitTree()
	{
		_stockfishClient?.Dispose();
		base._ExitTree();
	}

	private void SetDepth(int depth)
	{
		this.depth = depth;
	}
}
