using Solnet.Pumpfun;
using Solnet.Rpc;
using Solnet.Wallet;

namespace Bundler.Services
{
    public class SolanaService
    {
        public IRpcClient RpcClient { get; }
        public PumpfunClient Pumpfun { get; }

        // Now takes an Account payer instead of programId
        public SolanaService(string rpcUrl, Account payer)
        {
            RpcClient = ClientFactory.GetClient(rpcUrl);
            Pumpfun = new PumpfunClient(RpcClient, payer);
        }
    }
}
