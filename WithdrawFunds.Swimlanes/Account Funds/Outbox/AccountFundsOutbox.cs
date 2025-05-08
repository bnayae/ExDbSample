using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funds.Withdraw.WithdrawFunds;

[EvDbAttachMessageType<FundsWithdrewMessage>]
[EvDbOutbox<AccountFundsFactory>]
internal partial class AccountFundsOutbox
{
    protected override void ProduceOutboxMessages(FundsWithdrawnFromAccountEvent payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbAccountFundsViews views,
                                                  AccountFundsOutboxContext outbox)
    {
        base.ProduceOutboxMessages(payload, meta, views, outbox);
    }

}
