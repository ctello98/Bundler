using Solnet.Programs;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Types;
using Solnet.Wallet;

namespace Bundler.Services
{
    public class FundingService
    {
        public readonly SolanaService _sol;
        private readonly WalletService _wallet;

        public FundingService(SolanaService sol, WalletService wallet)
        {
            _sol = sol;
            _wallet = wallet;
        }

        public async Task<(decimal TotalTransfer, decimal TotalFees)> FundWalletsAsync(
            Account primary, IEnumerable<Account> recipients, decimal solAmount)
        {
            const ulong LAMPORTS_PER_SOL = 1000000000;
            // get fee per signature
            var recent = await _sol.RpcClient.GetLatestBlockHashAsync();



            ulong transferLamports = (ulong)(solAmount * LAMPORTS_PER_SOL);
            ulong totalFees = 0;

            foreach (var recv in recipients)
            {
                // build and send transfer
                var tx = new TransactionBuilder()
                    .SetRecentBlockHash(recent.Result.Value.Blockhash)
                    .SetFeePayer(primary)
                    .AddInstruction(
                        SystemProgram.Transfer(
                            primary.PublicKey,
                            recv.PublicKey,
                            transferLamports))
                    .Build(primary);

                var sig = await _sol.RpcClient.SendTransactionAsync(
                    tx, false, Commitment.Confirmed);
      
            }

            var totalTransferSol = (decimal)transferLamports * recipients.Count() / LAMPORTS_PER_SOL;
            var totalFeesSol = (decimal)totalFees / LAMPORTS_PER_SOL;
            return (totalTransferSol, totalFeesSol);
        }
    }
}
