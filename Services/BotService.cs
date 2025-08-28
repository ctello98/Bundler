using Solnet.Pumpfun;
using Solnet.Wallet;

namespace Bundler.Services
{
    public class BotService
    {
        private readonly SolanaService _sol;
        private readonly WalletService _wallet;

        public BotService(SolanaService sol, WalletService wallet)
        {
            _sol = sol;
            _wallet = wallet;
        }

        public async Task BuyAsync(Account account, string mint, decimal solAmount, int slippage)
        {
            // Instead of ChangePayer, create a new PumpfunClient for each wallet:
            var client = new PumpfunClient(_sol.RpcClient, account);
            await client.Buy(mint, solAmount, slippage);
        }

        public async Task SellAsync(Account account, string mint, decimal tokenAmount)
        {
            var client = new PumpfunClient(_sol.RpcClient, account);
            await client.Sell(mint, tokenAmount);
        }
    }
}
